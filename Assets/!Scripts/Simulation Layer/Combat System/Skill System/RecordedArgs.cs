using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public class RecordedArgs
    {
        //contains recorded info about the execution of a skill
        //created at time of skill execution
        //used by effects that reference data stored at the moment of skill execution, rather than at the time of effect
        //readonly values

        //precalculated stats
        public readonly StatController[] RecordedStats;
        //stored cursor location
        public readonly Vector2 RecordedCursorLocation;

        public RecordedArgs(
            StatController[] recordedStats,
            Vector2 recordedLocation)
        {
            RecordedStats = recordedStats;
            RecordedCursorLocation = recordedLocation;
        }
        public RecordedArgs(Actor statSource)
        {
            RecordedStats = statSource.CombatController.Stats.CalculateCurrentStatValues().ToArray();
            RecordedCursorLocation = GameServices.WorldPositionOfMouse();
        }
    }
}
