using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ModServices
{
    //handles most interaction with Mod system.

    public const int MAX_PREFIX_SUFFIX = 2;     //update this with rarity system

    public static bool ApplyNewRandomMod<TemplateType, ControllerType>(ModManager modManager, bool ignoreFullAffixSlot = false)
        where TemplateType : ModTemplate
        where ControllerType : ModController<TemplateType>
    {
        if (modManager == null)
            return false;

        var typedManager = (ModManager<TemplateType, ControllerType>)modManager;

        //get a random mod that matches the viability predicate (method is below)
        TemplateType selectedModTemplate;
        if (!GameDatabase.Mods.TryGetRandomModOfType(template => ModTemplateViabilityPredicate(modManager, template, ignoreFullAffixSlot), out selectedModTemplate))
        {
            Debug.Log("No mod was applied to moddable because no mods were viable.");
            return false;
        }

        //apply (and roll) mod
        typedManager.AddModFromTemplate(selectedModTemplate);
        return true;
    }
    private static bool ModTemplateViabilityPredicate(ModManager mc, ModTemplate template, bool ignoreFullAffixSlot = false)
    {
        //could have done this with a System.Predicate<>, but I like easy mode

        //not viable if we have the mod already
        if (mc.HasMod(template))
            return false;

        //not viable if we can't find the type of affix slot this mod uses
        AffixSlot foundSlot;
        if (!AffixSlot.TryGet(template.AffixSlot, out foundSlot))
            return false;

        //not viable if the number of mods in this affix slot are limited and we are maxed out
        if (!ignoreFullAffixSlot && foundSlot.CountIsLimited)
        {
            if (mc.CountMods(template.AffixSlot) >= MAX_PREFIX_SUFFIX)
                return false;
        }

        //viable
        return true;
    }
}

public class ModDatabase : DatabaseHelper<ModTemplate>
{
    public ModDatabase(string path, string assetTypeName, List<DatabaseHelper> containerList = null)
        :base(path, assetTypeName, containerList) { }

    public bool TryGetRandomModOfType<TemplateType>(out TemplateType foundTemplate) where TemplateType : ModTemplate
    {
        if (TryGetRandomModOfType(asset => true, out foundTemplate))
            return true;
        return false;
    }
    public bool TryGetRandomModOfType<TemplateType>(System.Predicate<TemplateType> predicate, out TemplateType foundTemplate) where TemplateType : ModTemplate
    {
        foundTemplate = null;

        var allModsOfType = GetAllModsOfType<TemplateType>();
        if (allModsOfType.Count == 0)
            return false;

        var modsWherePredicate = allModsOfType.Where(mod => predicate(mod)).ToList();

        if (modsWherePredicate.Count == 0)
            return false;

        foundTemplate = modsWherePredicate[Random.Range(0, allModsOfType.Count)];
        return true;
    }
    public List<TemplateType> GetAllModsOfType<TemplateType>() where TemplateType : ModTemplate
    {
        List<ModTemplate> foundAssets;
        TryFindMany(asset => asset.GetType() == typeof(TemplateType), out foundAssets);
        return foundAssets.Select(uncasted => (TemplateType)uncasted).ToList();
    }
}
