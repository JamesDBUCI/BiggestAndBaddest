using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectModifier : AbstractModifier<DirectModifierTemplate>, IHaveDuration
{
    public float Duration { get; private set; }

    public DirectModifier(DirectModifierTemplate template, float duration, Actor origin, bool linkedModifier)
        :base(template, origin)
    {
        Duration = duration;
      //  IsLinkedModifier = linkedModifier;
    }
}

public struct DirectModifierChance
{
    public readonly DirectModifier Modifier;
    public readonly float Chance;

    public DirectModifierChance(DirectModifier modifier, float chance)
    {
        Modifier = modifier;
        Chance = chance;
    }
    public bool WillItApply()       //haha, why did I put it in the struct? Because I can.
    {
        return Random.Range(0f, 1f) <= Chance;
    }
}
