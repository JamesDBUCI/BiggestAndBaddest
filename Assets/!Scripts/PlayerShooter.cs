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
            //returns true if the attack was made, false if it wasn't (due to cooldown)
            //returns a List of combatants hit as an out parameter

            AttackType currentAttack = _parentActor.GetCombatController().GetCurrentAttackType();

            List<ICombatant> combatantsHit;
            if (currentAttack.TryAttack(_parentActor, out combatantsHit))
            {
                currentAttack.ApplyAttackEffects(_parentActor, combatantsHit);
            }
        }
	}
}
