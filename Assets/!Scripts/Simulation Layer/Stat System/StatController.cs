using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatController
{
    //instance of a StatTemplate

    public readonly StatTemplate Template;
    public float Value { get; protected set; }

    //ctor
    public StatController(StatTemplate template)
        : this(template, template.DefaultValue) { }

    public StatController(StatTemplate template, float value)
    {
        Template = template;
        SetValue(value);
    }

    //functions
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
        originalValue = Mathf.Round(originalValue / precision) * precision;

        //raise to minimum
        if (template.MinValue.Enabled)
            originalValue = Mathf.Max(originalValue, template.MinValue.Value);

        //lower to maximum
        if (template.MaxValue.Enabled)
            originalValue = Mathf.Min(originalValue, template.MaxValue.Value);

        return originalValue;
    }
}
