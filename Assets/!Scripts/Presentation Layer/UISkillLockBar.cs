using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// Component that controls a UI representation of a Skill-lock timer for a Skill.
/// </summary>
public class UISkillLockBar : UIActorTimerBar
{
    protected override bool UpdateTimerInfo()
    {
        if (AssignedActor == null || !AssignedActor.CombatController.HasSkillLock)
        {
            //Debug.Log("Actor is null or not in skill-lock");
            return false;
        }            

        var comCon = AssignedActor.CombatController;

        _progressNormalized = comCon.SkillLockProgressNormalized;
        _remainingTime = comCon.SkillLockRemainingTime;
        return true;
    }
}