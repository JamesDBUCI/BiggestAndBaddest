using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatChangeTypeEnum
{
    PLUS, INCREASED, MORE, ADDITIONAL
    //minus, reduced, less, subtracted
}
public class StatChangeType
{
    private static Dictionary<StatChangeTypeEnum, StatChangeType> _all = new Dictionary<StatChangeTypeEnum, StatChangeType>()
    {
        { StatChangeTypeEnum.ADDITIONAL,
            new StatChangeType(Addition,
                "Additional", "Subtracted", "{0} {1}") },
        { StatChangeTypeEnum.INCREASED,
            new StatChangeType(Multiplication,
                "Increased", "Reduced", "{0}% {1}") },
        { StatChangeTypeEnum.MORE,
            new StatChangeType(Multiplication,
                "More", "Less", "{0}% {1}") },
        { StatChangeTypeEnum.PLUS,
            new StatChangeType(Addition,
                "+", "-", "{1}{0}") },
    };
    public static bool TryGet(StatChangeTypeEnum enumValue, out StatChangeType foundValue)
    {
        if (_all.TryGetValue(enumValue, out foundValue))
        {
            return true;
        }
        return false;
    }

    public readonly System.Func<float, float, float> ChangeFunc;
    public readonly string PositiveStyle;
    public readonly string NegativeStyle;
    public readonly string StyleFormat;

    private StatChangeType(System.Func<float, float, float> changeFunc, string positiveStyle, string negativeStyle, string styleFormat)
    {
        ChangeFunc = changeFunc;
        PositiveStyle = positiveStyle;
        NegativeStyle = negativeStyle;
        StyleFormat = styleFormat;
    }
    public string GetFormattedValueString(float changeValue)
    {
        //Debug.Log("Hello from class StatChangeType.");
        //FORMAT: {0} = value, {1} = pos or neg style word/char

        //if changeValue is less than 0, styleWord is negative. Otherwise, positive. (0 will have positive symbol or word)
        //remove sign (+/-) since the word will explain change direction
        string styleWord = changeValue < 0 ? NegativeStyle : PositiveStyle;

        return string.Format(StyleFormat, Mathf.Abs(changeValue), styleWord);
    }
    public string GetFormattedValueString(float minValue, float maxValue)
    {
        //for getting a range of possible values to describe templates
        return string.Format(StyleFormat,
            string.Format("({0}~{1})", minValue, maxValue),
            PositiveStyle);
    }

    private static float Addition(float a, float b)
    {
        return a + b;
    }
    private static float Multiplication(float a, float b)
    {
        return a * b;
    }
}
