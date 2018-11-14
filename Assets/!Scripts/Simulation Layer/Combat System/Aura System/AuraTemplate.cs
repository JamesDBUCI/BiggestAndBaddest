using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/New Aura Template")]
public class AuraTemplate : AbstractModifierTemplate
{
    //auras don't tick. remember this.

    public float AuraBaseRadius = 1;
    public List<Skill.TargetEnum> AffectedTargets = new List<Skill.TargetEnum>();
}
