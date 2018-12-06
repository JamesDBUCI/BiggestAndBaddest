using UnityEngine.UI;

/// <summary>
/// Component that controls a UI representation of a channeling timer for a Skill.
/// </summary>
public class UIChannelBar : UIActorTimerBar
{
    /// <summary>
    /// UI Text component for displaying the name of the channeled Skill.
    /// </summary>
    public Text SkillNameText;

    protected override bool UpdateTimerInfo()
    {
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