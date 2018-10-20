using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChoiceWeight
{
    SOMETIMES, ALWAYS
}

[System.Serializable]
public class Choice
{
    public List<Condition> TrueConditions = new List<Condition>();        //conditions that must all evaluate to true
    public List<Condition> FalseConditions = new List<Condition>();       //conditions that must all evaluate to false
    public ChoiceWeight ChoiceWeight = ChoiceWeight.SOMETIMES;

    public State NewState;          //new state if all conditions apply
}
