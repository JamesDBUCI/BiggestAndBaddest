using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AffixSlotEnum
{
    PREFIX, SUFFIX, IMPLICIT, UNIQUE
}

public class AffixSlot
{
    //meta data for how mods are treated based on the affix slot they take up.
    //all instances are defined at execution time and accessed with AffixSlot.TryGet

    //internal dictionary
    private static Dictionary<AffixSlotEnum, AffixSlot> _all = new Dictionary<AffixSlotEnum, AffixSlot>()
    {
        { AffixSlotEnum.PREFIX, new AffixSlot("{1} {0}", true)},  //"Prefix Existing Name", "{Heavy} {Greataxe}"
        { AffixSlotEnum.SUFFIX, new AffixSlot("{0} {1}", true)},  //"Existing Name Suffix", "{Greataxe} {of Giants}"
        { AffixSlotEnum.IMPLICIT, new AffixSlot("{0}", false)},   //"Existing Name", "{Greataxe}" (Implicit mods don't affect name)
        { AffixSlotEnum.UNIQUE, new AffixSlot("{0}", false)},     //"Existing Name", "{Greataxe}" (Unique moddables supply their own name)
    };

    //get an affixSlot handle (if it exists)
    public static bool TryGet(AffixSlotEnum enumValue, out AffixSlot foundValue)
    {
        if (!_all.TryGetValue(enumValue, out foundValue))
        {
            Debug.LogError("Unable to locate AffixSlot with type enum: " + enumValue);
            return false;
        }
        return true;
    }

    //instance fields
    public readonly string NameFormat;      //the way this mod affects an existing moddable name (uses standard .NET framework string.Format() method)
    public readonly bool CountIsLimited;    //should be different when rarity system is created

    //private constructor (no new instances after execution time)
    private AffixSlot(string nameFormat, bool countIsLimited)
    {
        NameFormat = nameFormat;
        CountIsLimited = countIsLimited;
    }

    //add mod external name to existing name
    public string ModifyName(string originalName, string newContent)
    {
        //{0} = original name, {1} = new content

        return string.Format(NameFormat, originalName, newContent);
    }
}
