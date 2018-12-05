using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoSkillController : MonoBehaviour, Skill.ISkillSource
{
    public Transform AimTransform;
    public AimManager AimManager { get; private set; }

    public Actor Origin { get; private set; }
    public AutoSkillInstance Instance { get; private set; }
    public Transform SourceTransform { get { return transform; } }
    public float Duration { get; private set; }

    public Actor CurrentTarget { get; protected set; }

    private void OnEnable()
    {
        AimManager = new AimManager(AimTransform);
    }

    public void SetAutoSkill(Actor origin, AutoSkillInstance autoSkill)
    {
        Origin = origin;
        Instance = autoSkill;
        Duration = autoSkill.TotalDuration;

        StartCoroutine(SkillTickCoroutine());

        Destroy(gameObject, Duration);
    }
    public void TargetAndAim()     //re-target or don't
    {
        //first
        if (CurrentTarget == null)
            CurrentTarget = GetClosestTarget();

        //then
        if (CurrentTarget != null)
        {
            AimManager.AimAtPoint(CurrentTarget.GetSpritePosition());

            //debug line
            Vector2 loc = AimManager.GetLocation();
            Debug.DrawLine(loc, loc + AimManager.GetAimDirection(), Color.red, Time.deltaTime);
        }
    }
    protected Actor GetClosestTarget()
    {
        var hits = Physics2D.OverlapCircleAll(AimManager.GetLocation(), Instance.MaxDetectRange);
        if (hits != null && hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                Actor hitActor = hit.GetComponent<Actor>();
                if (hitActor == null)
                    continue;

                //if hit actor is not a valid target of any effect in this skill, continue
                if (!Skill.TargetHelper.IsValidTarget(Instance.Template.Phase.Effects.SelectMany(info => info.AffectedActors).ToList(), Origin, hitActor))
                    continue;

                return hitActor;
            }
        }
        return null;
    }
    protected IEnumerator SkillTickCoroutine()
    {
        while (true)
        {
            for (float i = 0; i < Instance.TickDuration;)
            {
                if (!Game.Self.IsPaused)
                    i += Time.deltaTime;

                TargetAndAim();
                yield return null;
            }
            Use();
        }
    }
    public void Use()
    {
        Instance.Template.Phase.Action.Perform(Origin, this, Instance.Template.Phase);
    }
}
