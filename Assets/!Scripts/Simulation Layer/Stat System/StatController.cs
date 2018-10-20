using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatController
{
    protected List<Stat> _stats;
    protected List<StatFlag> _flags = new List<StatFlag>();
    protected List<StatChange> _statChanges = new List<StatChange>();

    public StatController()
    {
        _stats = StatServices.GetInitializedStatList();
    }
    
    //stat functions
    protected Stat FindStat(StatTemplate template)
    {
        return _stats.Find(s => s.Template == template);
    }
    public float GetBaseStatValue(StatTemplate template)
    {
        return FindStat(template).Value;
    }
    public void ChangeBaseStat(StatTemplate template, float valueDelta)
    {
        FindStat(template).ChangeValue(valueDelta);
    }
    public void SetBaseStat(StatTemplate template, float newValue)
    {
        FindStat(template).SetValue(newValue);
    }

    public float CalculateCurrentStatValue(StatTemplate template)
    {
        List<StatChange> changesForThisStat = _statChanges.Where(change => change.StatInternalName == template.name).ToList();

        float calculatedValue = GameMath.CalculateStatWithChanges(GetBaseStatValue(template), changesForThisStat);

        return Stat.GetLegalizedValue(template, calculatedValue);
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

public interface IHaveStats
{
    StatController GetStatController();
}
