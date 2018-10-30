using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class CrowdControlManager
{
    public Actor ParentActor { get; private set; }
    public bool HasAny
    {
        get
        {
            if (_allCC != null)
            {
                return _allCC.Any(cct => cct.InProgress);
            }
            return false;
        }
    }
    public bool FeatsDisabled { get { return HasCrowdControlOfType(cc => cc.Template.StopUsingFeats); } }
    public bool SpellsDisabled { get { return HasCrowdControlOfType(cc => cc.Template.StopUsingSpells); } }
    public bool MovementSpeedImpared { get { return HasCrowdControlOfType(cc => cc.Template.MoveSpeed < 1); } }
    public bool MovementDisabled { get { return HasCrowdControlOfType(cc => cc.Template.MoveSpeed == 0); } }
    public bool MovementDirectionImpared { get { return HasCrowdControlOfType(cc => cc.Template.MoveClockwiseOffset != 0); } }
    public bool AimingSpeedImpared { get { return HasCrowdControlOfType(cc => cc.Template.AimSpeed < 1); } }
    public bool AimingDisabled { get { return HasCrowdControlOfType(cc => cc.Template.AimSpeed == 0); } }
    public bool AimingDirectionImpared { get { return HasCrowdControlOfType(cc => cc.Template.AimClockwiseOffset != 0); } }

    List<CrowdControlTimer> _allCC = new List<CrowdControlTimer>();

    public bool TryAddCC(CrowdControl cc, out CrowdControlTimer timerHandle, bool bypassCCReduction = false)
    {
        //add new cc and return its timer (for early cancellation)

        timerHandle = null;

        CullExpiredTimers();

        if (cc.Duration <= 0)
            return false;

        StatManager actorStats = ParentActor.CombatController.Stats;

        //dynamic cc reduction
        float ccDuration = cc.Duration;
        if (!bypassCCReduction)
        {
            ccDuration = GameMath.CalculateAdjustedCCDuration(cc.Duration,
                actorStats.CalculateCurrentStatValue(Const.CC_REDUCTION_STAT));
        }        

        Debug.Log(string.Format("Applying {0} for {1} second(s).", cc.Template.ExternalName, cc.Duration));
        timerHandle = new CrowdControlTimer(ParentActor, ccDuration, cc);
        _allCC.Add(timerHandle);
        return true;
    }
    public bool AddCC(string internalName, float duration, out CrowdControlTimer timerHander)
    {
        timerHander = null;

        CrowdControlTemplate foundCC;
        if (!GameDatabase.CrowdControls.TryFind(internalName, out foundCC))
            return false;

        return TryAddCC(new CrowdControl(foundCC, duration), out timerHander);
    }
    public bool GetAllCCAndRemainingTime(out List<CrowdControl> allCCandDuration)
    {
        //mostly for UI
        allCCandDuration = null;
        if (!HasAny)
            return false;

        allCCandDuration = _allCC.Select(cct => new CrowdControl(cct.CrowdControl.Template, cct.RemainingTime)).ToList();
        return true;
    }
    public float GetCurrentMoveSpeedMultiplier()
    {
        float strongest;
        var strongestValue = FindStrongestEffect(cc => cc.Template.MoveSpeed, true, out strongest) ? strongest : 1;
        //Debug.Log(string.Format("Current move speed multiplier is {0}.", strongestValue));
        return strongestValue;
    }
    public float GetCurrentAimSpeedMultiplier()
    {
        float strongest;
        return FindStrongestEffect(cc => cc.Template.AimSpeed, true, out strongest) ? strongest : 1;
    }
    public float GetCurrentAimOffsetAmount()
    {
        float strongest;
        return FindStrongestEffect(cc => cc.Template.AimClockwiseOffset, true, out strongest) ? strongest : 0;
    }
    public float GetCurrentMoveOffsetAmount()
    {
        float strongest;
        return FindStrongestEffect(cc => cc.Template.MoveClockwiseOffset, true, out strongest) ? strongest : 0;
    }
    private bool EndAllCCOfType(string internalName)
    {
        return EndAllCCOfType(cc => cc.Template.name == internalName);
    }
    public bool EndAllCCOfType(CrowdControlTemplate template)
    {
        return EndAllCCOfType(cc => cc.Template == template);
    }
    private bool EndAllCCOfType(System.Predicate<CrowdControl> predicate)
    {
        List<CrowdControlTimer> removedTimers = _allCC.Where(cct => predicate(cct.CrowdControl)).ToList();
        removedTimers.ForEach(cct => EndCC(cct));
        return removedTimers.Count > 0;
    }
    public bool EndCC(CrowdControlTimer ccTimer)
    {
        if (ccTimer == null)
            return false;

        if (!HasAny)
            return false;

        if (!_allCC.Contains(ccTimer))
            return false;

        ccTimer.AutoCompleteTimer();
        _allCC.Remove(ccTimer);
        return true;
    }

    private bool HasCrowdControlOfType(string internalName)
    {
        return HasCrowdControlOfType(cc => cc.Template.name == internalName);
    }
    public bool HasCrowdControlOfType(CrowdControlTemplate template)
    {
        return HasCrowdControlOfType(cc => cc.Template == template);
    }
    private bool HasCrowdControlOfType(System.Predicate<CrowdControl> predicate)
    {
        if (!HasAny)
            return false;

        return _allCC.Exists(cct => predicate(cct.CrowdControl));
    }
    private bool FindStrongestEffect(System.Func<CrowdControl, float> valueSelector, bool highestValue, out float strongestEffectValue)
    {
        //Debug.Log("Finding strongest effect.");
        strongestEffectValue = 0;
        if (!HasAny)
        {
            //Debug.Log("No CC!");
            return false;
        }

        List<float> selectedValueList = _allCC.Select(cct => valueSelector(cct.CrowdControl)).ToList();
        selectedValueList.Sort();

        //reverse the order if we're getting the highest value
        if (highestValue)
            selectedValueList.Reverse();

        strongestEffectValue = selectedValueList[0];
        return true;
    }

    private void CullExpiredTimers()
    {
        if (_allCC == null)
            return;

        _allCC.RemoveAll(cct => !cct.InProgress);
    }

    public CrowdControlManager(Actor parent)
    {
        ParentActor = parent;
    }
}

[CreateAssetMenu(menuName = "Crowd Control")]
public class CrowdControlTemplate : ScriptableObject
{
    public string ExternalName;
    [Range(0, 1)]
    public float MoveSpeed = 1;
    [Range(0, 1)]
    public float AimSpeed = 1;
    [Range(0, 360)]
    public float MoveClockwiseOffset = 0;
    [Range(0, 360)]
    public float AimClockwiseOffset = 0;
    public bool StopUsingFeats = false;
    public bool StopUsingSpells = false;
}
public class CrowdControl
{
    public CrowdControlTemplate Template { get; private set; }
    public float Duration;

    public CrowdControl(CrowdControlTemplate template, float duration)
    {
        Template = template;
        Duration = duration;
    }
}
public class CrowdControlTimer : CombatTimer
{
    public CrowdControl CrowdControl { get; private set; }      //in the event more data gets added to CC and we wish we had tracked it here
    public CrowdControlTimer(Actor parent, float duration, CrowdControl cc)
        :base(parent, duration)
    {
        CrowdControl = cc;
    }
}
