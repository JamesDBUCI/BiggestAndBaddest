using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillController
{
    //handles all interactions with a Skill

    //fields and field accessors
    public Actor ParentActor { get; private set; }
    public Skill Skill { get; private set; }        //make a new SkillController if you want a new skill.
    public bool Enabled { get; set; }

    public CombatTimer Cooldown { get; private set; }
    public bool IsOnCooldown
    {
        get
        {
            if (Cooldown != null)
            {
                return Cooldown.InProgress;
            }
            return false;
        }
    }

    //ctor
    public SkillController(Actor parent, Skill skill)
    {
        ParentActor = parent;
        Skill = skill;
        Enabled = true;
    }

    //functionality
    public bool TryUse()
    {
        if (IsOnCooldown)               //can't use when it's on cooldown
            return false;

        //handle
        CombatController combatController = ParentActor.CombatController;

        if (Skill.IsInstant)            //if it's an instant Skill
        {
            //can't use if we're channeling and the skill can't be used while channeling
            if (combatController.IsChanneling && !Skill.InstantSpeedInfo.CanUseWhileChanneling)
                return false;

            //cancel my appointments
            if (combatController.HasSkillLock)
                combatController.CancelSkillLock();
            if (combatController.IsChanneling)
                combatController.CancelChanneling();
        }            

        if (combatController.HasSkillLock)          //can't use if we still have skill lock
            return false;

        if (combatController.IsChanneling)          //can't use if we're still channeling
            return false;

        //we can do it

        if (Skill.HasCooldown)
            SetCooldown();      //adjusted cooldown duration after the jump

        combatController.SetSkillLock(this);

        if (Skill.IsChanneled)      //channel if needed (all necessary checks are done after the jump)
        {
            //CombatController.StartChanneling() will call Use() when (and if) it's done
            combatController.StartChanneling(this);
        }            
        else
            Use();

        return true;
    }
    public void Use()
    {
        //Don't call this one manually. Only called when everything has been cleared by TryUse().

        CombatController cc = ParentActor.CombatController;
        AttackType foundType = Skill.GetAttackType();

        //apply immediate effects (these can't use combatant parameters, but can target deterministically)
        ApplyAttackEffects(SkillPhaseTimingEnum.ON_COMFIRM_USE, new List<Actor>());

        //perform actual attack effect (a melee raycast, a projectile, etc...) [ON_FINAL_HIT may trigger here, or somewhere else]
        foundType.DelegateValue(ParentActor, this);
    }

    public bool SetCooldown()
    {
        float adjustedCooldownDuration = ParentActor.CombatController.GetAdjustedCooldownDuration(Skill.CooldownInfo.CooldownDuration);
        if (adjustedCooldownDuration > 0)
        {
            Cooldown = new CombatTimer(ParentActor, adjustedCooldownDuration);
            return true;
        }
        return false;
    }

    private DamagePacket GetPhaseDamagePacket(SkillPhase phase, List<Stat> calculatedStats)
    {
        //with precalculated Stats
        return CombatServices.ConstructDamagePacket(ParentActor, calculatedStats, phase);
    }

    private List<AttackEffectDelegate> GetPhaseAttackEffects(SkillPhase phase)
    {
        List<AttackEffectDelegate> effectDelegates = new List<AttackEffectDelegate>();
        foreach (AttackEffectEnum enumValue in phase.SkillEffects)
        {
            AttackEffectDelegate del;
            if (!AttackEffect.TryGetDelegate(enumValue, out del))
                continue;

            effectDelegates.Add(del);
        }
        return effectDelegates;
    }
    private List<SkillPhase> GetPhasesByTiming(SkillPhaseTimingEnum enumValue)
    {
        return Skill.SkillPhases.Where(ph => ph.SkillPhaseTiming == enumValue).ToList();
    }
    public void ApplyAttackEffects(SkillPhaseTimingEnum phase, List<Actor> targets)
    {
        ApplyAttackEffects(phase, targets, ParentActor.CombatController.Stats.CalculateCurrentStatValues());
    }
    public void ApplyAttackEffects(SkillPhaseTimingEnum phase, List<Actor> targets, List<Stat> precalculatedStatSet)
    {
        if (targets == null)
            return;

        List<SkillPhase> selectedPhases = GetPhasesByTiming(phase);
        foreach (SkillPhase selectedPhase in selectedPhases)
        {
            DamagePacket damagePacket = GetPhaseDamagePacket(selectedPhase, precalculatedStatSet);
            List<AttackEffectDelegate> effects = GetPhaseAttackEffects(selectedPhase);

            effects.ForEach(effect => effect(damagePacket, ParentActor, targets));
        }
    }
}
