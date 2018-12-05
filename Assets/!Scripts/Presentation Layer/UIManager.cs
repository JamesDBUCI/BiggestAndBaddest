using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UISkillSlot SkillSlotPrefab;
    public Transform SkillSlotPanel;
    private List<UISkillSlot> _activeSkillSlots = new List<UISkillSlot>();

    public UIGearSlot GearSlotPrefab;
    public Transform GearSlotPanel;
    private List<UIGearSlot> _activeGearSlots = new List<UIGearSlot>();

    public UISkillLockBar SkillLockBar;
    public UIChannelBar ChannelBar;

    public UIGlobalCalloutTimer CalloutTimer;

    private void Awake()
    {
        if (Game.UI == null)
            Game.UI = this;
        else
            Destroy(gameObject);
    }
    public void NewSkillSlot(string buttonName, Skill.Controller assignedController)
    {
        UISkillSlot newSlot = Instantiate(SkillSlotPrefab, SkillSlotPanel);
        newSlot.SetButton(buttonName);
        newSlot.SetSkill(assignedController);

        _activeSkillSlots.Add(newSlot);
    }
    public void NewGearSlot(GearSlotController gearSlot)
    {
        UIGearSlot uiSlot = Instantiate(GearSlotPrefab, GearSlotPanel);
        //remember: set blank gear slot icon from gear slot type
        uiSlot.AssignSlot(gearSlot);
        gearSlot.onChangeEnabled.AddListener(uiSlot.UpdateState);

        _activeGearSlots.Add(uiSlot);
    }

    public void TestSetup()
    {
        if (Game.Self.Player != null)
        {
            var playerSkillsList = Game.Self.Player.CombatController.Skills.GetAllSlotControllers();
            foreach (var playerSkill in playerSkillsList)
            {
                if (playerSkill.Value.IsEmpty)  //if the skill controller does not have a skill instance
                    continue;

                string buttonName = SkillButtonManager.Keys[SkillButtonManager.GetButton(playerSkill.Key)];
                NewSkillSlot(buttonName, playerSkill.Value);
            }

            SkillLockBar.AssignActor(Game.Self.Player);
            ChannelBar.AssignActor(Game.Self.Player);

            var playerGearList = Game.Self.Player.CombatController.Gear.GetAllSlotControllers();
            foreach (var kvp in playerGearList)
            {
                NewGearSlot(kvp.Value);
            }
        }
        if (Game.Self.TestBoss != null)
        {
            CalloutTimer.AssignActor(Game.Self.TestBoss);
        }
    }
}
