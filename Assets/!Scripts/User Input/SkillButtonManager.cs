using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class SkillButtonManager
{
    //statics
    public static readonly Dictionary<KeyCode, string> Keys = new Dictionary<KeyCode, string>()
    {
        { KeyCode.Mouse0, "Left-click"},
        { KeyCode.Mouse1, "Right-click"},
        { KeyCode.Q, "Q"},
        { KeyCode.E, "E"},
        { KeyCode.Alpha1, "1"},
        { KeyCode.Alpha2, "2"},
        { KeyCode.Alpha3, "3"},
    };
    public static List<KeyCode> GetAllKeyCodes()
    {
        return Keys.Keys.ToList();
    }

    public static Dictionary<KeyCode, GearSlotEnum> KeyBindings;
    public static void InitKeybindings()
    {
        KeyBindings = new Dictionary<KeyCode, GearSlotEnum>();
        
        var allKeyCodes = GetAllKeyCodes();
        var allGearTypes = GearSlotType.GetAllGearSlots();

        if (allKeyCodes.Count != allGearTypes.Count)
            Debug.Log("KeyCode/GearSlotEnum count mismatch.");

        for (int i = 0; i < allKeyCodes.Count; i++)
        {
            KeyBindings.Add(allKeyCodes[i], allGearTypes[i]);
        }
    }

    public static GearSlotEnum GetSkill(KeyCode button)
    {
        GearSlotEnum skillInSlot;
        KeyBindings.TryGetValue(button, out skillInSlot);
        return skillInSlot;
    }
    public static KeyCode GetButton(GearSlotEnum skill)
    {
        return KeyBindings.FirstOrDefault(kvp => kvp.Value == skill).Key;
    }

    public static void AssignSkillToSelectedButton(KeyCode selectedButton, GearSlotEnum selectedSkill)
    {
        GearSlotEnum oldSkill = GetSkill(selectedButton);
        KeyCode oldButton = GetButton(oldSkill);

        //assign selected skill to selected button
        KeyBindings[selectedButton] = selectedSkill;

        //assign old skill to old button
        KeyBindings[oldButton] = oldSkill;

        Debug.Log(string.Format("Assigned {0} slot to {1} button. [and assigned {2} slot to {3} button]",
            selectedSkill, selectedButton, oldSkill, oldButton));
    }
}
