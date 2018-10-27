using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatTimer
{
    //things like Skill-lock and Channeling use this functionality

    //the actor this timer applies to (and who is running the actual timer coroutine)
    readonly protected Actor _parent;

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

    public CombatTimer(Actor parent, float duration, UnityAction onTimerCompleteAction = null, UnityAction onTimerCancelAction = null, UnityAction<Actor> onTimerInterruptAction = null)
    {
        _parent = parent;
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

        thisTimerCoroutine = _parent.StartCoroutine(Timer_Coroutine(duration));
    }

    protected virtual bool CheckInterruptIsValid(Actor interruptor = null) 
    {
        //overwrite this to check if the interrupt is allowed
        return true;
    }
    private void StopTimerCoroutine()
    {
        _parent.StopCoroutine(thisTimerCoroutine);
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
}

public class ChannelingTimer : CombatTimer
{
    public CrowdControlTimer CCTimer { get; private set; }
    public SkillLockTimer LinkedSkillLock { get; private set; }
    public bool MovingCancelsChannel { get; private set; }
    public bool EnemiesCanInterrupt { get; private set; }

    public ChannelingTimer(Actor parent, float duration, SkillController channeledSkill)
        : base(parent, duration)
    {
        SkillInfo_Channeling channelInfo = channeledSkill.Skill.ChannelingInfo;
        bool ChannelInflictsCC = channelInfo.SelfInflictedCC != null;
        if (ChannelInflictsCC)
        {
            var parentCCC = parent.CombatController.CrowdControl;
            //CC immunities checked live by CCController. we apply it, then CCController can decide whether to factor it in
            CrowdControlTimer CCTimer;
            if (parentCCC.TryAddCC(new CrowdControl(channelInfo.SelfInflictedCC, duration), out CCTimer, true))
            {
                onTimerCancel.AddListener(() => parentCCC.EndCC(CCTimer));
            }
            else
            {
                Debug.Log("CCC couldn't add CC.");
            }
        }

        LinkedSkillLock = parent.CombatController.SkillLock;

        MovingCancelsChannel = channelInfo.MovingCancelsChannel;

        EnemiesCanInterrupt = channelInfo.EnemiesCanInterrupt;

        //when the channel is complete, apply any after-channel effects, then fire off the skill we're channeling
        UnityAction channelCompleteAction =
            () => { channeledSkill.ApplyAttackEffects(SkillPhaseTimingEnum.ON_CHANNEL_COMPLETE, new List<Actor>() { channeledSkill.ParentActor });
                channeledSkill.Use();
            };

        //if this channel is cancelled (or interrupted), so is the skill-lock (animation)
        UnityAction channelCancelAction = () => { if (LinkedSkillLock != null) LinkedSkillLock.Cancel(); };

        //when the channel is interrupted, apply any on-channel-interrupt effects (optionally using the reference to the Actor that caused it)
        UnityAction<Actor> channelInterruptAction =
            (interruptor) => channeledSkill.ApplyAttackEffects(SkillPhaseTimingEnum.ON_CHANNEL_INTERRUPT, new List<Actor>() { interruptor });

        onTimerComplete.AddListener(channelCompleteAction);
        onTimerCancel.AddListener(channelCancelAction);
        onTimerInterrupt.AddListener(channelInterruptAction);

        if (MovingCancelsChannel)
            parent.onActorMove.AddListener(moveDistance => Cancel());
    }

    protected override bool CheckInterruptIsValid(Actor interruptor = null)
    {
        return EnemiesCanInterrupt;
    }
}

public class SkillLockTimer : CombatTimer
{
    public SkillLockTimer(Actor parent, float duration)
        :base(parent, duration)
    {
        
    }
}