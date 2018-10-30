using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    //events
    [HideInInspector] public UnityEventFloat onActorMove = new UnityEventFloat();
    [HideInInspector] public UnityEventFloat onActorRotate = new UnityEventFloat();

    public Transform AimTransform;      //can get more complicated for multiple aim transforms. We'll see.
    public float RotationSpeed;
    public float MovementSpeed;
    public CombatController CombatController { get; private set; }

    protected Rigidbody2D rb;

    protected void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        CombatController = new CombatController(this);
    }
    private void FixedUpdate()
    {
        AlignToBounds();
    }
    public Vector2 GetPosition()
    {
        return rb.position;
    }
    public void SetPosition(Vector2 position)
    {
        rb.MovePosition(position);
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
    public void RotateAim(float desiredDeltaZ)
    {
        desiredDeltaZ = Mathf.MoveTowards(0, desiredDeltaZ, RotationSpeed * Time.fixedDeltaTime);
        AimTransform.Rotate(0, 0, desiredDeltaZ);
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