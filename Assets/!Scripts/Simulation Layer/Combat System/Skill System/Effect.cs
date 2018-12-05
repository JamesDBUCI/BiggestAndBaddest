using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Skill
{
    public abstract class Effect : ScriptableObject
    {
        //subclasses are Unity-edited assets
        //requires Apply() method for applying the effect to pre-validated targets

        protected abstract void Apply(Actor validTarget, ISkillSource skillSource, Vector2 skillContactPoint, EffectInfo effect, RecordedArgs recordedArgs);

        //target validation (return true if target is valid)
        public bool ApplyIfValid(Actor actorHit, ISkillSource skillSource, Vector2 skillContactPoint, EffectInfo effect, RecordedArgs recordedArgs)
        {
            //if the target is considered an "AffectedActor"
            if (TargetHelper.IsValidTarget(effect.AffectedActors, skillSource.Origin, actorHit))
            {
                //apply the effect(varies by subclass)
                Apply(actorHit, skillSource, skillContactPoint, effect, recordedArgs);
                return true;
            }
            return false;
        }
        //multi-target validation (return true if any targets are valid)
        public bool ApplyIfValid(List<Actor> actorsHit, ISkillSource skillSource, Vector2 skillContactPoint, EffectInfo effect, RecordedArgs recordedArgs)
        {
            bool appliedToAny = false;
            foreach (var actorHit in actorsHit)
            {
                if (ApplyIfValid(actorHit, skillSource, skillContactPoint, effect, recordedArgs))
                    appliedToAny = true;
            }
            return appliedToAny;
        }
    }
    [CreateAssetMenu(menuName = "Skill/Effects/New Hit Effect")]
    public class HitEffect : Effect
    {
        //Unity-edited asset
        //contains all data needed to perform a hit effect on a target

        public bool UseRecordedStats = true;           //as opposed to calculating at time of impact
        public bool UseRecordedModifierRolls = true;    //as opposed to roling at time of impact

        protected override void Apply(Actor validTarget, ISkillSource skillSource, Vector2 skillContactPoint, EffectInfo effect, RecordedArgs recordedArgs)
        {
            var statsToUse =                    //we will use the following stats to determine potency (damage or healing)
                UseRecordedStats ?              //recorded or recalculate?
                recordedArgs.RecordedStats.ToList() :                                     //if yes, use recorded stats
                skillSource.Origin.CombatController.Stats.CalculateCurrentStatValues();     //if no, get new stat values

            var hitNugget = CombatServices.ConstructHitNugget(skillSource.Origin, effect, statsToUse);

            CombatServices.TakeDamageOrHeal(validTarget, hitNugget);    //target takes potency based on whatever stats we're using

            //roll direct modifier chances
            var directModifiersToApply =
                effect.DirectModifierChances
                .Where(dmc => dmc.WillItApply())
                .Select(rolled => rolled.Modifier).ToList();

            directModifiersToApply.ForEach(dm => validTarget.CombatController.DirectModifiers.TryAddTimer(dm, dm.Duration));   //apply direct modifiers to target

            //origin Actor takes reciprocal potency (ex. recoil, lifesteal, % self heal on ally heal, damage self to heal allies)
            CombatServices.TakeDamageOrHeal(skillSource.Origin, hitNugget, effect.ReciprocalPotencyScale);

            //possibly interrupt enemy channel. interruption validity figured further down the road
            if (effect.InterruptsEnemyChannel)
                validTarget.CombatController.InterruptChanneling(interruptor: skillSource.Origin);
        }
    }
    [CreateAssetMenu(menuName = "Skill/Effects/New Movement Effect")]
    public class MovementEffect : Effect
    {
        //Unity-edited asset
        //contains all data needed to perform a hit effect on a target

        //default values equate to basic forward dodge
        public bool MovementIsVoluntary = true;     //dodge = voluntary, knockback = involuntary

        public RelativeDirection Direction = RelativeDirection.TOWARD;          //where
        public RelativeWorldPoint WorldPoint = RelativeWorldPoint.AIM_POINT;    //in relation to

        public bool UseRecordedCursorLocation = false;

        protected override void Apply(Actor validTarget, ISkillSource skillSource, Vector2 skillContactPoint, EffectInfo effect, RecordedArgs recordedArgs)
        {
            var cursorLocationUsed =            //we will use the following cursor location
                UseRecordedCursorLocation ?     //recorded or recalculate?
                recordedArgs.RecordedCursorLocation :                 //if yes, use recorded
                (Vector2)GameServices.WorldPositionOfMouse();   //if, no, get current location of cursor

            //get relative direction to move in
            var normalizedDirection = RelativeMovementHelper.GetNormalizedDirection
                (validTarget, skillSource.SourceTransform.position, cursorLocationUsed, Direction, WorldPoint);

            //apply movement force to target
            validTarget.AddMovementForce
                (normalizedDirection, Const.DODGE_FORCE_VALUE * effect.MovementForceMultiplier, MovementIsVoluntary);
        }

        public enum RelativeWorldPoint
        {
            //contact object is the object that is applying the effect (the bullet itself, the origin point of the AoE, etc...)
            //aim point and cursor will very nearly always be the same (rotation speed reduction will make the difference)
            CONTACT_OBJECT = 0, AIM_POINT = 1, CURSOR = 2,
            //want to add closest ally, closest enemy, etc... maybe just move in a whole condition system and tie this into AI
        }
        public enum RelativeDirection
        {
            TOWARD = 0, AWAY_FROM = 1
        }
        private static class RelativeMovementHelper
        {
            //static helper class that calculates relative directions for movement skills

            //the only exposed method. use it.
            public static Vector2 GetNormalizedDirection
                (Actor movedActor, Vector2 skillContactPoint, Vector2 cursorLocation, RelativeDirection relativeDirection, RelativeWorldPoint relativeWorldPoint)
            {
                //get point 1 of line
                Vector2 movedActorPosition = movedActor.GetFeetColliderWorldLocation();

                //get point 2 of line
                Vector2 worldPoint = GetWorldPoint(movedActor, skillContactPoint, cursorLocation, relativeWorldPoint);

                //get direction as simple +/-
                float directionSign = relativeDirection == RelativeDirection.TOWARD ? 1 : -1;

                //vector maths
                return (movedActorPosition - worldPoint) * directionSign;
            }

            //self-explanatory
            private static Vector2 GetWorldPoint(Actor movedActor, Vector2 skillContactPoint, Vector2 cursorLocation, RelativeWorldPoint relativeWorldPoint)
            {
                if (relativeWorldPoint == RelativeWorldPoint.CONTACT_OBJECT)
                    return skillContactPoint;
                if (relativeWorldPoint == RelativeWorldPoint.AIM_POINT)
                    return movedActor.AimManager.GetAimDirection();     //this direction should be the same value as position relative to actor location
                if (relativeWorldPoint == RelativeWorldPoint.CURSOR)
                    return cursorLocation;

                //default
                return skillContactPoint;
            }
        }
    }
}
