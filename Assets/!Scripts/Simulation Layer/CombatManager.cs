using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager
{
    Actor _parent;

    private StatSet _statSet = new StatSet();

    public CombatManager(Actor parent)
    {
        _parent = parent;
    }
    public bool IsDead()
    {
        return _statSet.GetValue("CurrentHP") == 0;
    }
    public float GetHP()
    {
        return _statSet.GetValue("CurrentHP");
    }
    public void ChangeHP(float hpChange)
    {
        _statSet.ChangeValue("CurrentHP", hpChange);
        if (_statSet.GetValue("CurrentHP") == 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Debug.Log("I died. ~" + _parent.gameObject.name);
        GameObject.Destroy(_parent.gameObject);
        //_parent.GetComponent<Rigidbody2D>().gravityScale = 1;
    }
}
