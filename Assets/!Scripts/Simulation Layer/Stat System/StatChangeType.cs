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
    private const float STARTING_DELTA_ADDITIVE = 0;            //additive stat change types (plus, additional) modify stat values by 0 by default.
    private const float STARTING_DELTA_MULTIPLICATIVE = 100;    //multiplicative stat change types (increased, more) modify stat values by 100% by default.
    private const float MAGIC_MULTI_ADDITIVE = 1;               //additive stat change types are used as-written when doing game math.
    private const float MAGIC_MULTI_MULTIPLICATIVE = 0.01f;     //multiplicative stat change types have delta values multiplied by 0.01 before game math.

    private static Dictionary<StatChangeTypeEnum, StatChangeType> _all = new Dictionary<StatChangeTypeEnum, StatChangeType>()
    {
        { StatChangeTypeEnum.ADDITIONAL,
            new StatChangeType(Addition,
                "Additional", "Subtracted", "{0} {1} {2}",
                STARTING_DELTA_ADDITIVE,
                MAGIC_MULTI_ADDITIVE) },
        { StatChangeTypeEnum.INCREASED,
            new StatChangeType(Multiplication,
                "Increased", "Reduced", "{0}% {1} {2}",
                STARTING_DELTA_MULTIPLICATIVE,
                MAGIC_MULTI_MULTIPLICATIVE) },
        { StatChangeTypeEnum.MORE,
            new StatChangeType(Multiplication,
                "More", "Less", "{0}% {1} {2}",
                STARTING_DELTA_MULTIPLICATIVE,
                MAGIC_MULTI_MULTIPLICATIVE) },
        { StatChangeTypeEnum.PLUS,
            new StatChangeType(Addition,
                "+", "-", "{1}{0} {2}",
                STARTING_DELTA_ADDITIVE,
                MAGIC_MULTI_ADDITIVE) },
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
    public readonly float StartingDeltaValue;       //0 for additive, 1 for multiplicative
    public readonly float GameMathMagicMultiplier;  //multiply the dev-selected number times this when doing game math (.01 when dealing with percents)

    private StatChangeType(System.Func<float, float, float> changeFunc,
        string positiveStyle, string negativeStyle, string styleFormat, float startingDeltaValue, float gameMathMagicMultiplier)
    {
        ChangeFunc = changeFunc;
        PositiveStyle = positiveStyle;
        NegativeStyle = negativeStyle;
        StyleFormat = styleFormat;
        StartingDeltaValue = startingDeltaValue;
        GameMathMagicMultiplier = gameMathMagicMultiplier;
    }
    public string GetFormattedValueString(float changeValue, string statName)
    {
        //Debug.Log("Hello from class StatChangeType.");
        //FORMAT: {0} = value, {1} = pos or neg style word/char, {2} = stat external name

        //if changeValue is less than 0, styleWord is negative. Otherwise, positive. (0 will have positive symbol or word)
        //remove sign (+/-) since the word will explain change direction
        string styleWord = changeValue < 0 ? NegativeStyle : PositiveStyle;

        return string.Format(StyleFormat, Mathf.Abs(changeValue), styleWord, statName);
    }
    public string GetFormattedValueString(float minValue, float maxValue, string statName)
    {
        //for getting a range of possible values to describe templates
        return string.Format(StyleFormat,
            string.Format("({0}~{1})", minValue, maxValue),
            PositiveStyle, statName);
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
