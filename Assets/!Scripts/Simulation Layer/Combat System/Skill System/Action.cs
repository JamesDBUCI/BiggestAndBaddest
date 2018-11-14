using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skill
{
    public abstract class Action : ScriptableObject    //Skill.Action
    {
        //subclasses are Unity-edited assets

        //required Perform() method for producing the skill itself
        //"user" is of type Actor because all actions must be performed by Actors.
        //Other ISkillUser implementations apply effects directly but do not perform the action itself
        //entire "phase" is passed to send action info (ProjectileInfo, etc...)
        public abstract void Perform(Actor user, ISkillSource source, Phase phase);
        public void Perform(Actor user, Phase phase)
        {
            //overload where the actor is the source of the skill
            Perform(user, user, phase);
        }
    }
    [CreateAssetMenu(menuName = "Skill/Actions/New Projectile Action")]
    public class ProjectileAction : Action
    {
        //Unity-edited asset
        //contains all data needed to perform a projectile attack
        public CollidingSprite CollidingSprite;     //upward for the image is forward for the projectile

        public float Speed;
        public float SizeMultiplier;

        public override void Perform(Actor user, ISkillSource source, Phase phase)
        {
            //handle
            AimManager aimer = user.AimManager;

            //instantiate projectile prefab at aim location with aim rotation
            ProjectileController pc = Instantiate(
                Game.Self.ProjectilePrefab,
                aimer.GetLocation(),
                aimer.GetComplexRotation()
                ).GetComponent<ProjectileController>();

            pc.Arm(new RecordedArgs(user), phase, CollidingSprite, Speed, SizeMultiplier);    //inject everything
        }
    }

    //zone action
    //public class ZoneAction : Action
    //{
    //    //Unity-edited asset
    //    //contains all data needed to perform a projectile attack
    //    public CollidingSprite CollidingSprite;     //shape of zone
    //    public float Scale;

    //    public override void Perform(Actor user, ISkillSource source, Phase phase)
    //    {
    //        //handle
    //        AimManager aimer = user.AimManager;
    //        ZoneInfo zInfo = phase.ZoneInfo;

    //        //location
    //        Vector2 worldMousePos = GameServices.WorldPositionOfMouse();    //grab current cursor position
    //        Vector2 placementLocation =             //put the zone here:
    //            zInfo.MaxDistanceFromUser.Enabled ?     //is there a max distance from player?
    //                Vector2.MoveTowards(aimer.GetLocation(), worldMousePos, zInfo.MaxDistanceFromUser.Value) :  //if so, on closest valid point to the cursor
    //                worldMousePos;      //if not, exactly on the cursor

    //        //rotation
    //        float placementRotation = 
    //            zInfo.

    //        //instantiate zone prefab at location with correct rotation
    //        ZoneController zc = Instantiate(
    //            Game.Self.ZonePrefab,
    //            placementLocation,
                
    //            );

    //        zc.Arm(new RecordedArgs(user), phase, CollidingSprite, Speed, Scale);    //inject everything
    //    }
    //}

    //aoe action

    //field trigger action

    //trap action?
}
