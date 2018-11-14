using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearModManager : ModManager<GearModTemplate, GearModController>
{
    public override void AddModFromTemplate(GearModTemplate template)
    {
        AddExactMod(new GearModController(template));
    }
    public List<StatChange> GetStatChanges()
    {
        var changesList = new List<StatChange>();
        foreach (var mod in _mods)
        {
            var gmc = (GearModController)mod;
            if (gmc.HasStatChanges)
            {
                changesList.AddRange(gmc.StatChanges);
            }
        }
        return changesList;
    }
}
