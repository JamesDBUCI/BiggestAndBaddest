using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearController
{
    public GearTemplate Template { get; protected set; }    //If you want to use a different template, make a new GearItem
    protected GearModManager _modManager = new GearModManager();

    public GearController(GearTemplate template, List<GearModController> rolledMods = null)
    {
        Template = template;
        if (rolledMods != null)
        {
            rolledMods.ForEach(mod => _modManager.AddExactMod(mod));
        }
    }
    //mod functions
    public void AddMod(GearModTemplate template)
    {
        _modManager.AddModFromTemplate(template);
    }
    public void AddExactMod(GearModController mod)
    {
        _modManager.AddExactMod(mod);
    }
    public void RerollMods()
    {
        _modManager.RerollAllMods();
    }
    public void RemoveMod(GearModTemplate template)
    {
        _modManager.RemoveMod(template);
    }

    public List<StatChange> GetStatChanges()
    {
        return new List<StatChange>(_modManager.GetStatChanges());
    }
}
