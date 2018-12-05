using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ZoneController : MonoBehaviour, IActorOwned
{
    //unity fields
    public Transform ZoneAuraTransform;
    public Transform ZoneAutoSkillTransform;

    //properties
    public Actor Origin { get; protected set; }
    public AuraManager AuraManager { get; protected set; }
    public AutoSkillManager AutoSkillManager { get; protected set; }

    //overload
    public void SetZone(Actor origin, Vector2 location, float lifespan, AuraInstance aura = null, AutoSkillInstance autoSkill = null)
    {
        SetZone(origin, location, lifespan, new List<AuraInstance> { aura }, new List<AutoSkillInstance> { autoSkill });
    }
    //core set zone method
    public void SetZone(Actor origin, Vector2 location, float lifespan, List<AuraInstance> auras = null, List<AutoSkillInstance> autoSkills = null)
    {
        Origin = origin;

        AuraManager = new AuraManager(ZoneAuraTransform);
        AutoSkillManager = new AutoSkillManager(Origin, ZoneAutoSkillTransform);

        if (auras != null)
            auras.ForEach(a => AuraManager.AddNew(a));

        if (autoSkills != null)
            autoSkills.ForEach(a => AutoSkillManager.AddNew(a));

        Destroy(gameObject, lifespan);  //destroy this object after lifespan (will still tick when game is paused. fix this. [lifespan managed by zone manager?])

        //add to battlefield zone manager?
    }
}