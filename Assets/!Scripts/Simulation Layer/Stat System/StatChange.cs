using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatChangeTemplate
{
    //public string StatInternalName = "health";
    public StatTemplate affectedStat;       //draggy-drop in Unity
    public StatChangeTypeEnum ChangeType = StatChangeTypeEnum.PLUS;
    public float MinValue = 1;
    public float MaxValue = 10;
    [Tooltip("Define how random rolls snap to closest values. 1 = integers only, 2 = even numbers, 0.5 = half-values only, 0.1 = tenths place precision, etc...\n0 = divide by zero error you silly fool")]
    public float Precision = 1;
}

public class StatChange
{
    //generated from template, not modified in Unity

    public readonly string StatInternalName;
    public readonly StatChangeTypeEnum ChangeType;
    public readonly float Value;

    public StatChange(string statInternalName, StatChangeTypeEnum enumValue, float value)
    {
        StatInternalName = statInternalName;
        ChangeType = enumValue;
        Value = value;
    }
    public StatChange(StatChangeTemplate template, float value)
    {
        if (template == null)
            StatInternalName = "Undefined Stat";
        else
            StatInternalName = template.affectedStat.name;

        ChangeType = template.ChangeType;
        Value = value;
    }
}
