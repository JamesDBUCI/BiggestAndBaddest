using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AuraInstance : AbstractModifier<AuraTemplate>
{
    public float RadiusMultiplier = 1;

    //might need to add a reference to the Skill.Phase that spawned this

    public AuraInstance(Actor origin, AuraTemplate template, float radiusMultiplier)
        :base(template, origin)
    {
        RadiusMultiplier = radiusMultiplier;
    }
}
