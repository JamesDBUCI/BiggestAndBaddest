using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSkillInstance : AbstractTemplateInstance<AutoSkillTemplate>
{
    public float TickDuration;
    public float TotalDuration;
    public float MaxDetectRange;

    public AutoSkillInstance(AutoSkillTemplate template, float tickDuration = 0.5f, float totalDuration = 6f, float maxDetectRange = 5f)
        :base(template)
    {
        TickDuration = tickDuration;
        TotalDuration = totalDuration;
        MaxDetectRange = maxDetectRange;
    }
}
