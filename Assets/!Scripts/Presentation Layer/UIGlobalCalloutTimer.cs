using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIGlobalCalloutTimer : UIActorTimerBar {

    public Text SkillNameText;

    protected override bool UpdateTimerInfo()   //return false if data was unavailable (no actor, no channeling)
    {
        //get and set this UI timer's data for this frame
        Debug.Log(AssignedActor.CombatController.IsChanneling);
        AttackManager atkMngr = AssignedActor.GetComponent<AttackManager>();

        if (AssignedActor == null || !AssignedActor.CombatController.IsChanneling || atkMngr == null || !atkMngr.IsPreparing)
            return false;

        var comCon = AssignedActor.CombatController;

        _progressNormalized = comCon.ChannelingProgressNormalized;
        _remainingTime = comCon.ChannelingRemainingTime + atkMngr.SkillOffsetTime;

        return true;
    }
    public override void OnToggleVisible(bool value)
    {
        if (value && (AssignedActor != null))
            SkillNameText.text = AssignedActor.CombatController.ChanneledSkill.Contents.Template.NameExternal;
    }
}
