using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class PlayerShooter : MonoBehaviour
{
    Actor _parentActor;

    private void OnEnable()
    {
        _parentActor = GetComponent<Actor>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _parentActor.CombatController.Skills.TryUse(0);
                
        }
        if (Input.GetMouseButtonDown(1))
        {
            _parentActor.CombatController.Skills.TryUse(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _parentActor.CombatController.Skills.TryUse(2);
        }
    }
}
