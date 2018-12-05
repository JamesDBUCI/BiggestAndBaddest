using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public abstract class AbstractTimerManager : IActorOwned
{
    public Actor Origin { get; protected set; }

    public AbstractTimerManager(Actor origin)
    {
        Origin = origin;
    }

    protected abstract void CullExpiredTimers();
    public abstract void CleanUpListeners();
}

//Holy Generic Type Constraints, Batman!
//basically looks like this when not in abstract:
//CrowdControlTimerManager<CrowdControlTimer, CrowdControl, CrowdControlTemplate>
public abstract class AbstractTimerManager<TimerType, InstanceItemType, TemplateType> : AbstractTimerManager
    where TimerType : CombatTimerWithPayload<InstanceItemType>
    where InstanceItemType : AbstractTemplateInstance<TemplateType>/*, IHaveDuration*/
    where TemplateType : AbstractAssetTemplate
{
    public AbstractTimerManager(Actor parent)
        :base(parent) { }

    public virtual bool HasAny
    {
        get
        {
            if (_allTimers != null)
            {
                return _allTimers.Any(t => t.InProgress);
            }
            return false;
        }
    }

    protected List<TimerType> _allTimers = new List<TimerType>();

    public bool TryAddTimer(InstanceItemType instance, float duration)
    {
        TimerType unused;
        return TryAddTimer(instance, duration, out unused);
    }
    public bool TryAddTimer(InstanceItemType instance, float duration, out TimerType timerHandle)
    {
        //add something and return its timer (for early cancellation)

        timerHandle = null;

        CullExpiredTimers();

        if (duration <= 0)
            return false;

        if (!ValidateOrLegalizeInstanceItem(instance))
            return false;

        Debug.Log(string.Format("Applying {0} for {1} second(s).", instance.Template.NameExternal, duration));
        timerHandle = GetNewTimer(instance, duration);
        _allTimers.Add(timerHandle);
        return true;
    }

    protected virtual bool ValidateOrLegalizeInstanceItem(InstanceItemType instance) { return true; }
    protected abstract TimerType GetNewTimer(InstanceItemType instance, float duration);

    public bool GetAllPayloadsAndRemainingTime(out List<InstanceItemType> allPayloadsAndDuration)
    {
        //mostly for UI
        allPayloadsAndDuration = null;
        if (!HasAny)
            return false;

        allPayloadsAndDuration = _allTimers.Select(t => GetInstanceItemWithDuration(t.Payload, t.RemainingTime)).ToList();
        return true;
    }
    protected abstract InstanceItemType GetInstanceItemWithDuration(InstanceItemType instance, float duration);
    
    public bool EndOldestTimerOfType(string internalName, int count = 1)
    {
        return EndOldestTimerOfType(i => i.Template.name == internalName, count);
    }
    public bool EndOldestTimerOfType(TemplateType template, int count = 1)
    {
        return EndOldestTimerOfType(i => i.Template == template, count);
    }
    public bool EndOldestTimerOfType(System.Predicate<InstanceItemType> predicate, int count = 1)
    {
        if (!HasAny)
            return false;

        TimerType oldestTimer;
        for (int i = 0; i < count; i++)
        {
            oldestTimer = _allTimers.FirstOrDefault(t => predicate(t.Payload));
            if (oldestTimer == null)
                return false;

            EndTimer(oldestTimer);
        }
        return true;
    }
    public bool EndAllTimersOfType(string internalName)
    {
        return EndAllTimersOfType(t => t.Template.name == internalName);
    }
    public bool EndAllTimersOfType(TemplateType template)
    {
        return EndAllTimersOfType(t => t.Template == template);
    }
    protected bool EndAllTimersOfType(System.Predicate<InstanceItemType> predicate)
    {
        List<TimerType> removedTimers = _allTimers.Where(t => predicate(t.Payload)).ToList();
        removedTimers.ForEach(t => EndTimer(t));
        return removedTimers.Count > 0;
    }
    public bool EndTimer(TimerType timer)
    {
        if (timer == null)
            return false;

        if (!HasAny)
            return false;

        if (!_allTimers.Contains(timer))
            return false;

        EndTimer_Internal(timer);

        CullTimer(timer);
        return true;
    }
    protected virtual void EndTimer_Internal(TimerType timer)
    {
        timer.AutoCompleteTimer();
    }

    public bool HasActiveTimer(string internalName)
    {
        return HasActiveTimer(i => i.Template.name == internalName);
    }
    public bool HasActiveTimer(TemplateType template)
    {
        return HasActiveTimer(i => i.Template == template);
    }
    public bool HasActiveTimer(System.Predicate<InstanceItemType> predicate)
    {
        return FindAllActiveTimers().Exists(t => predicate(t.Payload));
    }
    public int CountActiveTimers()
    {
        return CountActiveTimers(i => true);
    }
    public int CountActiveTimers(string internalName)
    {
        return CountActiveTimers(i => i.Template.name == internalName);
    }
    public int CountActiveTimers(TemplateType template)
    {
        return CountActiveTimers(i => i.Template == template);
    }
    public int CountActiveTimers(System.Predicate<InstanceItemType> predicate)
    {
        return FindActiveTimers(i => predicate(i)).Count;
    }
    protected List<TimerType> FindActiveTimers(System.Predicate<InstanceItemType> predicate)
    {
        return FindAllActiveTimers().Where(t => predicate(t.Payload)).ToList();
    }
    protected List<TimerType> FindAllActiveTimers()
    {
        if (!HasAny)
            return new List<TimerType>();

        return _allTimers.Where(t => t.InProgress).ToList();
    }
    protected bool FindBiggestImpact(System.Func<InstanceItemType, float> valueSelector, bool highestValue, out float strongestImpactValue)
    {
        strongestImpactValue = 0;

        if (!HasAny)
            return false;

        List<float> selectedValueList = _allTimers.Select(t => valueSelector(t.Payload)).ToList();
        selectedValueList.Sort();

        //reverse the order if we're getting the highest value
        if (highestValue)
            selectedValueList.Reverse();

        strongestImpactValue = selectedValueList[0];
        return true;
    }

    protected void CullTimer(TimerType timer)
    {
        timer.CleanUpListeners();     //no memory leaks please
        _allTimers.Remove(timer);
    }
    protected override void CullExpiredTimers()
    {
        _allTimers.Where(t => !t.InProgress).ToList().ForEach(t => CullTimer(t));
    }

    public override void CleanUpListeners()
    {
        new List<TimerType>(_allTimers).ForEach(cct => CullTimer(cct));
    }
}