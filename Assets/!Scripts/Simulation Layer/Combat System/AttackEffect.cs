using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackEffectEnum
{
    DAMAGE_ENEMIES, HEAL_ALLIES
}
public delegate void AttackEffectDelegate(DamagePacket damagePacket, List<ICombatant> combatantsHit);
public sealed class AttackEffect
{
    //database class for all types of attack effects

    //internal list
    private static List<AttackEffect> _all = new List<AttackEffect>();

    public static bool TryGet(AttackEffectEnum enumValue, out AttackEffectDelegate foundEffectDelegate)
    {
        //get an effect from its enum value (for Unity editor access)

        AttackEffect foundEffect = _all.Find(effect => effect.EnumValue == enumValue);
        if (foundEffect != null)
        {
            foundEffectDelegate = foundEffect.DelegateValue;
            return true;
        }
        Debug.LogError("Unable to locate Attack effect with enum value: " + enumValue);
        foundEffectDelegate = null;
        return false;
    }

    //static instances (for script access)
    public static readonly AttackEffect DamageEnemies = new AttackEffect(AttackEffectEnum.DAMAGE_ENEMIES, DamageEnemies_Method);
    public static readonly AttackEffect HealAllies = new AttackEffect(AttackEffectEnum.HEAL_ALLIES, HealAllies_Method);

    //all the ways an attack can affect combatants (the actual methods called)
    public static void DamageEnemies_Method(DamagePacket damagePacket, List<ICombatant> combatantsHit)
    {
        IteratorForEachCombatant(combatantsHit,
                                 actor => actor.CompareTag("Enemy"),
                                 hit => CombatServices.TakeDamagePacket(hit, damagePacket));
    }
    public static void HealAllies_Method(DamagePacket damagePacket, List<ICombatant> combatantsHit)
    {
        IteratorForEachCombatant(combatantsHit,
                                 actor => actor.CompareTag("Ally"),
                                 hit => CombatServices.TakeDamagePacket(hit, damagePacket));
    }

    private static void IteratorForEachCombatant(List<ICombatant> all, System.Predicate<Actor> which, System.Action<ICombatant> what)
    {
        //do something to certain objects in a list which conform to a given predicate
        foreach (ICombatant hit in all)
        {
            Actor actorHit = hit as Actor;
            if (!which(actorHit))
                return;

            what(hit);
        }
    }

    //instance fields
    public readonly AttackEffectEnum EnumValue;
    public readonly AttackEffectDelegate DelegateValue;

    //instance ctor
    public AttackEffect(AttackEffectEnum enumValue, AttackEffectDelegate delegateValue)
    {
        EnumValue = enumValue;
        DelegateValue = delegateValue;

        _all.Add(this);     //add to list of all effects
    }
}
