using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public enum TargetEnum
    {
        SELF, ALLIES, ENEMIES
    }
    public static class TargetHelper
    {
        //helper class which figures out who is a valid target of a Skill.Effect

        //dictionary
        private static Dictionary<TargetEnum, System.Func<Actor, Actor, bool>> _all = new Dictionary<TargetEnum, System.Func<Actor, Actor, bool>>()
        {
            { TargetEnum.SELF, (origin, target) => target == origin },
            { TargetEnum.ALLIES, (origin, target) => _involvesNeutralActor(origin, target) || target.Faction == origin.Faction},
            { TargetEnum.ENEMIES, (origin, target) => _involvesNeutralActor(origin, target) || target.Faction != origin.Faction}
        };

        //helper method for handling Neutral Actors
        private readonly static System.Func<Actor, Actor, bool> _involvesNeutralActor =
            (origin, target) => origin.Faction == ActorFaction.NEUTRAL || target.Faction == ActorFaction.NEUTRAL;

        //core method for determining valid relative targets
        public static bool IsValidTarget(TargetEnum targetType, Actor origin, Actor target)
        {
            System.Func<Actor, Actor, bool> found;
            _all.TryGetValue(targetType, out found);
            return found(origin, target);
        }
        //overload
        public static bool IsValidTarget(List<TargetEnum> targetTypeList, Actor origin, Actor target)
        {
            return targetTypeList.TrueForAll(stt => IsValidTarget(stt, origin, target));
        }
    }
}
