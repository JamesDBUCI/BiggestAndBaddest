using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager
{
    public Actor ParentActor { get; private set; }

    private List<StatController> _baseStats = new List<StatController>();
    private List<StatusFlag> _flags = new List<StatusFlag>();

    //ctor
    public StatManager()
    {
        //write a default copy of every stat in the database
        GameDatabase.Stats.GetAllAssets().ForEach(st => _baseStats.Add(new StatController(st)));
    }
    public StatManager(Actor actor) : this()
    {
        ParentActor = actor;
    }

    //general functions
    public void SetWithCombatClass(ActorClass combatClass, int level, bool resetAllFlagsAndChanges = true)
    {
        foreach (ActorClassStatTemplate classStatTemplate in combatClass.Stats)
        {
            StatTemplate foundTemplate;
            if (!GameDatabase.Stats.TryFind(classStatTemplate.InternalName, out foundTemplate))
            {
                Debug.LogError(string.Format("{0}(CombatClass) calls for stat not found in database ({1})", combatClass.name, classStatTemplate.InternalName));
            }

            StatController foundStat = FindStat(foundTemplate);
            foundStat.SetValue(classStatTemplate.BaseValue);
            foundStat.ChangeValue(classStatTemplate.ValuePerLevel * level);
        }
    }

    //stat functions
    protected StatController FindStat(StatTemplate template)
    {
        return _baseStats.Find(s => s.Template == template);
    }
    public float GetBaseStatValue(StatTemplate template)
    {
        return FindStat(template).Value;
    }
    public void ChangeBaseStatValue(StatTemplate template, float valueDelta)
    {
        FindStat(template).ChangeValue(valueDelta);
    }
    public void SetBaseStatValue(StatTemplate template, float newValue)
    {
        FindStat(template).SetValue(newValue);
    }

    public float CalculateCurrentStatValue(string internalName)
    {
        StatTemplate foundStat;
        if (!GameDatabase.Stats.TryFind(internalName, out foundStat))
            return 0;

        return CalculateCurrentStatValue(foundStat);
    }
    public float CalculateCurrentStatValue(StatTemplate stat)
    {
        //core method

        CombatController c = ParentActor.CombatController;

        //gear stat changes
        var allStatChanges = c.Gear.GetAllEnabledStatChanges();

        //direct modifier stat changes
        allStatChanges.AddRange(c.DirectModifiers.GetAllStatChanges());
        
        //aura stat changes
        allStatChanges.AddRange(c.Auras.GetOverlappingAuras().SelectMany(i => i.Template.StatChanges));

        //Debug.Log(allStatChanges.Count + " total stat changes.");

        List<StatChange> changesForThisStat = allStatChanges.Where(change => change.ChangeTemplate.AffectedStat == stat).ToList();

        float calculatedValue = GameMath.CalculateStatWithChanges(GetBaseStatValue(stat), changesForThisStat);

        return StatController.GetLegalizedValue(stat, calculatedValue);
    }
    public List<StatController> CalculateCurrentStatValues()
    {
        List<StatController> statSet = new List<StatController>();
        foreach (StatController stat in _baseStats)
        {
            StatController newStat = new StatController(stat.Template);     //new, blank stat
            newStat.SetValue(CalculateCurrentStatValue(stat.Template));     //fill base value with current stat value

            statSet.Add(newStat);   //add it to the list
        }
        return statSet;
    }

    //flag functions
    public bool AddFlag(StatusFlag newFlag)
    {
        if (_flags.Contains(newFlag))
            return false;

        _flags.Add(newFlag);
        return true;
    }
    public bool RemoveFlag(StatusFlag newFlag)
    {
        //naturally returns false if none was found
        return _flags.Remove(newFlag);
    }
    public bool CheckFlag(StatusFlag newFlag)
    {
        return _flags.Contains(newFlag);
    }
}
