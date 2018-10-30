using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class ModManager
{
    //handles all interaction with a moddable thing's mods
    //an IModdable must have one of these (or be able to procure one upon request)

    //internal managed list of mod instances (these have been rolled)
    protected List<ModController> _mods = new List<ModController>();

    public void RerollMod(ModTemplate template)
    {
        //reroll the values of a mod

        if (!HasMod(template))          //if it has it
            return;

        FindMod(template).RollValues();
    }
    public void RerollModsWhere(System.Predicate<ModController> predicate)
    {
        _mods.Where(m => predicate(m)).ToList().ForEach(m => m.RollValues());
    }
    public void RerollAllMods()
    {
        RerollModsWhere(m => true);
    }
    public void RemoveMod(ModTemplate template)
    {
        _mods.RemoveAll(m => m.Template == template);     //remove all mods that match
    }
    private ModController FindMod(ModTemplate template)
    {
        //internal method for finding mods that come from a template
        return _mods.Find(m => m.Template == template);
    }
    public bool HasMod(ModTemplate template)
    {
        //does this mod controller have a mod with this template
        return _mods.Exists(m => m.Template == template);
    }
    public int CountMods()
    {
        return _mods.Count; //herp derp
    }
    public int CountMods(AffixSlotEnum withAffix)
    {
        //make predicate for comparing affix slot, then pass to method below
        AffixSlot affix;
        if (!AffixSlot.TryGet(withAffix, out affix))
            return 0;
        return CountMods(m => m.Affix == affix);
    }
    public int CountMods(System.Predicate<ModController> predicate)
    {
        //return number of mods which match predicate
        return _mods.Where(m => predicate(m)).Count();
    }

    public string GetModifiedName(string baseName)
    {
        //get a base name modified by all mods on this mod controller
        string returnString = baseName;

        //iterate all mods in list
        foreach (ModController mod in _mods)
        {
            //each mod will modify the name by accessing the pattern prescribed by the AffixSlot it's using
            //implicits and uniques will do nothing, prefixes and suffixes will apply changes
            returnString = mod.Affix.ModifyName(returnString, mod.NameExternal);
        }
        return returnString;
    }
}
public abstract class ModManager<TemplateType, ControllerType> : ModManager where TemplateType : ModTemplate where ControllerType : ModController<TemplateType>
{
    public abstract void AddModFromTemplate(TemplateType template);
    public void AddExactMod(ControllerType mod)
    {
        //add a mod
        //validity checks are left to you

        _mods.Add(mod);
    }
}