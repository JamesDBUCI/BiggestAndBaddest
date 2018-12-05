using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierAlignment
{
    NONE = 0, BUFF = 1, DEBUFF = 2, MIXED = 3
}
public abstract class AbstractModifierTemplate : AbstractAssetTemplate
{
    public string Description = "";
    //more vanity

    //general
    public ModifierAlignment Alignment = ModifierAlignment.BUFF;

    //stacking
    public OptionalInt MaxStackSize = new OptionalInt(1);    //how many of this stat modifier can be stacked on each other? (can be disabled for no limit)

    //always-on effects
    public List<StatChange> StatChanges = new List<StatChange>();
    public List<StatusFlag> StatusFlags = new List<StatusFlag>();
}
