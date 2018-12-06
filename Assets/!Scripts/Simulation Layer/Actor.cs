using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Component for a GameObject that represents and controls a combatant in battle.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour, Skill.ISkillSource
{
    /// <summary>
    /// Which Player-relative faction the Actor belongs to.
    /// </summary>
    public ActorFaction Faction { get; set; }

    /// <summary>
    /// Where Auras and other "ground"-based things are detected (ground in a 2D world)
    /// </summary>
    public Collider2D FeetCollider;

    /// <summary>
    /// Empty GameObject which rotates around to track where the Actor is looking/facing/pointing.
    /// </summary>
    public Transform ActorAimTransform;      //can get more complicated for multiple aim transforms. We'll see.

    /// <summary>
    /// Empty GameObject which acts as a parent transform for Auras broadcasted by the Actor. For organization purposes only.
    /// </summary>
    public Transform ActorAuraTransform;

    /// <summary>
    /// Empty GameObject which acts as a parent transform for Auto-skills being cast by the Actor. For organization purposes only.
    /// </summary>
    public Transform ActorAutoSkillTransform;   //where autoskills come from (not where they're facing)

    /// <summary>
    /// Magic number for Actor rotation speed.
    /// </summary>
    public float RotationSpeed;

    /// <summary>
    /// Magic number for Actor movement speed.
    /// </summary>
    public float MovementSpeed;

    /// <summary>
    /// Actor Component which controls all numbers-only data related to combat.
    /// </summary>
    public CombatController CombatController { get; private set; }
    
    //ISkillUser fulfillment
    public Actor Origin { get { return this; } }
    public Transform SourceTransform { get { return transform; } }
    public AimManager AimManager { get; private set; }

    //events

    /// <summary>
    /// Called after an Actor moves voluntarity (via inputs, not physics), passing the adjusted force of their movement as a float.
    /// </summary>
    [HideInInspector] public UnityEventFloat onActorMove = new UnityEventFloat();

    /// <summary>
    /// Called after an Actor rotates, passing the amount by which they rotated as a float.
    /// </summary>
    [HideInInspector] public UnityEventFloat onActorRotate = new UnityEventFloat();

    /// <summary>
    /// The Rigidbody2D attatched to this Actor GameObject.
    /// </summary>
    protected Rigidbody2D rb;

    protected void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        CombatController = new CombatController(this);
        AimManager = new AimManager(ActorAimTransform);
    }
    private void FixedUpdate()
    {
        AlignToBounds();    //keep the user in the test box
    }
    /// <summary>
    /// Get the graphical center position of the Actor as a Vector2.
    /// </summary>
    public Vector2 GetSpritePosition()
    {
        return rb.position;
    }

    /// <summary>
    /// Instantly set the graphical center position of the Actor (this does not invoke "onActorMove").
    /// Use this to relocate an Actor as a part of scene setup.
    /// For Actor movement, use AddMovementForce().
    /// </summary>
    /// <param name="position">New world location of Actor.</param>
    public void SetSpritePosition(Vector2 position)
    {
        rb.MovePosition(position);
    }

    /// <summary>
    /// Get the graphical center position of the Actor's feet as a Vector2.
    /// </summary>
    public Vector2 GetFeetColliderWorldLocation()
    {
        return transform.TransformPoint(FeetCollider.offset);
    }

    /// <summary>
    /// Core method for moving an Actor (voluntarily or otherwise).
    /// </summary>
    /// <param name="normalizedDirection">The direction in which to move the Actor (as a Vector2 with a total length of 1).</param>
    /// <param name="force">The force to apply to the Actor. If unsure, use Const.DODGE_FORCE_VAlUE.</param>
    /// <param name="anInvoluntaryMovement">If true, binding effects and speed multipliers will not affect this movement.
    /// Use this when the movement represents physics rather than tactical movement.</param>
    public void AddMovementForce(Vector2 normalizedDirection, float force, bool anInvoluntaryMovement = false)  //only my railgun can displace it
    {
        float adjustedForce = force;
        Vector2 adjustedDirection = normalizedDirection;
        if (!anInvoluntaryMovement)
        {
            //check for CC
            CrowdControlManager ccc = CombatController.CrowdControl;

            if (ccc.MovementDisabled)
                return;

            float ccMovementMulti = ccc.GetCurrentMoveSpeedMultiplier();
            float ccMovementDirectionOffset = ccc.GetCurrentMoveOffsetAmount();

            adjustedForce = ccMovementMulti * force;
            adjustedDirection = Quaternion.AngleAxis(ccMovementDirectionOffset, Vector3.back) * normalizedDirection;
        }        

        rb.AddForce(adjustedDirection * adjustedForce * Time.fixedDeltaTime, ForceMode2D.Impulse);

        if (onActorMove != null && !anInvoluntaryMovement)
            onActorMove.Invoke(adjustedForce);      //adjustedForce or force? We can decide when there's actually a need for the callback arg
    }

    /// <summary>
    /// Core method for rotating an Actor.
    /// Actor will attempt to face the provided world coordinates, but will be limited by their maximum rotation speed.
    /// </summary>
    /// <param name="worldPoint">World position to attempt to face.</param>
    public void RotateAimToward(Vector2 worldPoint)
    {
        var desiredDeltaZ = AimManager.GetDeltaRotationToAimAtPoint(worldPoint);

        var adjustedRoation = Mathf.MoveTowards(0, desiredDeltaZ, RotationSpeed * Time.fixedDeltaTime);     //calculate how far they can rotate on this frame.

        AimManager.RotateByDegrees(adjustedRoation);

        if (onActorRotate != null)
            onActorRotate.Invoke(adjustedRoation);
    }

    /// <summary>
    /// Place the Actor at the closest world location within the game's boundary.
    /// </summary>
    public void AlignToBounds()
    {
        Boundary gameBounds = Game.Self.PlayableArea;
        if (!gameBounds.Contains(rb.position))
        {
            rb.position = gameBounds.ClosestPoint(rb.position);
        }
    }
}

/// <summary>
/// UnityEvent which supplies an Actor as a parameter.
/// </summary>
[System.Serializable] public class UnityEventActor : UnityEvent<Actor> { }

/// <summary>
/// Interface for things owned by or associated with a single Actor.
/// </summary>
public interface IActorOwned
{
    Actor Origin { get; }
}

/// <summary>
/// Interface for Actor components.
/// </summary>
public interface IActorComponent
{
    Actor ParentActor { get; }
}