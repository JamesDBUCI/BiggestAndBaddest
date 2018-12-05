using UnityEngine.UI;
using UnityEngine;

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