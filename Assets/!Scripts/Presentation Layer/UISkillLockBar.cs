using UnityEngine.UI;

public class UISkillLockBar : UIActorTimerBar
{
    protected override TimerInfo GetTimerInfo()
    {
        return AssignedActor.CombatController.SkillLockInfo;
    }
}