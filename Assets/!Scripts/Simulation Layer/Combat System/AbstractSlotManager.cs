using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public abstract class AbstractSlotManager
{
    public Actor ParentActor { get; protected set; }

    public AbstractSlotManager(Actor parent)
    {
        ParentActor = parent;
    }
}
public abstract class AbstractSlotManager<SlotType, ContentType> : AbstractSlotManager where SlotType : AbstractSlotController<ContentType>
{
    protected Dictionary<GearSlotEnum, SlotType> _slots;

    //events
    public UnityEvent onBeforeSlotChanged = new UnityEvent();
    public UnityEvent onAfterSlotChanged = new UnityEvent();
    public UnityEvent onSlotSetEnabled = new UnityEvent();

    public AbstractSlotManager(Actor parent)
        :base(parent) { }

    public void InitSlots()
    {
        ForEachSlot(slot =>
        {
            slot.CleanUpListeners();
        });

        _slots = GetNewEmptyDictionary();
    }
    protected abstract Dictionary<GearSlotEnum, SlotType> GetNewEmptyDictionary();

    protected bool SlotIsValid(GearSlotEnum gearType)
    {
        return _slots.ContainsKey(gearType);
    }
    protected SlotType FindSlotController(GearSlotEnum gearType)
    {
        if (!SlotIsValid(gearType))
        {
            Debug.LogError("Specified slot does not exist in manager: " + gearType);
            return null;
        }

        SlotType slotType;
        _slots.TryGetValue(gearType, out slotType);
        return slotType;
    }

    public bool SlotIsEmpty(GearSlotEnum gearType)
    {
        return FindSlotController(gearType).IsEmpty;
    }
    public ContentType GetSlotContents(GearSlotEnum gearType)
    {
        return FindSlotController(gearType).Contents;
    }
    public bool HasContentsWhere(System.Predicate<ContentType> predicate)
    {
        return _slots.Values.Where(slot => !slot.IsEmpty).Select(slot => slot.Contents).ToList().Exists(cont => predicate(cont));
    }

    protected void AddContents(SlotType slot, ContentType newContents, bool silenceEvent = false)
    {
        SlotChangeAction(() => slot.AddContents(newContents), silenceEvent);
    }
    public bool AddContents(GearSlotEnum gearType, ContentType newContents, bool silenceEvent = false)    //return true if contents were added
    {
        if (!SlotIsEmpty(gearType))
            return false;

        SlotType slot = FindSlotController(gearType);
        AddContents(slot, newContents);
        return true;
    }
    public void RemoveContentsInSlot(GearSlotEnum gearType, bool silenceEvent = false)
    {
        SlotChangeAction(() => FindSlotController(gearType).RemoveContents(), silenceEvent);
    }
    public void RemoveAllContents(bool silenceEvent = false)
    {
        SlotChangeAction(() => _slots.Values.ToList().ForEach(slot => slot.RemoveContents()), silenceEvent);
    }
    protected void SlotChangeAction(System.Action action, bool silenceEvent)
    {
        RaiseBeforeSlotChange(silenceEvent);

        action();

        RaiseAfterSlotChange(silenceEvent);
    }
    public void SetSlotEnabled(GearSlotEnum gearType, bool value, bool silenceEvent = false)
    {
        SlotChangeAction(() => FindSlotController(gearType).SetEnabled(value, true), silenceEvent);
    }
    public void SetAllSlotsEnabled(bool value, bool silenceEvent = false)
    {
        SlotChangeAction(() => ForEachSlot(s =>
        {
            s.SetEnabled(value, true);
        }), silenceEvent);  //silence individual events, optional single event
    }

    protected void RaiseBeforeSlotChange(bool silenceEvent)
    {
        RaiseEvent(onBeforeSlotChanged, silenceEvent);
    }
    protected void RaiseAfterSlotChange(bool silenceEvent)
    {
        RaiseEvent(onAfterSlotChanged, silenceEvent);
    }
    protected void RaiseSetEnabled(bool silenceEvent = false)
    {
        RaiseEvent(onSlotSetEnabled, silenceEvent);
    }
    protected void RaiseEvent(UnityEvent uEvent, bool dontDoIt = false)
    {
        if (!dontDoIt && uEvent != null)
            uEvent.Invoke();
    }

    public List<KeyValuePair<GearSlotEnum, SlotType>> GetAllSlotControllers()
    {
        //mostly for UI
        return _slots.ToList();
    }
    public void ForEachSlot(System.Action<SlotType> action)
    {
        if (_slots == null)
            return;

        foreach (SlotType slot in _slots.Values.ToList())
        {
            action(slot);
        }
    }

    public virtual void CleanUpListeners()
    {
        ForEachSlot(slot =>
        {
            slot.CleanUpListeners();
        });

        onAfterSlotChanged.RemoveAllListeners();
        onBeforeSlotChanged.RemoveAllListeners();
        onSlotSetEnabled.RemoveAllListeners();
    }
}
