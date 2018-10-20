using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMod
{
    string GetNameInternal();
    string GetNameExternal();
    ModDescription GetDescription();
    AffixSlotEnum GetAffixSlot();
    StatChange[] GetStatChanges(StatChangeTypeEnum ofType);
    string[] GetFlags();
}

public class Mod : IMod
{
    //generated from template, not modified by Unity
    readonly ModTemplate _template;
    readonly List<StatChange> _rolledChanges;

    public Mod(ModTemplate template, List<StatChange> statChanges)
    {
        _template = template;
        _rolledChanges = new List<StatChange>(statChanges);     //cloned list
    }

    public AffixSlotEnum GetAffixSlot()
    {
        return _template.AffixSlot;
    }

    public ModDescription GetDescription()
    {
        //Debug.Log("Hello from class Mod.");
        List<string> effectsTemplate = new List<string>();
        List<string> effectsRolled = new List<string>();

        for (int i = 0; i < _template.StatChanges.Count; i++)
        {
            StatChangeTemplate changeTemplate = _template.StatChanges[i];

            StatChangeType changeType;
            if (!StatChangeType.TryGet(changeTemplate.ChangeType, out changeType))
                continue;

            string affectedStatExternalName = _template.StatChanges[i].affectedStat.ExternalName;

            effectsTemplate.Add(changeType.GetFormattedValueString(changeTemplate.MinValue, changeTemplate.MaxValue, affectedStatExternalName));
            effectsRolled.Add(changeType.GetFormattedValueString(_rolledChanges[i].Value, affectedStatExternalName));
        }

        for (int i = 0; i < _template.Flags.Count; i++)
        {
            //add all flag descriptions (later, maybe have separate internal and external descriptions)
            effectsRolled.Add(_template.Flags[i]);
            effectsTemplate.Add(_template.Flags[i]);
        }

        //return the effect descriptions
        return new ModDescription(_template.NameExternal, _template.AffixSlot.ToString(), effectsTemplate, effectsRolled);
    }

    public string[] GetFlags()
    {
        return _template.Flags.ToArray();
    }

    public string GetNameExternal()
    {
        return _template.NameExternal;
    }

    public string GetNameInternal()
    {
        return _template.name;  //returns the asset name from the editor, such as "New Mod Template"
    }

    public StatChange[] GetStatChanges(StatChangeTypeEnum ofType)
    {
        List<StatChange> statChangeSubset = _rolledChanges.FindAll(change => change.ChangeType == ofType);
        return statChangeSubset.ToArray();
    }
}

