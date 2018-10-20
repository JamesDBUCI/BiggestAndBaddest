using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class State : ScriptableObject
{
    public List<Action> Actions = new List<Action>();

    public List<Choice> Choices = new List<Choice>();

    public State RunState(StateManager stateManager)
    {
        foreach (var action in Actions)
        {
            action.Execute(stateManager);
        }

        List<Choice> validChoices = new List<Choice>();
        foreach (var choice in Choices)
        {
            bool valid = true;
            foreach (var tc in choice.TrueConditions)
            {
                if (!tc.Evaluate(stateManager))
                {
                    valid = false;
                    break;
                }
            }

            if (!valid)
                continue;

            foreach (var fc in choice.FalseConditions)
            {
                if (fc.Evaluate(stateManager))
                {
                    break;
                }
            }

            if (!valid)
                continue;

            validChoices.Add(choice);
        }

        if (validChoices.Count == 0)
            return null;

        State alwaysChoose = validChoices.Find(st => st.ChoiceWeight == ChoiceWeight.ALWAYS).NewState;
        if (alwaysChoose != null)
            return alwaysChoose;

        return validChoices[Random.Range(0, validChoices.Count)].NewState;
    }
}
