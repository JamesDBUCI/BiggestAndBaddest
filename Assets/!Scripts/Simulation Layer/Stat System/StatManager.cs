using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager
{
    protected List<StatController> _baseStats = new List<StatController>();
    protected List<StatFlag> _flags = new List<StatFlag>();
    protected List<StatChange> _statChanges = new List<StatChange>();

    //ctor
    public StatManager()
    {
        //write a default copy of every stat in the database
        GameDatabase.Stats.GetAllAssets().ForEach(st => _baseStats.Add(new StatController(st)));
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
    public float CalculateCurrentStatValue(StatTemplate template)
    {
        List<StatChange> changesForThisStat = _statChanges.Where(change => change.StatInternalName == template.name).ToList();

        float calculatedValue = GameMath.CalculateStatWithChanges(GetBaseStatValue(template), changesForThisStat);

        return StatController.GetLegalizedValue(template, calculatedValue);
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
    public bool AddFlag(StatFlag newFlag)
    {
        if (_flags.Contains(newFlag))
            return false;

        _flags.Add(newFlag);
        return true;
    }
    public bool RemoveFlag(StatFlag newFlag)
    {
        //naturally returns false if none was found
        return _flags.Remove(newFlag);
    }
    public bool CheckFlag(StatFlag newFlag)
    {
        return _flags.Contains(newFlag);
    }

    //stat change functions
    public void AddStatChange(StatChange newChange)
    {
        _statChanges.Add(newChange);
    }
    public void AddStatChanges(List<StatChange> newChanges)
    {
        _statChanges.AddRange(newChanges);
    }
    public bool RemoveStatChange(StatChange change)
    {
        return _statChanges.Remove(change);
    }
    public bool RemoveStatChanges(List<StatChange> changes)
    {
        bool allWereRemoved = true;
        foreach (StatChange change in changes)
        {
            allWereRemoved = allWereRemoved && _statChanges.Remove(change);
        }
        return allWereRemoved;
    }
    protected bool HasStatChange(StatChange change)
    {
        return _statChanges.Contains(change);
    }
    public int CountStatChanges()
    {
        return _statChanges.Count;
    }
}
