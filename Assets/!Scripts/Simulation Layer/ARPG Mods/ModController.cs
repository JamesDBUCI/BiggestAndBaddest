using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModController
{
    //handles all interaction with a moddable thing's mods
    //an IModdable must have one of these (or be able to procure one upon request)

    //internal managed list of mod instances (these have been rolled)
    private List<IMod> _mods = new List<IMod>();

    public void AddModFromTemplate(ModTemplate template)
    {
        //roll a new mod instance and add it
        AddExactMod(ModServices.CreateMod(template));
    }
    public void AddExactMod(IMod mod)
    {
        //add a mod
        //validity checks are left to ModServices

        _mods.Add(mod);
    }
    public void RerollMod(ModTemplate template)
    {
        //reroll the values of a mod

        if (!HasMod(template))          //if it has it
            return;

        RemoveMod(template);            //remove it
        AddModFromTemplate(template);   //add a new one
    }
    public void RemoveMod(ModTemplate template)
    {
        //remove a mod
        if (!HasMod(template))          //if it has it
            return;

        _mods.RemoveAll(m => m.GetNameInternal() == template.name);     //remove all mods that match
    }
    private IMod FindMod(ModTemplate template)
    {
        //internal method for finding mods that come from a template
        return _mods.Find(m => m.GetNameInternal() == template.name);
    }
    public bool HasMod(ModTemplate template)
    {
        //does this mod controller have a mod with this template
        IMod foundMod = FindMod(template);
        if (foundMod != null)
            return true;
        return false;
    }
    public int CountMods()
    {
        return _mods.Count; //herp derp
    }
    public int CountMods(AffixSlotEnum withAffix)
    {
        //make predicate for comparing affix slot, then pass to method below
        return CountMods(m => m.GetAffixSlot() == withAffix);
    }
    public int CountMods(System.Func<IMod, bool> predicate)
    {
        //return number of mods which match predicate
        return _mods.Where(predicate).Count();
    }

    public string GetModifiedName(string baseName)
    {
        //get a base name modified by all mods on this mod controller
        string returnString = baseName;

        //iterate all mods in list
        foreach (IMod mod in _mods)
        {
            //grab a handle for affix slot meta data
            AffixSlot affix;
            if (!AffixSlot.TryGet(mod.GetAffixSlot(), out affix))
                continue;

            //each mod will modify the name by accessing the pattern prescribed by the AffixSlot it's using
            //implicits and uniques will do nothing, prefixes and suffixes will apply changes
            returnString = affix.ModifyName(returnString, mod.GetNameExternal());
        }
        return returnString;
    }

    public List<ModDescription> GetAllModDescriptions()
    {
        //Debug.Log("Hello from class ModController.");
        List<ModDescription> modDescriptions = new List<ModDescription>();
        foreach (Mod m in _mods)
        {
            modDescriptions.Add(m.GetDescription());
        }

        return modDescriptions;
    }
}
