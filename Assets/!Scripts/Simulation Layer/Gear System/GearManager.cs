using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearManager : AbstractSlotManager<GearSlotController, GearInstance>
{
    public List<GearInstance> AllEquipped
    {
        get
        {
            return _slots.Values.Where(gsc => !gsc.IsEmpty).Select(gsc => gsc.Contents).ToList();
        }
    }
    public List<GearInstance> AllEnabled
    {
        get
        {
            return _slots.Values.Where(gsc => !gsc.IsEmpty && gsc.Enabled).Select(gsc => gsc.Contents).ToList();
        }
    }

    public GearManager(Actor origin)
        :base(origin) { }

    protected override Dictionary<GearSlotEnum, GearSlotController> GetNewEmptyDictionary()
    {
        var newDict = new Dictionary<GearSlotEnum, GearSlotController>();

        GearSlotType.ForEach(gearType => newDict.Add(gearType, new GearSlotController(ParentActor, gearType)));

        return newDict;
    }

    public List<StatChange> GetAllEnabledStatChanges()
    {
        List<StatChange> allChanges = new List<StatChange>();
        foreach (GearSlotController gsc in _slots.Values)
        {
            if (gsc.IsEmpty || !gsc.Enabled)
                continue;

            allChanges.AddRange(gsc.Contents.GetStatChanges());
        }
        return allChanges;
    }
}
