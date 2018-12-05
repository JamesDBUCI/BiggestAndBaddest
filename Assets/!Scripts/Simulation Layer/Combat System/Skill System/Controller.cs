using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public class Controller : AbstractSlotController<Instance>
    {
        //contains one Skill.Instance and functions for interacting with it

        //fields

        public Controller(Actor origin, GearSlotEnum gearType)
            :base(origin, gearType)
        {
            
        }

        public bool TryUse()
        {
            //Core skill execution method. This is the gate that determines whether the skill can be used or not.

            if (IsEmpty)       //can't be used if there is no skill assigned
                return false;

            if (Contents.IsOnCooldown || !Enabled)               //can't use when it's on cooldown
                return false;

            //handle
            CombatController combatController = ParentActor.CombatController;

            if (Contents.Template.InstantSpeedInfo.HasInstantSpeed)            //if it's an instant Skill
            {
                //can't use if we're channeling and the skill can't be used while channeling
                if (combatController.IsChanneling && !Contents.Template.InstantSpeedInfo.CanUseWhileChanneling)
                    return false;

                //cancel my appointments
                combatController.CancelSkillLock();     //this does nothing if there is no skill-lock

                if (Contents.Template.InstantSpeedInfo.CancelsChanneling)
                    combatController.CancelChanneling();    //this does nothing if there is no channeling
            }
            else
            {
                if (combatController.HasSkillLock)          //can't use if we're not instant speed and have skill lock
                    return false;

                if (combatController.IsChanneling)          //can't use if we're not instant speed and are channeling
                    return false;
            }

            //we can do it

            if (Contents.Template.CooldownInfo.HasCooldown)
                Contents.SetCooldown(ParentActor, Contents.Template.CooldownInfo.Duration);      //dynamic cooldowns go here

            if (Contents.Template.ChannelInfo.IsChanneled)      //channel if needed (all necessary checks are done after the jump)
            {
                combatController.StartChanneling(this);
            }
            else
            {
                combatController.SetSkillLock(Contents.Template.SkillLockInfo.Duration);    //set skill-lock
            }

            PerformPhases(PhaseTiming.ON_USE);
            return true;
        }
        public void PerformPhases(PhaseTiming timing)
        {
            //convenience method for accessing Instance's method of same name
            Contents.PerformPhases(ParentActor, timing);
        }
    }
}
