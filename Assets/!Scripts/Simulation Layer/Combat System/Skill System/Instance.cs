using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Skill
{
    public class Instance : AbstractTemplateInstance<Template>
    {
        //instance of a Skill.Template

        //cooldown stuff
        private CombatTimer _cooldown;
        public bool IsOnCooldown
        {
            get
            {
                if (_cooldown != null && _cooldown.InProgress)
                    return true;
                return false;
            }
        }
        public float CooldownProgressNormalized
        {
            get
            {
                if (IsOnCooldown)
                    return _cooldown.ProgressNormalized;
                return 1;
            }
        }
        public float CooldownRemainingTime
        {
            get
            {
                if (IsOnCooldown)
                    return _cooldown.RemainingTime;
                return 0;
            }
        }

        //events
        //onStartCooldown
        //onCompleteCooldown

        public Instance(Template template)
            : base(template) { }

        public void SetCooldown(Actor origin, float duration)
        {
            if (_cooldown != null)
                _cooldown.CleanUpListeners();

            _cooldown = new CombatTimer(origin, duration);
        }
        public List<Phase> GetPhases(PhaseTiming timing)        //is it neccessary for this to be public (or exist)?
        {
            return Template.Phases.FindAll(p => p.Timing == timing);
        }
        public void PerformPhases(Actor user, PhaseTiming timing)
        {
            var phases = GetPhases(timing);
            if (phases.Count > 0)
            {
                foreach (var phase in phases)
                {
                    phase.Action.Perform(user, phase);
                }
            }
        }
    }
}

//public class SkillInstance : AbstractTemplateInstance<SkillTemplate>
//{
//    //an instance of a SkillTemplate

//    //fields and field accessors
//    public Actor ParentActor { get; private set; }
//    public SkillTemplate Skill { get; private set; }        //make a new SkillController if you want a new skill.
//    public bool Enabled { get; set; }

//    private CombatTimer _cooldownTimer = null;
//    public bool IsOnCooldown
//    {
//        get
//        {
//            if (_cooldownTimer != null)
//            {
//                return _cooldownTimer.InProgress;
//            }
//            return false;
//        }
//    }
//    public TimerInfo CooldownTimerInfo { get { return IsOnCooldown ? new TimerInfo(_cooldownTimer) : new TimerInfo(); } }

//    //ctor
//    public SkillInstance(Actor parent, SkillTemplate template)
//        :base(template)
//    {
//        ParentActor = parent;
//        Skill = template;
//        Enabled = true;
//    }

//    //functionality
//    public bool TryUse()
//    {
//        if (IsOnCooldown)               //can't use when it's on cooldown
//            return false;

//        //handle
//        CombatController combatController = ParentActor.CombatController;

//        if (Skill.IsInstant)            //if it's an instant Skill
//        {
//            //can't use if we're channeling and the skill can't be used while channeling
//            if (combatController.IsChanneling && !Skill.InstantSpeedInfo.CanUseWhileChanneling)
//                return false;

//            //cancel my appointments
//            if (combatController.HasSkillLock)
//                combatController.CancelSkillLock();
//            if (combatController.IsChanneling)
//                combatController.CancelChanneling();
//        }            

//        if (combatController.HasSkillLock)          //can't use if we still have skill lock
//            return false;

//        if (combatController.IsChanneling)          //can't use if we're still channeling
//            return false;

//        //we can do it

//        if (Skill.HasCooldown)
//            SetCooldown();      //adjusted cooldown duration after the jump

//        combatController.SetSkillLock(this);

//        if (Skill.IsChanneled)      //channel if needed (all necessary checks are done after the jump)
//        {
//            //CombatController.StartChanneling() will call Use() when (and if) it's done
//            combatController.StartChanneling(this);
//        }
//        else
//        {
//            //main hit goes here?
//        }

//        return true;
//    }

//    public bool SetCooldown()
//    {
//        float adjustedCooldownDuration = ParentActor.CombatController.GetAdjustedCooldownDuration(Skill.CooldownInfo.CooldownDuration);
//        if (adjustedCooldownDuration > 0)
//        {
//            if (_cooldownTimer != null)
//                _cooldownTimer.CleanUpListeners();

//            _cooldownTimer = new CombatTimer(ParentActor, adjustedCooldownDuration);
//            return true;
//        }
//        return false;
//    }

//    public void PerformPhases(SkillPhaseTimingEnum timing)
//    {
//        Skill.Phases.AllPhases.Where(p => p.Timing == timing).ToList().ForEach(p => PerformPhase(p));
//    }
//    private void PerformPhase(SkillPhase phase)
//    {
//        var effectArgs = new SkillArgs(ParentActor, phase);
//        SkillServices.PerformSkillAndApplyEffects(phase, effectArgs);
//    }
//}
