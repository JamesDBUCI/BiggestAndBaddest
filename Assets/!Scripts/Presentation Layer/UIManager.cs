using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highest-level manager component for handling all combat UI functions. There should only be one of these running at a time.
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Prefab that is instantiated to represent one of the Player's Skills.
    /// </summary>
    public UISkillSlot SkillSlotPrefab;

    /// <summary>
    /// UI object under which UISkillSlot instances will be instantiated.
    /// </summary>
    public Transform SkillSlotPanel;

    private List<UISkillSlot> _activeSkillSlots = new List<UISkillSlot>();

    /// <summary>
    /// Prefab that is instantiated to represent one of the Player's Gear items.
    /// </summary>
    public UIGearSlot GearSlotPrefab;

    /// <summary>
    /// UI object under which UIGearSlot instances will be instantiated.
    /// </summary>
    public Transform GearSlotPanel;

    private List<UIGearSlot> _activeGearSlots = new List<UIGearSlot>();

    /// <summary>
    /// The Player's Skill-lock (animation duration) bar.
    /// </summary>
    public UISkillLockBar SkillLockBar;

    /// <summary>
    /// The Player's Channel (wind-up) bar.
    /// </summary>
    public UIChannelBar ChannelBar;

    private void Awake()
    {
        //singleton behavior
        if (Game.UI == null)
            Game.UI = this;
        else
            Destroy(gameObject);
    }
    /// <summary>
    /// Spawn a new UISkillSlot on the UI.
    /// </summary>
    /// <param name="buttonName">The name of the button that will activate this Skill.</param>
    /// <param name="assignedController">The Skill controller that this button represents.</param>
    public void NewSkillSlot(string buttonName, Skill.Controller assignedController)
    {
        UISkillSlot newSlot = Instantiate(SkillSlotPrefab, SkillSlotPanel);
        newSlot.SetButton(buttonName);
        newSlot.SetSkill(assignedController);

        _activeSkillSlots.Add(newSlot);
    }

    /// <summary>
    /// Spawn a new UIGearSlot on the UI.
    /// </summary>
    /// <param name="gearSlot">The Gear slot controller to represent.</param>
    public void NewGearSlot(GearSlotController gearSlot)
    {
        UIGearSlot uiSlot = Instantiate(GearSlotPrefab, GearSlotPanel);
        //remember: set blank gear slot icon from gear slot type
        uiSlot.AssignSlot(gearSlot);
        gearSlot.onChangeEnabled.AddListener(uiSlot.UpdateState);

        _activeGearSlots.Add(uiSlot);
    }

    /// <summary>
    /// Setup UI. This method should be remade later.
    /// </summary>
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
    }
}
