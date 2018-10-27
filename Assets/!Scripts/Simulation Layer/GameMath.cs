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
    public static float CalculateScaledStatTotal(CombatController combatController, List<StatScaler> statScaling)
    {
        float totalScaledValue = 0;
        foreach (StatScaler scaledStat in statScaling)
        {
            float currentStatValue = combatController.Stats.CalculateCurrentStatValue(scaledStat.Stat);

            totalScaledValue += currentStatValue * scaledStat.Scale;
        }
        return totalScaledValue;
    }
    public static float CalculateAdjustedDamage(float incomingDamage, float coreResistance, float secondaryResistanceAverage)
    {
        float outgoingDamage = incomingDamage;
        outgoingDamage = CalculateAdjustedDamage(outgoingDamage, coreResistance);
        outgoingDamage = CalculateAdjustedDamage(outgoingDamage, secondaryResistanceAverage);
        return outgoingDamage;
    }
    private static float CalculateAdjustedDamage(float incomingDamage, float resistanceStatValue)
    {
        //assuming 75 resist is 75% resistance
        //negative resistance will add more damage
        //more than max resistance will be capped at max
        return incomingDamage * (1 - Mathf.Min(resistanceStatValue, Const.MAX_RESISTANCE_STAT_VALUE)/ 100);
    }
    public static float CalculateMaxHP(float maxHPScaleStatValue)
    {
        return maxHPScaleStatValue * 10;
    }
    public static float CalculateAdjustedCCDuration(float ccDuration, float reductionStat)
    {
        return (1 - (reductionStat / 1000)) * ccDuration;
    }
    public static float CalculateAdjustedChannelDuration(float channelDuration, float reductionStat)
    {
        return (1 - (reductionStat / 1000)) * channelDuration;
    }
    public static float CalculateAdjustedSkillLockDuration(float skillLockDuration, float reductionStat)
    {
        return (1 - (reductionStat / 1000)) * skillLockDuration;
    }
    public static float CalculateAdjustedCooldownDuration(float cooldownDuration, float reductionStat)       //blank for now
    {
        return cooldownDuration;
    }
    public static float CalculateAdjustedMovementSpeed(float channelDuration, float boostStat)
    {
        return (1 + (boostStat / 1000)) * channelDuration;
    }
}
