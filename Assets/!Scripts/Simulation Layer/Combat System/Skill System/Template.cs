using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Skill
{
    public interface ISkillSource : IAimer, IActorOwned
    {
        Transform SourceTransform { get; }
    }

    [CreateAssetMenu(menuName = "Skill/New Skill Template")]
    public class Template : AbstractAssetTemplate
    {
        //Unity-edited asset
        //contains all data for a skill

        /*
         *  GENERAL INFO
         * 
         */

        //(AbstractAssetTemplate.NameExternal covers the name)
        public string Description;
        //(AbstractAssetTemplate.Icon covers the icon)
        public Domain Domain;      //Feat or Spell?
        public List<Tag> Tags = new List<Tag>();        //skill interaction tags with no inherent effects

        /*
         *  TIMING INFO
         * 
         */
        public InstantSpeedInfo InstantSpeedInfo = new InstantSpeedInfo();
        public ChannelInfo ChannelInfo = new ChannelInfo();
        public SkillLockInfo SkillLockInfo = new SkillLockInfo();
        public CooldownInfo CooldownInfo = new CooldownInfo();

        /*
         *  PHASES
         * 
         */
        public List<Phase> Phases = new List<Phase>();
    }
    public enum Domain
    {
        FEAT, SPELL
    }
    public enum Tag
    {
        //to be replaced by yet more ScriptableObjects
        ULTIMATE, HOLY /* More to come */
    }

    [System.Serializable]
    public class InstantSpeedInfo
    {
        //info within a skill template
        //specifies if a skill has instant speed (along with some info about that)

        public bool HasInstantSpeed;            //does it?
        public bool CanUseWhileChanneling;   //false = skill will not trigger if user is mid-channel

        public bool CancelsChanneling;  //only applies if skill can be used while channeling.
                                        //false = you can use this and not break your channel (technically you'll overwrite it if this also has a channel)
    }
    [System.Serializable]
    public class ChannelInfo
    {
        //info within a skill template
        //specifies if a skill has channel time (along with some info about that)

        public bool IsChanneled;            //is it?
        public float Duration;              //how long (in seconds)?
        public bool IsInterruptable;        //(by enemy "Interrupt Target Channel" hit effects)
        public bool IsCanceledByMoving;     //(voluntary movement will cancel channel (cooldown applies))
        public List<DirectModifier> DirectModifiersApplied = new List<DirectModifier>();    //applied to user during channel (removed on cancel/interrupt)
        public List<EffectInfo> InterruptEffects = new List<EffectInfo>();   //applied to interrupting enemy
    }
    [System.Serializable]
    public class SkillLockInfo
    {
        //info within a skill template
        //specifies info about skill-lock (skill animations are synched to skill-lock duration)

        public float Duration;      //takes place after channel if skill is channeled
    }
    [System.Serializable]
    public class CooldownInfo
    {
        //info within a skill template
        //specifies if a skill has cooldown time (along with some info about that)
        public bool HasCooldown;
        public float Duration;
        public float MultiplierIfChannelInterrupted;       //cooldown duration is multiplied by this if channel was interrupted (applies to channeled skills)
    }
    [System.Serializable]
    public class EffectInfo
    {
        //info within a skill template
        //specifies an effect and its arguments/parameters (lots of unused data)

        //[General]
        public Effect Effect;
        public List<TargetEnum> AffectedActors;

        //[Damage Args]
        public List<StatScaler> Scaling = new List<StatScaler>();
        public List<DamageWithType> Potency = new List<DamageWithType>();
        public float ReciprocalPotencyScale = 0;   //apply same potency to user at this scale (lifesteal, recoil, heal % of healing done, give hp to target)

        //[Direct Modifier Args (Stat-changes, DoT, Auras, Auto-skills)]
        public List<DirectModifierChance> DirectModifierChances = new List<DirectModifierChance>();

        //[Movement Args]
        public float MovementForceMultiplier = 1;

        //[Miscellaneous Args]
        public bool InterruptsEnemyChannel = false;
    }
}