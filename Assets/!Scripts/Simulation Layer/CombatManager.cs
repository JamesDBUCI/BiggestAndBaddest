using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : IHaveStats
{
    Actor _parent;
    StatController _statController;

    private float _currentHP;

    public CombatManager(Actor parent)
    {
        _parent = parent;
        _statController = new StatController();

        StatTemplate healthStat;
        StatServices.StatTemplateDB.TryFind("vitality", out healthStat);
        _currentHP = _statController.CalculateCurrentStatValue(healthStat);
    }
    public bool IsDead()
    {
        return _currentHP == 0;
    }
    public float GetHP()
    {
        return _currentHP;
    }
    public void ChangeHP(float hpDelta)
    {
        _currentHP += hpDelta;
        if (_currentHP <= 0)
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

    public StatController GetStatController()
    {
        return _statController;
    }
}
