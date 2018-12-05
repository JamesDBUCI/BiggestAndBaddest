using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour, /*Skill.ISkillUser*/ Skill.ISkillSource
{
    public ActorFaction Faction { get; set; }

    public Collider2D FeetCollider;     //Where Auras and other "ground"-based things are detected (ground in a 2D world)

    public Transform ActorAimTransform;      //can get more complicated for multiple aim transforms. We'll see.
    public Transform ActorAuraTransform;     //where auras come from
    public Transform ActorAutoSkillTransform;   //where autoskills come from (not where they're facing)

    public float RotationSpeed;
    public float MovementSpeed;
    public CombatController CombatController { get; private set; }
    
    //ISkillUser fulfillment
    public Actor Origin { get { return this; } }
    public Transform SourceTransform { get { return transform; } }
    public AimManager AimManager { get; private set; }
    //public DirectModifierManager DirectModifierManager { get { return CombatController.DirectModifiers; } }
    //public CrowdControlManager CrowdControlManager { get { return CombatController.CrowdControl; } }
    //public AuraManager AuraManager { get { return CombatController.Auras; } }
    //public AutoSkillManager AutoSkillManager { get { return CombatController.AutoSkills; } }

    //events
    [HideInInspector] public UnityEventFloat onActorMove = new UnityEventFloat();
    [HideInInspector] public UnityEventFloat onActorRotate = new UnityEventFloat();

    protected Rigidbody2D rb;

    protected void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        CombatController = new CombatController(this);
        AimManager = new AimManager(ActorAimTransform);
    }
    private void FixedUpdate()
    {
        AlignToBounds();
    }
    public Vector2 GetSpritePosition()
    {
        return rb.position;
    }
    public void SetSpritePosition(Vector2 position)
    {
        rb.MovePosition(position);
    }
    public Vector2 GetFeetColliderWorldLocation()
    {
        return transform.TransformPoint(FeetCollider.offset);
    }
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

    public void RotateAimToward(Vector2 worldPoint)
    {
        //desiredDeltaZ = Mathf.MoveTowards(0, desiredDeltaZ, RotationSpeed * Time.fixedDeltaTime);
        //AimTransform.Rotate(0, 0, desiredDeltaZ);
        var desiredDeltaZ = AimManager.GetDeltaRotationToAimAtPoint(worldPoint);
        AimManager.RotateByDegrees(Mathf.MoveTowards(0, desiredDeltaZ, RotationSpeed * Time.fixedDeltaTime));
    }

    public void AlignToBounds()
    {
        Boundary gameBounds = Game.Self.PlayableArea;
        if (!gameBounds.Contains(rb.position))
        {
            rb.position = gameBounds.ClosestPoint(rb.position);
        }
    }
}
[System.Serializable] public class UnityEventActor : UnityEvent<Actor> { }
public interface IActorOwned
{
    Actor Origin { get; }
}
public interface IActorComponent
{
    Actor ParentActor { get; }
}