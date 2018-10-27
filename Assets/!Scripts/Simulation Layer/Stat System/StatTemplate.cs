using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat")]
public class StatTemplate : ScriptableObject
{
    //data for a combat stat
    public string ExternalName;
    public string Description;

    public float DefaultValue;
    public OptionalFloat MinValue = new OptionalFloat(0);     //false = no min value
    public OptionalFloat MaxValue = new OptionalFloat(1, false);     //false = no max value
    public OptionalFloat Precision = new OptionalFloat(1, false);    //false = integer values only

    public Color Color;     //there can be more of these
    public Sprite Icon;     //also of these
}
