using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StatServices
{
    public static DatabaseHelper<StatTemplate> StatTemplateDB = new DatabaseHelper<StatTemplate>("Stats", "Combat Stat");
    public static DatabaseHelper<StatFlag> StatFlagDB = new DatabaseHelper<StatFlag>("Flags", "Stat Flag");

    public static bool LoadStatDBs()
    {
        if (!StatTemplateDB.Load())
            return false;
        if (!StatFlagDB.Load())
            return false;
        return true;
    }

    public static Stat CreateStat(StatTemplate template)
    {
        Stat newStat = new Stat(template);
        newStat.SetValue(template.DefaultValue);
        return newStat;
    }
    public static List<Stat> GetInitializedStatList()
    {
        List<Stat> statList = new List<Stat>();
        foreach(StatTemplate template in StatTemplateDB.GetAllAssets())
        {
            statList.Add(CreateStat(template));
        }
        return statList;
    }
    public static void TestStatSystem(IHaveStats actor)
    {
        StatController sc = actor.GetStatController();

        StatTemplate foundTemplate;
        if (!StatTemplateDB.TryFind("strength", out foundTemplate))
            return;

        Debug.Log("Base Strength stat: " + sc.GetBaseStatValue(foundTemplate));
        Debug.Log("Current Strength stat: " + sc.CalculateCurrentStatValue(foundTemplate));

        sc.AddStatChanges(new List<StatChange>()
        {
            new StatChange("strength", StatChangeTypeEnum.PLUS, 25),
            new StatChange("strength", StatChangeTypeEnum.INCREASED, 0.25f),
        });

        Debug.Log("Added stat changes.");
        Debug.Log("Stat change count = " + sc.CountStatChanges());

        Debug.Log("Base Strength stat: " + sc.GetBaseStatValue(foundTemplate));
        Debug.Log("Current Strength stat: " + sc.CalculateCurrentStatValue(foundTemplate));

        StatFlag foundFlag;
        if (!StatFlagDB.TryFind("cooling strike", out foundFlag))
            return;

        Debug.Log("stat controller has \"Cooling Strike\": " + sc.CheckFlag(foundFlag));

        sc.AddFlag(foundFlag);

        Debug.Log("Added stat flag.");

        Debug.Log("stat controller has \"Cooling Strike\": " + sc.CheckFlag(foundFlag));
    }
}