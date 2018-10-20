using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stat System/New Stat Template")]
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

public class Stat
{
    //instance of a StatTemplate
    public readonly StatTemplate Template;
    public float Value { get; protected set; }

    public Stat(StatTemplate template)
    {
        Template = template;
    }
    public void ChangeValue(float valueDelta)
    {
        Value += valueDelta;
        LegalizeValue();
    }
    public void SetValue(float newValue)
    {
        Value = newValue;
        LegalizeValue();
    }
    public void LegalizeValue()
    {
        Value = GetLegalizedValue(Template, Value);
    }
    public static float GetLegalizedValue(StatTemplate template, float originalValue)
    {
        //make the value conform to templates min, max, and precision specifications

        //if precision is not provided, make the value integral
        float precision = template.Precision.Enabled ? template.Precision.Value : 1;

        //round to nearest precision level
        originalValue = Mathf.Round(originalValue / template.Precision.Value) * template.Precision.Value;

        //raise to minimum
        if (template.MinValue.Enabled)
            originalValue = Mathf.Max(originalValue, template.MinValue.Value);

        //lower to maximum
        if (template.MaxValue.Enabled)
            originalValue = Mathf.Min(originalValue, template.MaxValue.Value);

        return originalValue;
    }
}
