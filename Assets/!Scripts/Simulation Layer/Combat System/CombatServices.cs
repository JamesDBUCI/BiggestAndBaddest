using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoreDamageTypeEnum
{
    PHYSICAL, MAGIC, HYBRID, ADAPTIVE_HIGH, ADAPTIVE_LOW, TRUE
}
public enum ActorFaction
{
    ALLY, ENEMY, NEUTRAL
}
public static class CombatServices
{
    private static float GetStatValueFromSet(StatTemplate stat, List<StatController> set)
    {
        StatController foundStat = set.Find(setStat => setStat.Template == stat);
        if (foundStat == null)
            return 0;

        return foundStat.Value;
    }
    public static HitNugget ConstructHitNugget(Actor origin, Skill.EffectInfo effect)
    {
        return ConstructHitNugget(origin, effect, origin.CombatController.Stats.CalculateCurrentStatValues());
    }
    public static HitNugget ConstructHitNugget(Actor origin, Skill.EffectInfo effect, List<StatController> stats)
    {
        if (effect.Potency == null)
            return new HitNugget(origin, null);   //blank damage

        //get current stats
        var currentStats = origin.CombatController.Stats.CalculateCurrentStatValues();

        //empty list
        List<DamageWithType> finalDamageInHit = new List<DamageWithType>();

        float totalScaleStatValue = 0;

        //iterate stat scaling
        foreach (StatScaler statScaler in effect.Scaling)
        {
            totalScaleStatValue += GetStatValueFromSet(statScaler.Stat, currentStats) * statScaler.Scale;
        }

        //iterate potencies
        foreach (DamageWithType potency in effect.Potency)
        {
            finalDamageInHit.Add(new DamageWithType(potency.CoreDamageType, totalScaleStatValue * (potency.Value / 100), potency.SecondaryTypes.ToArray()));
        }

        return new HitNugget(origin, finalDamageInHit.ToArray());
    }
    public static void TakeDamageOrHeal(Actor target, HitNugget hitNugget, float scale = 1)
    {
        TakeDamageOrHeal(target, new List<DamageWithType>(hitNugget.Damage), scale);
    }
    public static void TakeDamageOrHeal(Actor target, List<DamageWithType> damageOrHealing, float scale = 1)
    {
        //core method for healing or dealing damage to a target
        //damage scale defaults to 1, but should be modified for reciprocal damage or healing (recoil, etc...)

        //grab a handle
        StatManager sc = target.CombatController.Stats;

        //iterate all type nodes of incoming damage
        float totalPotencyAfterMitigation = 0;
        foreach (DamageWithType dmg in damageOrHealing)
        {
            //figure out sign of dmg (damage is positive, healing is negative)
            bool dmgIsHealing = dmg.Value < 0;

            //resistance to the core damage type of the hit
            float coreResistValue = dmgIsHealing ? 0 : GetCoreResistValue(target, dmg.CoreDamageType);

            //list of resistances for each secondary damage type in the node
            List<float> secondaryResistances = new List<float>();

            if (!dmgIsHealing)
            {
                //iterate all secondary types within the node
                foreach (DamageType dt in dmg.SecondaryTypes)
                {
                    if (dt == null)     //if there is an empty damage type slot
                        continue;

                    if (dt.ResistanceStat.Stat == null)     //if there is an empty resistance stat type slot
                        continue;

                    //if the damage type can be resisted

                    float subtypeResistance = 0;

                    //get resist stat scaled value (cap it at max, since we're calculating as resistance)

                    subtypeResistance = Mathf.Min(Const.MAX_RESISTANCE_STAT_VALUE,
                                                    sc.CalculateCurrentStatValue(dt.ResistanceStat.Stat) * dt.ResistanceStat.Scale);

                    //Debug.Log("Resistance Stat found and value applied: " + subtypeResistance);

                    /*
                        * We could make unorthodox stats like Critical Hit Rate or move speed into resist stats for something,
                        * so we have to cap the stat total here and only here.
                    */

                    //add it to the list
                    secondaryResistances.Add(subtypeResistance);
                }
            }

            //get average of resists in case of multi-type damage node
            float averageOfResists = 0;

            //there is an outlying chance that the only damage type in a node is not resistable, leading to a division by 0.
            //so we make a conditional
            if (secondaryResistances.Count != 0)
            {
                foreach (float subtypeRes in secondaryResistances)
                {
                    averageOfResists += subtypeRes;
                }
                averageOfResists /= secondaryResistances.Count;   //NOTE: non-resistable damage types (no resist stat provided) will not affect average resistance
            }

            float nodePotency = GameMath.CalculateAdjustedDamage(dmg.Value, coreResistValue, averageOfResists);
            totalPotencyAfterMitigation += nodePotency;     //factor in scale here
        }

        //TAKE THAT HIT
        target.CombatController.ChangeHP(Mathf.RoundToInt(-totalPotencyAfterMitigation));   //inverse of potency because positive value is damage
    }

    public static float GetCoreResistValue(Actor combatant, CoreDamageTypeEnum coreDamageType)
    {
        float totalResistance = 0;
        if (coreDamageType == CoreDamageTypeEnum.PHYSICAL)
        {
            GetCoreResistValue_Basic(combatant, Const.PHYSICAL_RESIST_STAT, out totalResistance);
        }
        if (coreDamageType == CoreDamageTypeEnum.MAGIC)
        {
            GetCoreResistValue_Basic(combatant, Const.MAGIC_RESIST_STAT, out totalResistance);
        }
        if (coreDamageType == CoreDamageTypeEnum.HYBRID)
        {
            totalResistance = GetCoreResistValue_Compound(combatant, (f, g) => f + g);
        }
        if (coreDamageType == CoreDamageTypeEnum.ADAPTIVE_HIGH)
        {
            totalResistance = GetCoreResistValue_Compound(combatant, (f, g) => Mathf.Max(f, g));
        }
        if (coreDamageType == CoreDamageTypeEnum.ADAPTIVE_LOW)
        {
            totalResistance = GetCoreResistValue_Compound(combatant, (f, g) => Mathf.Min(f, g));
        }
        if (coreDamageType == CoreDamageTypeEnum.TRUE)
        {
            totalResistance = 0;    // :D
        }
        return totalResistance;
    }
    private static bool GetCoreResistValue_Basic(Actor combatant, string resistStatInternalName, out float totalResistance)
    {
        totalResistance = 0;

        totalResistance = combatant.CombatController.Stats.CalculateCurrentStatValue(resistStatInternalName);
        return true;
    }
    private static float GetCoreResistValue_Compound(Actor combatant, System.Func<float, float, float> selectionFunction)
    {
        float PhysResistValue;
        GetCoreResistValue_Basic(combatant, Const.PHYSICAL_RESIST_STAT, out PhysResistValue);

        float MageResistValue;
        GetCoreResistValue_Basic(combatant, Const.MAGIC_RESIST_STAT, out MageResistValue);

        //Mathf.Min, or Mathf.Max, really
        return selectionFunction(PhysResistValue, MageResistValue);
    }
    public static float GetMaxHP(StatManager sc)
    {
        return GameMath.CalculateMaxHP(sc.CalculateCurrentStatValue(Const.MAXHP_SCALE_STAT));
    }
}

public struct HitNugget
{
    public readonly Actor Origin;
    public readonly DamageWithType[] Damage;

    public HitNugget(Actor origin, DamageWithType[] damage)
    {
        Origin = origin;
        Damage = damage;
    }
}
