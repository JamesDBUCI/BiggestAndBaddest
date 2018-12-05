using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear")]
public class GearTemplate : AbstractAssetTemplate
{
    public GearSlotEnum GearSlot;
    public List<GearModTemplate> ImplicitMods;
    public Skill.Template Skill;
}
