using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class DirectModifierManager : AbstractTimerManager<ModifierTimer, DirectModifier, DirectModifierTemplate>
{
    public override bool HasAny
    {
        //slightly different HasAny property
        //doesn't need timers to be in progress to count them as active
        //(for the small window of time where the timer may be done, but the final effects are waiting to proc)

        get
        {
            if (_allTimers != null)
            {
                if (_allTimers.Count > 0)
                    return true;
            }
            return false;
        }
    }

    public DirectModifierManager(Actor parent)
        :base(parent) { }

    public bool AddTimer(string internalName, float duration, out ModifierTimer timerHandle)
    {
        timerHandle = null;

        DirectModifierTemplate foundModifier;
        if (!GameDatabase.DirectModifiers.TryFind(internalName, out foundModifier))
            return false;

        return TryAddTimer(new DirectModifier(foundModifier, duration, Origin, false), duration, out timerHandle);
    }
    protected override bool ValidateOrLegalizeInstanceItem(DirectModifier modifier)
    {
        if (!ActorCanReceiveModifier(modifier))
            return false;

        LegalizeModifier(modifier);

        OnModifierApply(modifier);

        return true;
    }
    public bool ActorCanReceiveModifier(DirectModifier modifier)
    {
        if (modifier == null)
            return false;

        //checks go below this line

        //modifier max stack size
        if (modifier.Template.MaxStackSize.Enabled &&
            CountActiveTimers(modifier.Template) >= modifier.Template.MaxStackSize.Value)
            return false;

        //class immunity
        if (Origin.CombatController.CurrentClass.ModifierImmunity.Contains(modifier.Template))
            return false;
        
        return true;
    }
    protected void LegalizeModifier(DirectModifier modifier)
    {
        //legalize values here if needed (change duration, change potency, change modifier template, etc...)
    }

    protected IEnumerator TickCoroutine()
    {
        //check every frame until we tick, then wait the tick duration
        //this is structured to circumvent an exploit where you might be able to pause before each tick and resume right after

        //forever (until coroutine is manually stopped)
        while(true) //<-- careful: make sure this doesn't go infinite without yeilding
        {
            //forever until broken
            while(true) //<-- careful: make sure this doesn't go infinite without yeilding
            {
                //if the game isn't paused
                if (!Game.Self.IsPaused)
                {
                    //do one tick of all modifiers
                    ExecuteTick();

                    //break the enclosing loop after we did the tick
                    break;
                }
                //if the game is paused
                else
                {
                    //yield control to the rest of Unity until we're called again on the next frame
                    //control will pick up at the IsPaused check
                    yield return null;
                }
            }

            //yield control to the rest of Unity for the modifier tick duraton (a fraction of a second, but more than a couple frames)
            yield return new WaitForSeconds(Const.MODIFIER_TICK_DURATION);
        }
    }
    protected void ExecuteTick()
    {
        //perform one modifier tick 

        if (!HasAny)
            return;

        //tick each in-progress timer
        foreach (ModifierTimer timer in FindAllActiveTimers())
        {
            //grab a handle
            DirectModifierTemplate modifier = timer.Payload.Template;

            //execute each per tick effect

            //damage/healing
            if (modifier.DamageOrHealingPerTick == null || modifier.DamageOrHealingPerTick.Count == 0)
            {
                CombatServices.TakeDamageOrHeal(Origin, modifier.DamageOrHealingPerTick);
            }

            //more go here
        }

        //proc each completed timer's final effects
        //this section is the OFFICIAL proc of "natural expiration". any other removal will not proc as if natural

        foreach (ModifierTimer timer in _allTimers.Where(t => !t.InProgress))
        {
            OnModifierExpire(timer.Payload, true);
            CullTimer(timer);
        }
    }

    protected void OnModifierApply(DirectModifier modifier)
    {
        ////added on apply
        //var addedOnApply = modifier.Template.AddedOnApply ?? new List<DirectModifierTemplate.AddedMod>();   //the list or an empty one
        //foreach (var added in addedOnApply)
        //{
        //    if (added.IgnoreIfParentIsLinked && modifier.IsLinkedModifier)
        //        continue;

        //    TryAddTimer(new DirectModifier(added.Modifier, added.Duration, modifier.Origin, true));
        //}

        //removed on apply
        var removedOnApply = modifier.Template.RemovedOnApply ?? new List<DirectModifierTemplate.RemovedMod>();   //the list or an empty one
        foreach (var removed in removedOnApply)
        {
            DirectModifierTemplate modifierToRemove = removed.Modifier;

            if (removed.RemoveLimitedStackCount.Enabled)
            {
                EndOldestTimerOfType(modifierToRemove, removed.RemoveLimitedStackCount.Value);
            }
            else
            {
                EndAllTimersOfType(modifierToRemove);
            }
        }
    }
    protected void OnModifierExpire(DirectModifier expiringMod, bool naturalExpiration = false)
    {
        //added on expire
        var addedOnExpire = expiringMod.Template.AddedOnExpire ?? new List<DirectModifierTemplate.AddedMod>();   //the list or an empty one
        foreach (var addedMod in addedOnExpire)
        {
            if (LinkedModifierIsValid(expiringMod, addedMod, naturalExpiration))
            {
                TryAddTimer(new DirectModifier(addedMod.Modifier, addedMod.Duration, expiringMod.Origin, true), addedMod.Duration);
            }
        }

        ////removed on expire
        //var removedOnExpire = expiringMod.Template.RemovedOnExpire ?? new List<DirectModifierTemplate.RemovedMod>();   //the list or an empty one
        //foreach (var removedMod in removedOnExpire)
        //{
        //    if (LinkedModifierIsValid(expiringMod, removedMod, naturalExpiration))
        //    {
        //        if (removedMod.RemoveLimitedStackCount.Enabled)
        //        {
        //            EndOldestTimerOfType(removedMod.Modifier, removedMod.RemoveLimitedStackCount.Value);
        //        }
        //        else
        //        {
        //            EndAllTimersOfType(removedMod.Modifier);
        //        }
        //    }
        //}
    }
    protected bool LinkedModifierIsValid(DirectModifier parent, DirectModifierTemplate.LinkedMod linked, bool naturalExpiration)
    {
        //if (parent.IsLinkedModifier)
        //    return false;

        if (linked.IgnoreIfUnnaturalExpire && !naturalExpiration)
            return false;

        return true;
    }

    public List<StatChange> GetAllStatChanges()
    {
        //we don't need to check if timers are expired, since culling is done per tick
        return _allTimers.SelectMany(t => t.Payload.Template.StatChanges).ToList();
    }

    protected override void EndTimer_Internal(ModifierTimer timer)
    {
        //we don't want auto-complete from base.EndTimer_Internal(timer);

        OnModifierExpire(timer.Payload, false);   //unnatural expiration
        
        //(base.EndTimer() will cull timer on return)
    }
    protected override void CullExpiredTimers()
    {
        //we override this with nothing because we do all our cleanup in ExecuteTick();
    }

    protected override DirectModifier GetInstanceItemWithDuration(DirectModifier instance, float duration)
    {
        return new DirectModifier(instance.Template, duration, instance.Origin, false);
    }

    protected override ModifierTimer GetNewTimer(DirectModifier instance, float duration)
    {
        return new ModifierTimer(Origin, duration, instance);
    }
}

public class ModifierTimer : CombatTimerWithPayload<DirectModifier>
{
    public ModifierTimer(Actor parent, float duration, DirectModifier modifier)
        :base(parent, duration, modifier) { }
}
