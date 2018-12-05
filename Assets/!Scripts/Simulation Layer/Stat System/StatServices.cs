using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SkillPhaseTimingEnum
{
    ON_USE, ON_CHANNEL_COMPLETE, MAIN_HIT
}
public static class StatServices
{
    public static StatController CreateStat(StatTemplate template)
    {
        StatController newStat = new StatController(template);
        newStat.SetValue(template.DefaultValue);
        return newStat;
    }
    public static StatController CreateNewStatFromClassStatTemplate(ActorClassStatTemplate classStat)
    {
        if (string.IsNullOrEmpty(classStat.InternalName))
            return null;

        StatTemplate foundTemplate;
        if (!GameDatabase.Stats.TryFind(classStat.InternalName, out foundTemplate))
            return null;

        return new StatController(foundTemplate, classStat.BaseValue);
    }
    public static StatChange RollStatChangeFromTemplate(StatChangeTemplate template)
    {
        if (template == null)
        {
            Debug.LogError("Tried to roll stat change from null template");
            return null;
        }

        return new StatChange(template);
    }
}