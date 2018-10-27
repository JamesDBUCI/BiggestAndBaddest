using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    public float Speed;
    private SkillController _skillController;    //holds the method we'll call to enact the skill phase
    private List<Stat> _statSnapshot;   //holds the stats of the attacker at time of firing

    public void Arm(SkillController skillController, List<Stat> statSnapshot)
    {
        _skillController = skillController;
        _statSnapshot = statSnapshot;
    }
    private void Start()
    {
        Destroy(gameObject, 5f);        //make this nicer later
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

        _skillController.ApplyAttackEffects(SkillPhaseTimingEnum.ON_MAIN_HIT, new List<Actor>() { hitCombatant }, _statSnapshot);

        Destroy(gameObject);    //this one
    }
}
