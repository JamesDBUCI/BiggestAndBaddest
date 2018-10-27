using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StatServices
{
    public static Stat CreateStat(StatTemplate template)
    {
        Stat newStat = new Stat(template);
        newStat.SetValue(template.DefaultValue);
        return newStat;
    }
    public static Stat CreateNewStatFromClassStatTemplate(CombatClassStatTemplate classStat)
    {
        if (string.IsNullOrEmpty(classStat.InternalName))
            return null;

        StatTemplate foundTemplate;
        if (!GameDatabase.Stats.TryFind(classStat.InternalName, out foundTemplate))
            return null;

        return new Stat(foundTemplate, classStat.BaseValue);
    }
}