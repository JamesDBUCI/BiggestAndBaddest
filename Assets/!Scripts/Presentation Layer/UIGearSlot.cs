using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGearSlot : MonoBehaviour
{
    public GearSlotController AssignedSlot { get; private set; }

    public Image EmptyIcon;
    public Image GearIcon;
    public Image DisabledGearSlotOverlay;

    public void AssignSlot(GearSlotController slot)
    {
        AssignedSlot = slot;
        slot.onSlotChanged.AddListener(UpdateState);

        UpdateState();        
    }
    public void UpdateState()
    {
        if (AssignedSlot.HasItem)
            GearIcon.sprite = AssignedSlot.Item.Template.Icon;

        GearIcon.enabled = AssignedSlot.HasItem;
        EmptyIcon.enabled = !AssignedSlot.HasItem;

        DisabledGearSlotOverlay.enabled = !AssignedSlot.Enabled;
    }
}