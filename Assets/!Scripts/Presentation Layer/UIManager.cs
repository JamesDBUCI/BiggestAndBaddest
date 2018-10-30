using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UISkillSlot SkillSlotPrefab;
    public Transform SkillSlotPanel;
    private List<UISkillSlot> _activeSlots = new List<UISkillSlot>();
    
    public UISkillLockBar SkillLockBar;
    public UIChannelBar ChannelBar;

    private void Awake()
    {
        if (Game.UI == null)
            Game.UI = this;
        else
            Destroy(gameObject);
    }
    public void NewSkillSlot(string buttonName, SkillController assignedController)
    {
        UISkillSlot newSlot = Instantiate(SkillSlotPrefab, SkillSlotPanel);
        newSlot.SetButton(buttonName);
        newSlot.SetSkill(assignedController);

        _activeSlots.Add(newSlot);
    }

    public void TestSetup()
    {
        string[] dummyInputButtonNames = new string[] { "Left-click", "Right-click", "1", "2", "3", "4", "5" };
        if (Game.Self.Player != null)
        {
            var playerSkillsList = Game.Self.Player.CombatController.Skills.GetAllSkillsAndSlotIndexes();
            foreach (var playerSkill in playerSkillsList)
            {
                NewSkillSlot(dummyInputButtonNames[playerSkill.Value1], playerSkill.Value2);
            }

            SkillLockBar.AssignActor(Game.Self.Player);
            ChannelBar.AssignActor(Game.Self.Player);
        }
    }
}
