using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class PlayerMover : MonoBehaviour
{
    Actor _parent;

    private void Awake()
    {
        _parent = GetComponent<Actor>();
	}
	
	void FixedUpdate ()
    {
        //look
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float angleToCursor = Vector2.SignedAngle(_parent.AimTransform.up, worldMousePos - _parent.AimTransform.position);
        Debug.Log(angleToCursor);

        _parent.RotateAim(angleToCursor);

        //move
        var v = Input.GetAxisRaw("Vertical");
        var h = Input.GetAxisRaw("Horizontal");

        if (h != 0 || v != 0)
        {
            _parent.AddMovementForce(new Vector2(h, v).normalized, _parent.MovementSpeed);
        }
    }
}
