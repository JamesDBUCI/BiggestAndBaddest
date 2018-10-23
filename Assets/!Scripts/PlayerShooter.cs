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
        //rotate projectile Transform
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _parentActor.GetProjectileTransform().rotation = Quaternion.LookRotation(worldMousePos - transform.position, Vector3.forward);

        //replaced later with the many input commands that can happen

        if (Input.GetButton("Fire1"))
        {
            _parentActor.GetCombatController().GetCurrentAttackType().TryAttack(_parentActor);
        }
	}
}
