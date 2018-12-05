using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearModController : ModController<GearModTemplate>
{
    public List<StatChange> StatChanges { get; private set; }
    public bool HasStatChanges
    {
        get
        {
            if (StatChanges != null && StatChanges.Count > 0)
                return true;
            return false;
        }
    }
    public List<StatusFlag> StatusFlags { get; private set; }
    public bool HasStatusFlags
    {
        get
        {
            if (StatusFlags != null && StatusFlags.Count > 0)
                return true;
            return false;
        }
    }

    public GearModController(GearModTemplate template)
        : base(template)
    {
        //StatChanges = new List<StatChange>();
        //StatusFlags = new List<StatusFlag>();
    }

    public override void RollValues()
    {
        List<StatChange> rerolledChanges = new List<StatChange>();
        var gmt = (GearModTemplate)Template;
        foreach (var change in gmt.StatChanges)
        {
            rerolledChanges.Add(new StatChange(change));
        }
        StatChanges = rerolledChanges;
        StatusFlags = new List<StatusFlag>(gmt.StatusFlags);
    }
}
