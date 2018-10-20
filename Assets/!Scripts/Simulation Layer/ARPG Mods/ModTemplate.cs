using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mod System/New ModTemplate")]
public class ModTemplate : ScriptableObject
{
    //Unity's editable template for a mod

    public string NameExternal;
    public AffixSlotEnum AffixSlot;
    public List<StatChangeTemplate> StatChanges;
    public List<string> Flags;
}
