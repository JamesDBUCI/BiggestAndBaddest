using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatTimer : IActorOwned
{
    //things like Skill-lock and Channeling use this functionality

    //the actor this timer applies to (and who is running the actual timer coroutine)
    public Actor Origin { get; protected set; }

    //the full amount of time to count
    public float TotalDuration { get; private set; }

    //how much time is left
    protected float _durationRemaining = 0;

    //was this Timer interrupted or cancelled?
    public bool CanceledOrInterrupted { get; private set; }

    //was this Timer completed?
    public bool Completed { get { return _durationRemaining == 0; } }

    //are we still counting (channeling, in skill-lock, etc...)?
    public bool InProgress { get { return _durationRemaining > 0 && !CanceledOrInterrupted; } }

    //return the total remaining time (in seconds)
    public float RemainingTime { get { return _durationRemaining; } }

    //return the percent completion of this timer (between 0 and 1)
    public float ProgressNormalized { get { return CanceledOrInterrupted ? 0 : 1 - (_durationRemaining / TotalDuration); } }

    readonly Coroutine thisTimerCoroutine;
    public UnityEvent onTimerComplete = new UnityEvent();
    public UnityEvent onTimerCancel = new UnityEvent();
    public UnityEvent<Actor> onTimerInterrupt = new UnityEventActor();

    public CombatTimer(Actor origin, float duration, UnityAction onTimerCompleteAction = null, UnityAction onTimerCancelAction = null, UnityAction<Actor> onTimerInterruptAction = null)
    {
        Origin = origin;
        TotalDuration = duration;
        CanceledOrInterrupted = false;

        if (onTimerCompleteAction != null)
            onTimerComplete.AddListener(onTimerCompleteAction);
        {
            onTimerCancel.RemoveAllListeners();
            onTimerInterrupt.RemoveAllListeners();
        }

        if (onTimerCancelAction != null)
            onTimerCancel.AddListener(onTimerCancelAction);

        if (onTimerInterruptAction != null)
            onTimerInterrupt.AddListener(onTimerInterruptAction);

        thisTimerCoroutine = Origin.StartCoroutine(Timer_Coroutine(duration));
    }

    protected virtual bool CheckInterruptIsValid(Actor interruptor = null) 
    {
        //overwrite this to check if the interrupt is allowed
        return true;
    }
    private void StopTimerCoroutine()
    {
        Origin.StopCoroutine(thisTimerCoroutine);
    }
    public void Cancel()
    {
        if (CanceledOrInterrupted || Completed)
            return;

        CanceledOrInterrupted = true;

        if (onTimerCancel != null)
            onTimerCancel.Invoke();

        StopTimerCoroutine();
    }
    public bool Interrupt(bool bypassInterruptEffects = false, Actor interruptor = null)
    {
        //forcefully interrupt timer (check validity, cancel timer, and invoke interrupt events)
        if (!CheckInterruptIsValid())
            return false;

        if (onTimerInterrupt != null)
            onTimerInterrupt.Invoke(interruptor);

        Cancel();

        return true;
    }
    protected void SetTimerValue(float exactDurationRemaining)
    {
        _durationRemaining = Mathf.Max(exactDurationRemaining, 0);
    }
    public void AutoCompleteTimer()
    {
        SetTimerValue(0);
    }
    public void MultiplyWaitTime(float multiplier)
    {
        SetTimerValue(_durationRemaining * multiplier);
    }
    public void AddOrRemoveWaitTime(float timeDelta)
    {
        SetTimerValue(_durationRemaining + timeDelta);
    }
    protected IEnumerator Timer_Coroutine(float duration)
    {
        //negative time is treated as zero

        _durationRemaining = duration;
        while (_durationRemaining > 0)
        {
            yield return null;
            if (!Game.Self.IsPaused)                //can't pause your way out of timers, amigo
                AddOrRemoveWaitTime(-Time.deltaTime);
        }

        if (onTimerComplete != null)
            onTimerComplete.Invoke();
    }
    public void CleanUpListeners()
    {
        onTimerCancel.RemoveAllListeners();
        onTimerComplete.RemoveAllListeners();
        onTimerInterrupt.RemoveAllListeners();
    }
}

