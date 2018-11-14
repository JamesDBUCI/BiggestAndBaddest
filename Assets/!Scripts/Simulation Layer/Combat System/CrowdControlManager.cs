using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class CrowdControlManager : AbstractTimerManager<CrowdControlTimer, CrowdControl, CrowdControlTemplate>
{
    public bool FeatsDisabled { get { return HasActiveTimer(cc => cc.Template.StopUsingFeats); } }
    public bool SpellsDisabled { get { return HasActiveTimer(cc => cc.Template.StopUsingSpells); } }
    public bool MovementSpeedImpared { get { return HasActiveTimer(cc => cc.Template.MoveSpeed < 1); } }
    public bool MovementDisabled { get { return HasActiveTimer(cc => cc.Template.MoveSpeed == 0); } }
    public bool MovementDirectionImpared { get { return HasActiveTimer(cc => cc.Template.MoveClockwiseOffset != 0); } }
    public bool AimingSpeedImpared { get { return HasActiveTimer(cc => cc.Template.AimSpeed < 1); } }
    public bool AimingDisabled { get { return HasActiveTimer(cc => cc.Template.AimSpeed == 0); } }
    public bool AimingDirectionImpared { get { return HasActiveTimer(cc => cc.Template.AimClockwiseOffset != 0); } }

    public CrowdControlManager(Actor parent)
        :base(parent) { }

    protected override bool ValidateOrLegalizeInstanceItem(CrowdControl instance)
    {
        float reductionStatValue = Origin.CombatController.Stats.CalculateCurrentStatValue(Const.CC_REDUCTION_STAT);
        float adjustedCCDuration = GameMath.CalculateAdjustedCCDuration(instance.Duration, reductionStatValue);
        instance = new CrowdControl(instance.Template, adjustedCCDuration);

        return true;
    }
    public bool TryAddTimer(string internalName, float duration, out CrowdControlTimer timerHandle)
    {
        timerHandle = null;

        CrowdControlTemplate foundCC;
        if (!GameDatabase.CrowdControls.TryFind(internalName, out foundCC))
            return false;

        return TryAddTimer(new CrowdControl(foundCC, duration), duration, out timerHandle);
    }
    public bool TryAddTimer(CrowdControl instance, out CrowdControlTimer timerHandle)
    {
        return TryAddTimer(instance, instance.Duration, out timerHandle);
    }
    public float GetCurrentMoveSpeedMultiplier()
    {
        float strongest;
        var strongestValue = FindBiggestImpact(cc => cc.Template.MoveSpeed, true, out strongest) ? strongest : 1;
        //Debug.Log(string.Format("Current move speed multiplier is {0}.", strongestValue));
        return strongestValue;
    }
    public float GetCurrentAimSpeedMultiplier()
    {
        float strongest;
        return FindBiggestImpact(cc => cc.Template.AimSpeed, true, out strongest) ? strongest : 1;
    }
    public float GetCurrentAimOffsetAmount()
    {
        float strongest;
        return FindBiggestImpact(cc => cc.Template.AimClockwiseOffset, true, out strongest) ? strongest : 0;
    }
    public float GetCurrentMoveOffsetAmount()
    {
        float strongest;
        return FindBiggestImpact(cc => cc.Template.MoveClockwiseOffset, true, out strongest) ? strongest : 0;
    }

    protected override CrowdControlTimer GetNewTimer(CrowdControl instance, float duration)
    {
        return new CrowdControlTimer(Origin, duration, instance);
    }

    protected override CrowdControl GetInstanceItemWithDuration(CrowdControl instance, float duration)
    {
        return new CrowdControl(instance.Template, duration);
    }
}

public class CrowdControl : AbstractTemplateInstance<CrowdControlTemplate>, IHaveDuration
{
    public float Duration { get; private set; }
    public CrowdControl(CrowdControlTemplate template, float duration)
        :base(template)
    {
        Duration = duration;
    }
}

public class CrowdControlTimer : CombatTimerWithPayload<CrowdControl>
{
    public CrowdControlTimer(Actor parent, float duration, CrowdControl cc)
        :base(parent, duration, cc) { }
}
