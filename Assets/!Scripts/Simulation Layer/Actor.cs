using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour, IModdable, ICombatant
{
    public Transform ProjectileTransform;
    public Transform GetProjectileTransform()
    {
        return ProjectileTransform;
    }

    public AttackType CurrentAttackType;

    protected ModController _modController = new ModController();
    public ModController GetModController()
    {
        return _modController;
    }

    public CombatController _combatController;
    public CombatController GetCombatController()
    {
        return _combatController;
    }
    public StatController GetStatController()
    {
        return _combatController.GetStatController();
    }

    protected Rigidbody2D rb;

    protected void OnEnable()
    {
        _combatController = new CombatController(this);
        rb = GetComponent<Rigidbody2D>();
    }
    public Vector2 GetPosition()
    {
        return rb.position;
    }
    public void SetPosition(Vector2 position)
    {
        rb.MovePosition(position);
    }
    public void MoveTowards(Vector2 position, float maxDelta)
    {
        rb.MovePosition(Vector2.MoveTowards(GetPosition(), position, maxDelta));
    }
}