public abstract class CombatTimerWithPayload<PayloadType> : CombatTimer
{
    public PayloadType Payload { get; protected set; }

    public CombatTimerWithPayload(Actor origin, float duration, PayloadType payload, UnityAction onTimerCompleteAction = null,
        UnityAction onTimerCancelAction = null, UnityAction<Actor> onTimerInterruptAction = null)
        :base(origin, duration, onTimerCompleteAction, onTimerCancelAction, onTimerInterruptAction)
    {
        Payload = payload;
    }
}

public class ChannelingTimer : CombatTimerWithPayload<Skill.Controller>
{
    public ChannelingTimer(Actor origin, float duration, Skill.Controller channeledSkillController)
        : base(origin, duration, channeledSkillController)
    {
        //lots of setup for channeled skills (should this be in CombatController.StartChanneling instead?)

        //handle
        var channelInfo = channeledSkillController.Contents.Template.ChannelInfo;

        //direct modifiers applied to user during channel
        var dmAppliedList = channelInfo.DirectModifiersApplied;
        if (dmAppliedList.Count > 0)
        {
            //handle
            var dmm = Origin.CombatController.DirectModifiers;

            //interate all modifiers applied
            foreach (var dm in dmAppliedList)
            {
                //handle to modifier timer
                ModifierTimer dmTimer;
                
                //add a new direct modifer to parent actor, get a handle to the new timer
                if (!dmm.TryAddTimer(dm, dm.Duration, out dmTimer))
                {
                    //If the direct modifier failed to be added, don't worry about it. Just jump to the next modifier in the list.
                    continue;
                }

                //when this channel is canceled, end the direct modifier timer
                onTimerCancel.AddListener(() => dmm.EndTimer(dmTimer));
                
                //when this channel is completed, end the direct modifier timer
                onTimerComplete.AddListener(() => dmm.EndTimer(dmTimer));
            }
        }

        //effects applied to interrupting actor on interrupt (if there are any)
        var interruptEffectList = channelInfo.InterruptEffects;
        if (interruptEffectList.Count > 0)
        {
            //when interrupted, perform the OnChannelInterrupt method
            onTimerInterrupt.AddListener(OnChannelInterrupt);
        }

        //"on channel complete" phase effects
        onTimerComplete.AddListener(OnChannelComplete);

        //if channel is canceled by moving, when parent actor moves, cancel this channel
        if (channelInfo.IsCanceledByMoving)
            origin.onActorMove.AddListener(moveDistance => Cancel());
    }

    protected override bool CheckInterruptIsValid(Actor interruptor = null)
    {
        //Interruption is valid if channeled skill is interruptable
        return Payload.Contents.Template.ChannelInfo.IsInterruptable;
    }

    protected void OnChannelInterrupt(Actor interruptor)
    {
        //get args for this moment
        Skill.RecordedArgs recordedArgs = new Skill.RecordedArgs(Origin);
        Vector2 originActorSpritePosition = Origin.GetSpritePosition();

        //iterate all interrupt effects to apply
        foreach (var effectInfo in Payload.Contents.Template.ChannelInfo.InterruptEffects)
        {
            //apply the effect to the interrupting actor (if valid)
            effectInfo.Effect.ApplyIfValid(interruptor, Origin, originActorSpritePosition, effectInfo, recordedArgs);
        }
    }
    protected void OnChannelComplete()
    {
        //things to do when the channel is completed

        //perform all On Channel Complete phases
        Payload.PerformPhases(Skill.PhaseTiming.ON_CHANNEL_COMPLETE);

        //set actor skill lock for appropriate duration (dynamically adjusted after the jump)
        Origin.CombatController.SetSkillLock(Payload.Contents.Template.SkillLockInfo.Duration);
    }
}