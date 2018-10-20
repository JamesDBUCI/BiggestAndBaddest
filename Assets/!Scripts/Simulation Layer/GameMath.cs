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
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.PLUS, 0);
        //Debug.Log("value is now " + finalValue);

        //factor in increase/decrease changes
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.INCREASED, 1);
        //Debug.Log("value is now " + finalValue);

        //factor in more/less changes
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.MORE, 1);
        //Debug.Log("value is now " + finalValue);

        //factor in added/subtracted changes
        finalValue = ApplyStatChangesOfType(finalValue, changes, StatChangeTypeEnum.ADDITIONAL, 0);
        //Debug.Log("value is now " + finalValue);

        return finalValue;
    }
    public static float ApplyStatChangesOfType(float originalValue, List<StatChange> changes, StatChangeTypeEnum changeType, float defaultDelta)
    {
        StatChangeType foundType;
        if (!StatChangeType.TryGet(changeType, out foundType))
            return originalValue;

        float totalDelta = defaultDelta;
        List<StatChange> changesOfType = changes.Where(change => change.ChangeType == changeType).ToList();
        foreach (StatChange change in changesOfType)
        {
            totalDelta += change.Value;
        }

        return foundType.ChangeFunc(originalValue, totalDelta);
    }
}
