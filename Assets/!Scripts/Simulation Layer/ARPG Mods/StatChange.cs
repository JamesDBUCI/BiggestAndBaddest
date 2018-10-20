using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatChangeTemplate
{
    public string StatName = "Health";
    public StatChangeTypeEnum ChangeType = StatChangeTypeEnum.PLUS;
    public float MinValue = 1;
    public float MaxValue = 10;
    [Tooltip("Define how randoms rolls snap to closest values. 1 = integers only, 2 = even numbers, 0.5 = half-values only, 0.1 = tenths place precision, etc...\n0 = divide by zero error you silly fool")]
    public float Precision;
}

public class StatChange
{
    //generated from template, not modified in Unity

    public string StatName;
    public StatChangeTypeEnum ChangeType;
    public float Value;

    public StatChange(StatChangeTemplate template, float value)
    {
        StatName = template.StatName;
        ChangeType = template.ChangeType;
        Value = value;
    }
}
