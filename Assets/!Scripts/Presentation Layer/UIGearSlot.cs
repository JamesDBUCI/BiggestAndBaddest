using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// Component that controls a UI representation of a Gear slot for an Actor.
/// </summary>
public class UIGearSlot : UISlot<GearSlotController, GearInstance>
{
    /// <summary>
    /// UI Image component representing an empty Gear slot.
    /// </summary>
    public Image EmptyIcon;

    /// <summary>
    /// UI Image component representing a Gear slot that has been disabled.
    /// </summary>
    public Image DisabledGearSlotOverlay;

    protected override Sprite GetContentIcon()
    {
        var castSlot = (GearSlotController)AssignedSlot;
        return castSlot.Contents.Template.Icon;
    }
    protected override void OnUpdateState()
    {
        //Debug.Log("OnUpdateState()");
        var castSlot = (GearSlotController)AssignedSlot;

        EmptyIcon.enabled = castSlot.IsEmpty;
        DisabledGearSlotOverlay.enabled = !AssignedSlot.Enabled;
    }
}