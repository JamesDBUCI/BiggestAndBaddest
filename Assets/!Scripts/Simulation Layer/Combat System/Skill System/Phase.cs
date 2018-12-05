using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Skill
{
    public enum PhaseTiming
    {
        ON_USE, ON_CHANNEL_COMPLETE
    }
    [System.Serializable]
    public class Phase
    {
        //contains info about a single action-effects relationship in a skill
        //skills have 1..n of these

        //timing
        public PhaseTiming Timing = PhaseTiming.ON_USE;
        //action (dropped in editor)
        public Action Action;

        //action info
        public ProjectileInfo ProjectileInfo = new ProjectileInfo();
        public AoeInfo AoeInfo = new AoeInfo();
        public ZoneInfo ZoneInfo = new ZoneInfo();
        public FieldTriggerInfo FieldTriggerInfo = new FieldTriggerInfo();
        //trap info

        //effect info list
        public List<EffectInfo> Effects = new List<EffectInfo>();
    }
    [System.Serializable]
    public class ZoneInfo
    {
        //info within a skill template
        //specifies how and where a zone is generated

        [Header("Zone Info - General")]
        public OptionalFloat MaxDistanceFromUser = new OptionalFloat(4);    //set to false to place Zone anywhere on screen. set to 0 to lock at user's feet.
        public float Lifetime = 5f;     //zone will be destroyed after x seconds
        //aura and auto-skill radii determined individually

        [Header("Zone Info - Contents")]
        public List<AuraInstance> Auras = new List<AuraInstance>();
        public List<AutoSkillInstance> AutoSkills = new List<AutoSkillInstance>();
    }
    [System.Serializable]
    public class AoeInfo
    {
        //info within a skill template
        //specifies how and where an AoE happens

        [Header("AoE Info - Placement")]
        public OptionalFloat MaxDistanceFromUser = new OptionalFloat(4);    //set to false to place AoE anywhere on screen. set to 0 to lock at user's feet.

        [Header("AoE Info - Hitbox")]
         //scale and shape of hitbox is defined by asset
        public float RotationOffset = 0;        //0 degrees offset is normally in the user's aim direction
        public bool RotationIsGlobal = false;   //if so, 0 degrees offset is always up

        [Header("AoE Info - Indicator")]
        public bool HideAoeIndicator = false;                       //indicator only displays for channeled skills
        [Range(0, 2)] public float IndicatorDurationMultipler = 1;  //default (1) is half of channel duration
    }
    [System.Serializable]
    public class ProjectileInfo
    {
        //info within a skill template
        //specifies info about the fired projectile that varies between most skills

        public float Range = 9f;                //range in Unity Units the projectile can travel from firing point before destruction
        public bool PiercesEnemies = false;     //bullet continues through enemies hit to complete full range

        //trajectory would go here (sine wave motion, etc...)
    }
    [System.Serializable]
    public class FieldTriggerInfo
    {
        //info within a skill template
        //specifies how a field trigger is invoked

        //field triggers include things like "destroy arena walls" and "hard enrage"
        //they can be thought of as parameterless Actions that can be referenced in a simple Dictionary

        public string TriggerName;
    }
}