using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    public DamagePacket Damage;

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

        //Debug.Log("Hit Enemy");
        CombatServices.TakeDamagePacket(hitCombatant, Damage);

        Destroy(gameObject);    //this one
    }
}
