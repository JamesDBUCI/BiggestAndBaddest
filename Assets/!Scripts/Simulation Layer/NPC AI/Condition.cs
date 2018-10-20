using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    public abstract bool Evaluate(StateManager stateManager);
    //duration?
}

[CreateAssetMenu(menuName = "AI/Conditions/HP Value")]
public class Cond_HPValue : Condition
{
    private delegate bool ComparisonDelegate(float sideA, float sideB);
    public enum Comparator
    {
        LESS_THAN, LESS_THAN_OR_EQUAL_TO, EQUAL_TO, GREATER_THAN_OR_EQUAL_TO, GREATER_THAN, DIFFERENT_FROM
    }

    public Comparator CompareType;
    public float HPValue;

    public override bool Evaluate(StateManager stateManager)
    {
        ComparisonDelegate del;
        if (CompareType == Comparator.LESS_THAN)
            del = (f, g) => f < g;
        else if(CompareType == Comparator.LESS_THAN_OR_EQUAL_TO)
            del = (f, g) => f <= g;
        else if(CompareType == Comparator.EQUAL_TO)
            del = (f, g) => f == g;
        else if(CompareType == Comparator.GREATER_THAN_OR_EQUAL_TO)
            del = (f, g) => f >= g;
        else if(CompareType == Comparator.GREATER_THAN)
            del = (f, g) => f > g;
        else
            del = (f, g) => f != g;     //CompareType == Comparator.DIFFERENT_FROM

        return del(stateManager.ParentActor.CombatManager.GetHP(), HPValue);
    }
}

