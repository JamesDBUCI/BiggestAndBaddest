using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameMath
{
    public static float CalculateStatWithChanges(float originalValue, List<StatChange> changes)
    {
        if (changes.Count == 0)
            return originalValue;

        //order is very important here

        //start with base value
        float finalValue = originalValue;
        //Debug.Log("value is now " + finalValue);

        //factor in plus/minus changes
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.PLUS);
        //Debug.Log("value is now " + finalValue);

        //factor in increase/decrease changes
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.INCREASED);
        //Debug.Log("value is now " + finalValue);

        //factor in more/less changes
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.MORE);
        //Debug.Log("value is now " + finalValue);

        //factor in added/subtracted changes
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.ADDITIONAL);
        //Debug.Log("value is now " + finalValue);

        return finalValue;
    }
    public static float ApplyStatChangesOfType(float originalValue, List<StatChange> changes, StatChangeTypeEnum changeType)
    {
        StatChangeType foundType;
        if (!StatChangeType.TryGet(changeType, out foundType))
            return originalValue;

        float totalDelta = foundType.StartingDeltaValue;

        List<StatChange> changesOfType = changes.Where(change => change.ChangeType == changeType).ToList();

        foreach (StatChange change in changesOfType)
        {
            totalDelta += change.Value;
        }

        //don't forget your game math magic multiplier! (a 75% increase is written as 75 in-editor, but used value is actually 0.75)
        return foundType.ChangeFunc(originalValue, totalDelta * foundType.GameMathMagicMultiplier);
    }
    public static float CalculateOutgoingDamage(ICombatant combatant, List<StatScaler> statScaling)
    {
        float outgoingDamage = 0;
        foreach (StatScaler scaledStat in statScaling)
        {
            float currentStatValue = combatant.GetCombatController().
                GetStatController().CalculateCurrentStatValue(scaledStat.Stat);

            outgoingDamage += currentStatValue * scaledStat.Scale;
        }
        return outgoingDamage;
    }
    public static float CalculateDamageAfterResistance(float incomingDamage, float resistanceStatValue)
    {
        //assuming 75 resist is 75% resistance
        //negative resistance will add more damage
        //more than max resistance will be capped at max
        return incomingDamage * (1 - Mathf.Min(resistanceStatValue, StatServices.MAX_RESISTANCE_STAT_VALUE)/ 100);
    }
}
