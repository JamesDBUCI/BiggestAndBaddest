using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : IHaveStats
{
    Actor _parent;
    StatController _statController;

    public float LastAttackTimestamp { get; set; }

    public AttackType GetCurrentAttackType()
    {
        return _parent.CurrentAttackType;  //this replaced later by gear-based skills
    }

    private float _currentHP = 1000;

    public CombatController(Actor parent)
    {
        _parent = parent;
    }
    public void Init()
    {
        _statController = new StatController();

        //StatTemplate healthStat;
        //StatServices.StatTemplateDB.TryFind("vitality", out healthStat);

        //_currentHP = _statController.CalculateCurrentStatValue(healthStat);
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

        if (Mathf.Sign(hpDelta) == -1)
            Debug.Log(string.Format("{0} took {1} damage.", _parent.name, -hpDelta));
        else
            Debug.Log(string.Format("{0} was healed by {1}.", _parent.name, hpDelta));

        if (_currentHP <= 0)
        {
            Die();
        }
    }
    public void SetHP(float value)
    {
        _currentHP = value;
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
