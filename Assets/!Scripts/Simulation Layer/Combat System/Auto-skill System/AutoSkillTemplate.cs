using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/New Auto-Skill Template")]
public class AutoSkillTemplate : AbstractAssetTemplate
{
    public Skill.Phase Phase = new Skill.Phase();
}
