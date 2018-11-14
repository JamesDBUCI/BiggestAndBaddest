using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearSlotType
{
    //statics
    public static Dictionary<GearSlotEnum, GearSlotType> All = new Dictionary<GearSlotEnum, GearSlotType>()
    {
        { GearSlotEnum.WEAPON_MAIN, new GearSlotType("Main Hand") },
        { GearSlotEnum.WEAPON_OFFHAND, new GearSlotType("Off Hand") },
        { GearSlotEnum.HEAD, new GearSlotType("Head") },
        { GearSlotEnum.CHEST, new GearSlotType("Chest") },
        { GearSlotEnum.ARMS, new GearSlotType("Arms") },
        { GearSlotEnum.LEGS, new GearSlotType("Legs") },
        { GearSlotEnum.FEET, new GearSlotType("Feet") },
    };

    //instance fields
    public string Name;
    //icon, etc...
    public GearSlotType(string name)
    {
        Name = name;
    }
}
