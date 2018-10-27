using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public SkillSlotController SlotController1;
    public SkillSlotController SlotController2;
    public SkillLockBar SkillLockBar;
    public ChannelBar ChannelBar;

    private void Awake()
    {
        if (Game.UI == null)
            Game.UI = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        
    }
    public void TestSetup()
    {
        SlotController1.SetButton("Left-Click");
        SlotController2.SetButton("Right-Click");

        if (Game.Self.Player != null)
        {
            SlotController1.SetSkill(Game.Self.Player.CombatController.Skills.GetSkillInSlot(0));
            SlotController2.SetSkill(Game.Self.Player.CombatController.Skills.GetSkillInSlot(1));

            SkillLockBar.SetNewActor(Game.Self.Player);
            ChannelBar.SetNewActor(Game.Self.Player);
        }
    }
}
