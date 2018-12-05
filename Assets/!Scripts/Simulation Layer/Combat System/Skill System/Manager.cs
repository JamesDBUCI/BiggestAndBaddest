using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Skill
{
    public class Manager : AbstractSlotManager<Controller, Instance>
    {
        public Manager(Actor parent)
            :base(parent)
        {
            //event listeners set by combat controller init phase
        }

        public void UpdateSkills(List<GearInstance> fromGearSet)
        {
            RaiseEvent(onBeforeSlotChanged);
            RemoveAllContents(true);
            fromGearSet.ForEach(gi => AddContents(gi.Template.GearSlot, new Instance(gi.Template.Skill), true));
            RaiseEvent(onAfterSlotChanged);
        }

        public bool TryUse(GearSlotEnum gearType)
        {
            var slot = FindSlotController(gearType);
            if (slot.IsEmpty)   //!Enabled check after the jump
                return false;

            if (!slot.TryUse())      //if it didn't go off
                return false;

            return true;    //we used it
        }

        protected override Dictionary<GearSlotEnum, Controller> GetNewEmptyDictionary()
        {
            var newDict = new Dictionary<GearSlotEnum, Controller>();
            GearSlotType.ForEach(gs => newDict.Add(gs, new Controller(ParentActor, gs)));
            return newDict;
        }
    }

}