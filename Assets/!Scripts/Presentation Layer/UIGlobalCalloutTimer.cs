using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIGlobalCalloutTimer : UIActorTimerBar {

    public Text SkillNameText;

    protected override TimerInfo GetTimerInfo()
    {
        if (AssignedActor == null)
            return new TimerInfo();
        var channelInfo = AssignedActor.CombatController.ChannelingInfo;
        return channelInfo.TimerInfo;
    }
    public override void OnToggleVisible(bool value)
    {
        if (value & SkillNameText != null && AssignedActor != null)
            SkillNameText.text = AssignedActor.CombatController.ChannelingInfo.SkillName;
        if (!value)
            SkillNameText.text = "";
    }
}
