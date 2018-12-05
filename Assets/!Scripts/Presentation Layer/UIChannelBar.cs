using UnityEngine.UI;

public class UIChannelBar : UIActorTimerBar
{
    public Text SkillNameText;

    protected override bool UpdateTimerInfo()   //return false if data was unavailable (no actor, no channeling)
    {
        //get and set this UI timer's data for this frame

        if (AssignedActor == null || !AssignedActor.CombatController.IsChanneling)
            return false;

        var comCon = AssignedActor.CombatController;

        _progressNormalized = comCon.ChannelingProgressNormalized;
        _remainingTime = comCon.ChannelingRemainingTime;

        return true;
    }
    public override void OnToggleVisible(bool value)
    {
        if (value && (AssignedActor != null))
            SkillNameText.text = AssignedActor.CombatController.ChanneledSkill.Contents.Template.NameExternal;
    }
}