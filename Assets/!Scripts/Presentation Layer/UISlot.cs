using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Abstract class for UI representations of slots that relate 1-to-1 to Gear types (i.e. Skills).
/// </summary>
public abstract class UISlot : MonoBehaviour
{
    /// <summary>
    /// Represented slot controller.
    /// </summary>
    public AbstractSlotController AssignedSlot { get; protected set; }

    /// <summary>
    /// Assign a slot controller to this UI element.
    /// </summary>
    public void AssignSlot(AbstractSlotController slot)
    {
        if (slot == null)
            return;

        AssignedSlot = slot;
        slot.onChangeEnabled.AddListener(UpdateState);

        UpdateState();
    }

    /// <summary>
    /// Called when the assigned slot controller is updated.
    /// </summary>
    public abstract void UpdateState();
}

/// <summary>
/// Abstract class for UI representations of slots that relate 1-to-1 to Gear types (i.e. Skills).
/// </summary>
public abstract class UISlot<SlotType, ContentType> : UISlot where SlotType : AbstractSlotController<ContentType>
{
    /// <summary>
    /// UI Image component for displaying a Sprite representing the slot contents.
    /// </summary>
    public Image ContentIcon;

    /// <summary>
    /// Called when the assigned slot controller is updated.
    /// </summary>
    public override void UpdateState()
    {
        var castSlot = (SlotType)AssignedSlot;

        if (castSlot.IsEmpty)
        {
            ContentIcon.enabled = false;
        }
        else
        {
            ContentIcon.sprite = GetContentIcon();
            ContentIcon.enabled = true;
        }

        OnUpdateState();
    }

    /// <summary>
    /// Obtains a Sprite representing the slot contents. Override this or it will return null (blank Sprite).
    /// </summary>
    /// <returns>Returns null if not overridden to return an icon representing the slot contents.</returns>
    protected virtual Sprite GetContentIcon() { return null; }

    /// <summary>
    /// Called after UpdateState() functionality. Override this to add functionality when slot is updated.
    /// </summary>
    protected virtual void OnUpdateState() { }
}
