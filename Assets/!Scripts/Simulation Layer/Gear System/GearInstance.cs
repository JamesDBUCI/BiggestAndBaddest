using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearInstance : AbstractTemplateInstance<GearTemplate>
{
    protected GearModManager _modManager = new GearModManager();

    public GearInstance(GearTemplate template, List<GearModController> rolledMods = null)
        :base(template)
    {
        if (template.ImplicitMods != null && template.ImplicitMods.Count > 0)
        {
            template.ImplicitMods.ForEach(imp => _modManager.AddModFromTemplate(imp));
        }
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
