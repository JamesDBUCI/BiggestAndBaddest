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
        if (Input.GetButton("Fire1"))
        {
            _parentActor.CombatController.Skills.TryUse(0);
        }
        if (Input.GetButton("Fire2"))
        {
            _parentActor.CombatController.Skills.TryUse(1);
        }
    }
}
