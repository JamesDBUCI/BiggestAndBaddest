using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatSet
{
    class Stat
    {
        public readonly string Name;
        public float Value;
        public Stat(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }

    private List<Stat> _allStats = new List<Stat>()
    {
        new Stat("CurrentHP", 100),
        new Stat("MaxHealth", 100),
        new Stat("Mana", 100),
        new Stat("Strength", 10),
        new Stat("Intelligence", 10),
    };

    private Stat GetStat(string statName)
    {
        return _allStats.Find(s => s.Name == statName);
    }

    public float GetValue(string statName)
    {
        return GetStat(statName).Value;
    }
    public void SetValue(string statName, float value)
    {
        GetStat(statName).Value = Mathf.Max(value, 0);
    }
    public void ChangeValue(string statName, float delta)
    {
        SetValue(statName, GetValue(statName) + delta);
    }
}
