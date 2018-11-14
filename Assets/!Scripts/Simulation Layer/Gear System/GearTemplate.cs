using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear")]
public class GearTemplate : ScriptableObject
{
    public string ExternalName;
    public GearSlotEnum GearSlot;
    public Sprite Icon;
    public List<GearModTemplate> ImplicitMods;
    public Skill Skill;
}
