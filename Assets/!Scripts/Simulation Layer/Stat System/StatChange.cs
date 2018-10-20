using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatChangeTemplate
{
    public string StatInternalName = "health";      //hopefully get the custom editor up and running to make this more reliable
    public StatChangeTypeEnum ChangeType = StatChangeTypeEnum.PLUS;
    public float MinValue = 1;
    public float MaxValue = 10;
    [Tooltip("Define how random rolls snap to closest values. 1 = integers only, 2 = even numbers, 0.5 = half-values only, 0.1 = tenths place precision, etc...\n0 = divide by zero error you silly fool")]
    public float Precision;
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
        StatInternalName = template.StatInternalName;
        ChangeType = template.ChangeType;
        Value = value;
    }
}
