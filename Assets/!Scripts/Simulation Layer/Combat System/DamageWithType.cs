using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DamageWithType
{
    public float Value;
    public List<DamageType> Types;
    public DamageWithType(float value, params DamageType[] types)
    {
        Value = value;
        Types = new List<DamageType>(types);
    }
}
