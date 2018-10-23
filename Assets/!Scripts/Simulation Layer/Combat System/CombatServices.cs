using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatServices
{
    public static DatabaseHelper<AttackType> AttackTypeDB = new DatabaseHelper<AttackType>("Attack Types", "AttackType");
    public static bool LoadAttackTypeDB()
    {
        return AttackTypeDB.Load();
    }

    public static DamagePacket ConstructDamagePacket(ICombatant origin, AttackType attack)
    {
        StatController sc = origin.GetStatController();

        //empty list
        List<DamageWithType> damageNodes = new List<DamageWithType>();

        float totalScaleStatValue = 0;

        //iterate stat scaling
        foreach (StatScaler statScaler in attack.StatScaling)
        {
            totalScaleStatValue += sc.CalculateCurrentStatValue(statScaler.Stat) * statScaler.Scale;
        }

        foreach (DamageWithType potency in attack.DamagePotency)
        {
            //Debug.Log("Damage going into packet: " + totalScaleStatValue * (potency.Value / 100) + " " + potency.Types[0].ExternalName);
            damageNodes.Add(new DamageWithType(totalScaleStatValue * (potency.Value / 100), potency.Types.ToArray()));
        }

        return new DamagePacket(origin, damageNodes.ToArray());
    }
    public static void TakeDamagePacket(ICombatant combatant, DamagePacket damage)
    {
        //grab a handle
        StatController sc = combatant.GetStatController();

        //iterate all type nodes of incoming damage
        float totalDamageAfterMitigation = 0;
        foreach (DamageWithType dmg in damage.DamageNodes)
        {
            //list of resistances for each damage type in the node
            List<float> subtypeResistances = new List<float>();

            //iterate all types within the node
            foreach (DamageType dt in dmg.Types)
            {
                if (dt == null)     //if there is an empty damage type slot
                    continue;

                if (dt.ResistanceStat.Stat == null)     //if there is an empty resistance stat type slot
                    continue;

                //if the damage type can be resisted

                float subtypeResistance = 0;

                //get resist stat scaled value (cap it at max, since we're calculating as resistance)
                    
                subtypeResistance = Mathf.Min(StatServices.MAX_RESISTANCE_STAT_VALUE,
                                                sc.CalculateCurrentStatValue(dt.ResistanceStat.Stat) * dt.ResistanceStat.Scale);

                //Debug.Log("Resistance Stat found and value applied: " + subtypeResistance);

                /*
                    * We could make unorthodox stats like Critical Hit Rate or move speed into resist stats for something,
                    * so we have to cap the stat total here and only here.
                */

                //add it to the list
                subtypeResistances.Add(subtypeResistance);
            }

            //get average of resists in case of multi-type damage node
            float averageOfResists = 0;

            //there is an outlying chance that the only damage type in a node is not resistable, leading to a division by 0.
            //so we make a conditional
            if (subtypeResistances.Count != 0)
            {
                foreach (float subtypeRes in subtypeResistances)
                {
                    averageOfResists += subtypeRes;
                }
                averageOfResists /= subtypeResistances.Count;   //NOTE: non-resistable damage types (no resist stat provided) will not affect average resistance
            }

            float nodeDamage = GameMath.CalculateDamageAfterResistance(dmg.Value, averageOfResists);
            totalDamageAfterMitigation += nodeDamage;
            //Debug.Log("Damage going into boss: " + nodeDamage + " " + dmg.Types[0].ExternalName);
        }

        //TAKE THAT HIT
        combatant.GetCombatController().ChangeHP(Mathf.RoundToInt(-totalDamageAfterMitigation));
    }
}

public interface ICombatant : IHaveStats
{
    CombatController GetCombatController();
    Transform GetProjectileTransform();
}

public struct DamagePacket
{
    public readonly ICombatant Origin;
    public readonly List<DamageWithType> DamageNodes;

    public DamagePacket(ICombatant origin, params DamageWithType[] damageNodes)
    {
        Origin = origin;
        DamageNodes = new List<DamageWithType>(damageNodes);
    }
}
