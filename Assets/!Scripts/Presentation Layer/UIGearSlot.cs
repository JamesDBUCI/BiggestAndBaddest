using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UISlot : MonoBehaviour
{
    public AbstractSlotController AssignedSlot { get; protected set; }

    public void AssignSlot(AbstractSlotController slot)
    {
        if (slot == null)
            return;

        AssignedSlot = slot;
        slot.onChangeEnabled.AddListener(UpdateState);

        UpdateState();
    }
    public abstract void UpdateState();
}
public abstract class UISlot<SlotType, ContentType> : UISlot where SlotType : AbstractSlotController<ContentType>
{
    public Image ContentIcon;

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
    protected virtual Sprite GetContentIcon() { return null; }
    protected virtual void OnUpdateState() { }
}
public class UIGearSlot : UISlot<GearSlotController, GearInstance>
{
    public Image EmptyIcon;
    public Image DisabledGearSlotOverlay;

    protected override Sprite GetContentIcon()
    {
        var castSlot = (GearSlotController)AssignedSlot;
        return castSlot.Contents.Template.Icon;
    }
    protected override void OnUpdateState()
    {
        var castSlot = (GearSlotController)AssignedSlot;

        EmptyIcon.enabled = castSlot.IsEmpty;
        DisabledGearSlotOverlay.enabled = !AssignedSlot.Enabled;
    }
}