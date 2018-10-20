using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public sealed class StateManager : MonoBehaviour
{
    public Actor ParentActor { get; private set; }

    public State ActiveState;

    private bool aiActive = true;

    private void OnEnable()
    {
        ParentActor = GetComponent<Actor>();
    }

    public void SetAIActive(bool value)
    {
        aiActive = value;
    }

    public void StateTick()
    {
        if (!aiActive)
            return;

        //set the next state to be the result of sorting out the next state (if null, SetState() will keep current state)
        SetState(ActiveState.RunState(this));
    }

    public void SetState(State newState)
    {
        if (newState != null)
        {
            ActiveState = newState;
        }
    }
}
