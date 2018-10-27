using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AttackTypeEnum
{
    MELEE, PROJECTILE
}
public delegate void AttackTypeDelegate(Actor origin, SkillController skillController);
public sealed class AttackType
{
    private static List<AttackType> _all = new List<AttackType>();
    public static bool TryGet(AttackTypeEnum enumValue, out AttackType foundAttackType)
    {
        foundAttackType = _all.Find(at => at.EnumValue == enumValue);
        if (foundAttackType != null)
        {
            return true;
        }
        Debug.LogError("Unable to locate AttackType with enum value: " + enumValue);
        return false;
    }

    //static instances
    public static readonly AttackType Melee = new AttackType(AttackTypeEnum.MELEE, MeleeAttack_Method);
    public static readonly AttackType Projectile = new AttackType(AttackTypeEnum.PROJECTILE, ProjectileAttack_Method);

    //attack type delegate methods
    public static void MeleeAttack_Method(Actor origin, SkillController skillController)
    {
        MeleeSkill skill = skillController.Skill as MeleeSkill;

        //draw a raycast with provided max range and return the enemies hit.

        //handles
        Transform projTrans = origin.AimTransform;

        //get all objects hit by the raycast (it's usually a small ray, so we'll have 0~1 hits much of the time)
        RaycastHit2D[] hits = Physics2D.RaycastAll(projTrans.transform.position, projTrans.up, skill.MeleeRange);

        //debug ray
        Debug.DrawRay(projTrans.transform.position, projTrans.up * skill.MeleeRange, Color.green, skill.SkillLockInfo.SkillLockDuration);

        //if there were no hits for some reason, just leave right here;
        if (hits == null)
            return;

        //make a list of the objects hit
        List<RaycastHit2D> hitsList = new List<RaycastHit2D>(hits);

        //make an empty list for the ICombatants (Actors) we're going to hit
        List<Actor> combatantsHit = new List<Actor>();

        //iterate the list of raycast hits
        foreach (RaycastHit2D hit in hitsList)
        {
            //try to get an Actor component off the object hit
            Actor actorComponent = hit.transform.gameObject.GetComponent<Actor>();

            //if there wasn't one, it wasn't an ICombatant, so we're not able to do damage to it (next hit object, please)
            if (actorComponent == null)
                continue;

            //stop hitting yourself
            if (actorComponent == origin as Actor)
                continue;

            //if we got here, it was an actor and we're gonna say we hit it (enemies or allies)
            combatantsHit.Add(actorComponent);

            //if we've maxed out the number of targets this attack can hit, we stop checking hits
            int maxTargetsActual = 1;       //change if we're going to dynamically add more targets to skills
            if (combatantsHit.Count == maxTargetsActual)
                break;
        }

        //we hit them
        skillController.ApplyAttackEffects(SkillPhaseTimingEnum.ON_MAIN_HIT, combatantsHit);
    }
    public static void ProjectileAttack_Method(Actor origin, SkillController skillController)
    {
        ProjectileSkill skill = skillController.Skill as ProjectileSkill;

        if (skill.ProjectilePrefab == null)
            return;

        Transform projTrans = origin.AimTransform;
        ProjectileController pc = Object.Instantiate(skill.ProjectilePrefab.gameObject, projTrans.position, projTrans.rotation).GetComponent<ProjectileController>();
        pc.Arm(skillController, origin.CombatController.Stats.CalculateCurrentStatValues());    //inject current stat values

        //we don't hit them yet. we wait.
    }  

    //instance fields
    public AttackTypeEnum EnumValue;
    public AttackTypeDelegate DelegateValue;

    public AttackType(AttackTypeEnum enumValue, AttackTypeDelegate delegateValue)
    {
        EnumValue = enumValue;
        DelegateValue = delegateValue;

        _all.Add(this);
    }
}