using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Direct Modifier")]
public class DirectModifierTemplate : AbstractModifierTemplate
{
    //stacking
    public bool OverwriteOldest = true;                     //if max stacks is reached, will we overwrite the oldest stacks, or will we ignore any more until room is made?
    public bool RefreshStacksOnApply = false;               //does this refresh duration of all stacks of the same modifier when a new one is appled?
    public bool LoseStacksOnRemove = false;                 //do all stacks get removed when one is lost?

    //per tick
    public List<DamageWithType> DamageOrHealingPerTick = new List<DamageWithType>();

    //linked crowd control
    public List<CrowdControl> AppliedCrowdControl = new List<CrowdControl>();
    public bool CrowdControlIgnoresReduction = false;
    public bool CrowdControlCuredWithModifier = false;

    //linked modifiers
    [System.Serializable]
    public abstract class LinkedMod
    {
        //information about modifiers linked to this one

        //the template to be added or removed
        public DirectModifierTemplate Modifier = null;

        //the chance (0~1) that this will proc (be added or removed)
        [Range(0, 1)] public float chanceToProc = 1;

        //You should ignore this linked mod if the parent mod is expiring unnaturally
        public bool IgnoreIfUnnaturalExpire = true;
    }
    [System.Serializable]
    public class AddedMod : LinkedMod
    {
        public float Duration = 1;        
    }
    [System.Serializable]
    public class RemovedMod : LinkedMod
    {
        public OptionalInt RemoveLimitedStackCount = new OptionalInt(false);
    }

    ////list of modifiers that may be added when this is successfully applied to a target (and if they are ignored when main mod is applied as a linked mod)
    //public List<AddedMod> AddedOnApply = new List<AddedMod>();
    //list of modifiers that may be removed when this is successfully applied to a target
    public List<RemovedMod> RemovedOnApply = new List<RemovedMod>();
    //list of modifiers that may be added when this is removed from a target (and if they are ignored on unnatural recovery such as cure or stack overwrite)
    public List<AddedMod> AddedOnExpire = new List<AddedMod>();
    ////list of modifiers that may be removed when this is removed from a target (and if they are ignored on cure)
    //public List<RemovedMod> RemovedOnExpire = new List<RemovedMod>();
}
