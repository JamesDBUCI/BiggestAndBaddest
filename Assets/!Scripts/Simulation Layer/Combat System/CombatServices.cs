using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoreDamageTypeEnum
{
    PHYSICAL, MAGIC, HYBRID, ADAPTIVE_HIGH, ADAPTIVE_LOW, TRUE
}
public static class CombatServices
{
    public static float GetStatValueFromSet(StatTemplate stat, List<StatController> set)
    {
        StatController foundStat = set.Find(setStat => setStat.Template == stat);
        if (foundStat == null)
            return 0;

        return foundStat.Value;
    }
    public static DamagePacket ConstructDamagePacket(Actor origin, List<StatController> CalculatedStatSet, SkillPhase phase)
    {
        //this overload assumes you have the stats you want to use for calculation
        //(either because it was delayed damage, or you just got them from the other overload)
        
        if (phase.Potency == null)
            return new DamagePacket(origin, new DamageWithType(CoreDamageTypeEnum.TRUE, 0, null));   //blank damage

        //empty list
        List<DamageWithType> damageNodes = new List<DamageWithType>();

        float totalScaleStatValue = 0;

        //iterate stat scaling
        foreach (StatScaler statScaler in phase.StatScaling)
        {
            totalScaleStatValue += GetStatValueFromSet(statScaler.Stat, CalculatedStatSet) * statScaler.Scale;
        }

        foreach (DamageWithType potency in phase.Potency)
        {
            //Debug.Log("Damage going into packet: " + totalScaleStatValue * (potency.Value / 100) + " " + potency.Types[0].ExternalName);
            damageNodes.Add(new DamageWithType(potency.CoreDamageType, totalScaleStatValue * (potency.Value / 100), potency.SecondaryTypes.ToArray()));
        }

        return new DamagePacket(origin, damageNodes.ToArray());
    }
    public static DamagePacket ConstructDamagePacket(Actor origin, SkillPhase skillPhase)
    {
        //this overload assumes you want to calculate the damage based on stat values at this exact moment in time.
        return ConstructDamagePacket(origin, origin.CombatController.Stats.CalculateCurrentStatValues(), skillPhase);
    }
    public static void TakeDamagePacket(Actor combatant, DamagePacket damage)
    {
        //grab a handle
        StatManager sc = combatant.CombatController.Stats;

        //iterate all type nodes of incoming damage
        float totalDamageAfterMitigation = 0;
        foreach (DamageWithType dmg in damage.DamageNodes)
        {
            //resistance to the core damage type of the hit
            float coreResistValue = GetCoreResistValue(combatant, dmg.CoreDamageType);

            //list of resistances for each secondary damage type in the node
            List<float> secondaryResistances = new List<float>();

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

            float nodeDamage = GameMath.CalculateAdjustedDamage(dmg.Value, coreResistValue, averageOfResists);
            totalDamageAfterMitigation += nodeDamage;
            //Debug.Log("Damage going into boss: " + nodeDamage + " " + dmg.Types[0].ExternalName);
        }

        //TAKE THAT HIT
        combatant.CombatController.ChangeHP(Mathf.RoundToInt(-totalDamageAfterMitigation));
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

public struct DamagePacket
{
    public readonly Actor Origin;
    public readonly List<DamageWithType> DamageNodes;

    public DamagePacket(Actor origin, params DamageWithType[] damageNodes)
    {
        Origin = origin;
        DamageNodes = new List<DamageWithType>(damageNodes);
    }
}
