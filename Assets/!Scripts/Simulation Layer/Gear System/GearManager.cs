using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearManager
{
    public Actor ParentActor { get; private set; }
    private Dictionary<GearSlotEnum, GearSlotController> _slots = new Dictionary<GearSlotEnum, GearSlotController>();

    //events
    public UnityEvent onBeforeGearChanged = new UnityEvent();
    public UnityEvent onAfterGearChanged = new UnityEvent();

    public GearManager(Actor parent)
    {
        ParentActor = parent;

        foreach (var slotType in GearSlotType.All.Keys)
        {
            _slots.Add(slotType, new GearSlotController(parent));
        }
    }
    private bool SlotIsValid(GearSlotEnum gearType)
    {
        return _slots.ContainsKey(gearType);
    }
    private GearSlotController FindGearSlotController(GearSlotEnum gearType)
    {
        if (!SlotIsValid(gearType))
        {
            Debug.LogError("Specified gear slot does not exist in manager: " + gearType);
            return null;
        }

        GearSlotController gsc;
        _slots.TryGetValue(gearType, out gsc);
        return gsc;
    }

    public bool HasGearInSlot(GearSlotEnum gearType)
    {
        return FindGearSlotController(gearType).HasItem;
    }
    public GearController GetGearInSlot(GearSlotEnum gearType)
    {
        return FindGearSlotController(gearType).Item;
    }
    public bool HasGearItem(GearTemplate itemToMatch)
    {
        return HasGearWhere(item => item.Template == itemToMatch);
    }
    public bool HasGearWhere(System.Predicate<GearController> predicate)
    {
        return _slots.Values.Where(gsc => gsc.HasItem).Select(gsc => gsc.Item).ToList().Exists(item => predicate(item));
    }

    private void AddGearItem(GearController item)
    {
        GearSlotController gsc = FindGearSlotController(item.Template.GearSlot);
        gsc.AddItem(item);
    }
    public bool AddGear(GearController newGear, bool silenceEvent = false)    //return true if gear was added
    {
        if (HasGearInSlot(newGear.Template.GearSlot))
            return false;

        GearChangeAction(() => AddGearItem(newGear), silenceEvent);
        return true;
    }
    public void AddOrReplaceGear(List<GearController> newGear, bool silenceEvent = false)
    {
        //silence individual events
        GearChangeAction(() => newGear.ForEach(gear => AddGear(gear, true)), silenceEvent);
    }
    public void RemoveGearInSlot(GearSlotEnum gearType, bool silenceEvent = false)
    {
        GearChangeAction(() => FindGearSlotController(gearType).RemoveItem(), silenceEvent);
    }
    public void RemoveAllGear(bool silenceEvent = false)
    {
        GearChangeAction(() => _slots.Values.ToList().ForEach(gsc => gsc.RemoveItem()), silenceEvent);
    }
    private void GearChangeAction(System.Action action, bool silenceEvent)
    {
        BeforeGearChange(silenceEvent);

        action();

        AfterGearChange(silenceEvent);
    }
    public void SetGearEnabled(GearSlotEnum gearType, bool value, bool silenceEvent = false)
    {
        GearChangeAction(() => FindGearSlotController(gearType).SetEnabled(value, true), silenceEvent);
    }

    private void BeforeGearChange(bool silenceEvent)
    {
        if (!silenceEvent && onBeforeGearChanged != null)
            onBeforeGearChanged.Invoke();
    }
    private void AfterGearChange(bool silenceEvent)
    {
        if (!silenceEvent && onAfterGearChanged != null)
            onAfterGearChanged.Invoke();
    }

    public List<KeyValuePair<GearSlotEnum, GearSlotController>> GetAllGearSlotControllers()
    {
        //mostly for UI
        return _slots.ToList();
    }
}
