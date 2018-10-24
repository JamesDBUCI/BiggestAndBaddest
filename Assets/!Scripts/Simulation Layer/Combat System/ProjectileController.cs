using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    public DamagePacket Damage;                 //damage should be calculated and set when fired
    public AttackType OriginalAttackType;       //process these effects at time of impact

    public float Speed;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }
    private void FixedUpdate()
    {
        transform.Translate(Vector3.up * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Actor hitCombatant = collision.gameObject.GetComponent<Actor>();
        if (hitCombatant == null || collision.gameObject.CompareTag("Player"))
            return;

        //Debug.Log("Hit Combatant");
        OriginalAttackType.ApplyAttackEffects(Damage, new List<ICombatant>() { hitCombatant });

        Destroy(gameObject);    //this one
    }
}
