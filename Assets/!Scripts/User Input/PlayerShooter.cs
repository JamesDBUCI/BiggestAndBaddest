using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class PlayerShooter : MonoBehaviour
{
    Actor _parentActor;

    private void OnEnable()
    {
        _parentActor = GetComponent<Actor>();
    }

    private void Update()
    {
        foreach (KeyCode key in SkillButtonManager.GetAllKeyCodes())
        {
            if (Input.GetKey(key))
            {
                GearSlotEnum selectedGearSlot = SkillButtonManager.KeyBindings[key];

                if (selectedGearSlot != GearSlotEnum.UNDEFINED)
                    _parentActor.CombatController.Skills.TryUse(selectedGearSlot);
                else
                {
                    Debug.Log("Skill button is unassigned.");
                }
            }
        }
    }
}
