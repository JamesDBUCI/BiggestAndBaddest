using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackEffectEnum
{
    DAMAGE_ENEMIES, HEAL_ALLIES, DODGE_FORWARD, DODGE_BACKWARD, KNOCK_FORWARD, KNOCK_BACK
}
public delegate void AttackEffectDelegate(DamagePacket damagePacket, Actor origin, List<Actor> combatantsHit);
public sealed class AttackEffect
{
    //database class for all types of attack effects

    //internal list
    private static List<AttackEffect> _all = new List<AttackEffect>();

    public static bool TryGetDelegate(AttackEffectEnum enumValue, out AttackEffectDelegate foundEffectDelegate)
    {
        AttackEffect foundEffect;
        if (TryGetInstance(enumValue, out foundEffect))
        {
            foundEffectDelegate = foundEffect.DelegateValue;
            return true;
        }
        foundEffectDelegate = null;
        return false;
    }
    public static bool TryGetPotencyRequirement(AttackEffectEnum enumValue, out bool potencyIsRequired)
    {
        potencyIsRequired = false;
        AttackEffect foundEffect;
        if (!TryGetInstance(enumValue, out foundEffect))
            return false;
        potencyIsRequired = foundEffect.RequiresPotency;
        return true;
    }

    private static bool TryGetInstance(AttackEffectEnum enumValue, out AttackEffect foundInstance)
    {
        foundInstance = _all.Find(effect => effect.EnumValue == enumValue);
        if (foundInstance == null)
        {
            Debug.LogError("Unable to locate Attack effect with enum value: " + enumValue);
            return false;
        }
        return true;
    }

    //static instances (for script access)
    public static readonly AttackEffect DamageEnemies = new AttackEffect(AttackEffectEnum.DAMAGE_ENEMIES, DamageEnemies_Method, true);
    public static readonly AttackEffect HealAllies = new AttackEffect(AttackEffectEnum.HEAL_ALLIES, HealAllies_Method, true);
    public static readonly AttackEffect DodgeForward = new AttackEffect(AttackEffectEnum.DODGE_FORWARD, DodgeForward_Method, false);
    public static readonly AttackEffect DodgeBackward = new AttackEffect(AttackEffectEnum.DODGE_BACKWARD, DodgeBackward_Method, false);
    public static readonly AttackEffect KnockForward = new AttackEffect(AttackEffectEnum.KNOCK_FORWARD, KnockForward_Method, false);
    public static readonly AttackEffect KnockBackward = new AttackEffect(AttackEffectEnum.KNOCK_BACK, KnockBackward_Method, false);

    //all the ways an attack can affect combatants (the actual methods called)
    public static void DamageEnemies_Method(DamagePacket damagePacket, Actor origin, List<Actor> combatantsHit)
    {
        IteratorForEachCombatant(origin, combatantsHit,
                                 actor => actor.CompareTag("Enemy"),
                                 hit => CombatServices.TakeDamagePacket(hit, damagePacket));
    }
    public static void HealAllies_Method(DamagePacket damagePacket, Actor origin, List<Actor> combatantsHit)
    {
        IteratorForEachCombatant(origin, combatantsHit,
                                 actor => actor.CompareTag("Ally"),
                                 hit => CombatServices.TakeDamagePacket(hit, damagePacket));
    }
    public static void DodgeForward_Method(DamagePacket damagePacket, Actor origin, List<Actor> combatantsHit)
    {
        MoveInDirection(origin, origin.AimTransform.up, false);
    }
    public static void DodgeBackward_Method(DamagePacket damagePacket, Actor origin, List<Actor> combatantsHit)
    {
        MoveInDirection(origin, -origin.AimTransform.up, false);
    }
    public static void KnockForward_Method(DamagePacket damagePacket, Actor origin, List<Actor> combatantsHit)
    {
        MoveInDirection(origin, origin.AimTransform.up, true);
    }
    public static void KnockBackward_Method(DamagePacket damagePacket, Actor origin, List<Actor> combatantsHit)
    {
        MoveInDirection(origin, -origin.AimTransform.up, true);
    }

    //helper methods
    private static void IteratorForEachCombatant(Actor origin, List<Actor> all, System.Predicate<Actor> which, System.Action<Actor> what)
    {
        //do something to certain objects in a list which conform to a given predicate
        foreach (Actor hit in all)
        {
            if (hit == null)        //we actually can this deep with a list containing null entries
                continue;

            Actor actorHit = hit as Actor;
            if (!which(actorHit))
                return;

            what(hit);
        }
    }
    private static void MoveInDirection(Actor actorToMove, Vector2 direction, bool anInvoluntaryMovement)
    {
        float dodgeForce = Const.DODGE_FORCE_VALUE;        //remember to change me if dynamic dodge distance is a thing;
        actorToMove.AddMovementForce(direction, dodgeForce, anInvoluntaryMovement);
    }

    //instance fields
    public readonly AttackEffectEnum EnumValue;
    public readonly AttackEffectDelegate DelegateValue;
    public readonly bool RequiresPotency;

    //instance ctor
    public AttackEffect(AttackEffectEnum enumValue, AttackEffectDelegate delegateValue, bool requiresPotency)
    {
        EnumValue = enumValue;
        DelegateValue = delegateValue;
        RequiresPotency = requiresPotency;

        _all.Add(this);     //add to list of all effects
    }
}
