using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Actor : MonoBehaviour, IModdable, IHaveStats
{
    protected ModController _modController = new ModController();
    public ModController GetModController()
    {
        return _modController;
    }

    public CombatManager CombatManager;
    public StatController GetStatController()
    {
        return CombatManager.GetStatController();
    }

    protected Rigidbody2D rb;

    protected void OnEnable()
    {
        CombatManager = new CombatManager(this);
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
