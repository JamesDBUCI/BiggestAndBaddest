using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GearSlotEnum
{
    //undefined is necessary when unassigning skills, etc...
    UNDEFINED = -1, WEAPON_MAINHAND = 0, WEAPON_OFFHAND = 1, HEAD = 2, CHEST = 3, ARMS = 4, LEGS = 5, FEET = 6
}
public class GearSlotType
{
    //statics
    public static Dictionary<GearSlotEnum, GearSlotType> All = new Dictionary<GearSlotEnum, GearSlotType>()
    {
        { GearSlotEnum.WEAPON_MAINHAND, new GearSlotType(GearSlotEnum.WEAPON_MAINHAND, "Main Hand") },
        { GearSlotEnum.WEAPON_OFFHAND, new GearSlotType(GearSlotEnum.WEAPON_OFFHAND, "Off Hand") },
        { GearSlotEnum.HEAD, new GearSlotType(GearSlotEnum.HEAD, "Head") },
        { GearSlotEnum.CHEST, new GearSlotType(GearSlotEnum.CHEST, "Chest") },
        { GearSlotEnum.ARMS, new GearSlotType(GearSlotEnum.ARMS, "Arms") },
        { GearSlotEnum.LEGS, new GearSlotType(GearSlotEnum.LEGS, "Legs") },
        { GearSlotEnum.FEET, new GearSlotType(GearSlotEnum.FEET, "Feet") },
    };
    public static List<GearSlotEnum> GetAllGearSlots()
    {
        return All.Keys.ToList();
    }
    public static void ForEach(System.Action<GearSlotEnum> action)
    {
        foreach (var gearType in All.Keys)
        {
            action(gearType);
        }
    }

    //instance fields
    public readonly GearSlotEnum EnumValue;
    public readonly string Name;
    //icon, etc...

    public GearSlotType(GearSlotEnum enumValue, string name)
    {
        EnumValue = enumValue;
        Name = name;
    }
}
