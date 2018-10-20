using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ModServices
{
    //handles most interaction with Mod system.

    public const int MAX_PREFIX_SUFFIX = 2;     //update this with rarity system

    private static DatabaseHelper<ModTemplate> _modTemplateDB = new DatabaseHelper<ModTemplate>("ModTemplates", "Mod Template");
    public static bool LoadModTemplateDB()
    {
        return _modTemplateDB.Load();
    }

    public static bool ApplyNewRandomMod(IModdable moddable)
    {
        //grab a handle
        ModController mc = moddable.GetModController();
        if (mc == null)
            return false;

        //check for max mods of affix
        bool maxPrefix = mc.CountMods(AffixSlotEnum.PREFIX) >= MAX_PREFIX_SUFFIX;
        bool maxSuffix = mc.CountMods(AffixSlotEnum.SUFFIX) >= MAX_PREFIX_SUFFIX;

        if (maxPrefix && maxSuffix)
        {
            Debug.Log("No mod was applied to moddable because all affix slots were full.");
            return false;
        }

        //get a random mod that matches the viability predicate (method is below)
        ModTemplate selectedModTemplate;
        if (!_modTemplateDB.TryGetRandom(template => ModTemplateViabilityPredicate(mc, template), out selectedModTemplate))
        {
            Debug.Log("No mod was applied to moddable because no mods were viable.");
            return false;
        }

        //apply (and roll) mod
        mc.AddModFromTemplate(selectedModTemplate);
        return true;
    }
    private static bool ModTemplateViabilityPredicate(ModController mc, ModTemplate template)
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
        if (foundSlot.CountIsLimited)
        {
            if (mc.CountMods(template.AffixSlot) >= MAX_PREFIX_SUFFIX)
                return false;
        }

        //viable
        return true;
    }

    public static Mod CreateMod(ModTemplate template)
    {
        //create a new mod instance from a template

        List<StatChange> rolledChanges = new List<StatChange>();

        //iterate stat change templates from mod template
        foreach (StatChangeTemplate change in template.StatChanges)
        {
            //roll a value for this stat change template
            float rolledValue = RollValue(change);
            
            //apply the roll to a new stat change instance
            StatChange rolledChange = new StatChange(change, rolledValue);
            
            //add it to the list
            rolledChanges.Add(rolledChange);
        }

        //return a new mod instance with rolled stat changes
        return new Mod(template, rolledChanges);
    }

    private static float RollValue(StatChangeTemplate template)
    {
        //Math has been broken down for readability
        float precision = template.Precision;
        int adjustedMin = Mathf.FloorToInt(template.MinValue / precision);
        int adjustedMax = Mathf.FloorToInt(template.MaxValue / precision);
        float roll = Random.Range(adjustedMin, adjustedMax + 1) * precision;

        //if values are [precision = 0.5, min = 4, max = 13]
        //adjustedMin = 4 / 0.5 = 4 * 2 = 8
        //adjustedMax = 13 / 0.5 = 13 * 2 = 26
        //roll is between 8 and 26 (27 is excluded), then multiplied by 0.5 (roll of 21 = final value of 10.5)

        //if values are [precision = 4, min = 8, max = 28]
        //adjustedMin = 8 / 4 = 2
        //adjustedMax = 28 / 4 = 7
        //roll is between 2 and 7 (8 is excluded), then multiplied by 4 (roll of 3 = final value of 12)

        return roll;
    }

    public static void TestModSystem(IModdable moddable)
    {
        ApplyNewRandomMod(moddable);
        ApplyNewRandomMod(moddable);
        ModController testMC = moddable.GetModController();

        Debug.Log("Added Mod(s) to test moddable.");
        Debug.Log("Test moddable mod count = " + testMC.CountMods());
        Debug.Log("Test moddable modified name = " + testMC.GetModifiedName("BASE NAME"));

        List<ModDescription> modDescriptions = testMC.GetAllModDescriptions();
        for (int i = 0; i < modDescriptions.Count; i++)
        {
            LogModDescription(modDescriptions[i]);
        }
    }

    public static void LogModDescription(ModDescription modDescription)
    {
        Debug.Log("Mod Name: " + modDescription.NameExternal);
        Debug.Log("Mod Affix: " + modDescription.AffixSlot);

        for (int i = 0; i < modDescription.EffectDescriptionsTemplate.Count; i++)
        {
            Debug.Log("Effect " + i + " template: " + modDescription.EffectDescriptionsTemplate[i]);
            Debug.Log("Effect " + i + " rolled: " + modDescription.EffectDescriptionsRolled[i]);
        }
    }
}

public struct ModDescription
{
    //a packet of strings for displaying mod descriptions
    public readonly string NameExternal;
    public readonly string AffixSlot;
    public readonly List<string> EffectDescriptionsTemplate;
    public readonly List<string> EffectDescriptionsRolled;

    public ModDescription(string nameExternal, string affixSlot, List<string> effectsTemplate, List<string> effectsRolled)
    {
        NameExternal = nameExternal;
        AffixSlot = affixSlot;
        EffectDescriptionsTemplate = new List<string>(effectsTemplate);
        EffectDescriptionsRolled = new List<string>(effectsRolled);
    }
}

public interface IModdable
{
    ModController GetModController();
}
