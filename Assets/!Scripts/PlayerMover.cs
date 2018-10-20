using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    public float speed;

    Rigidbody2D rb;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        var v = Input.GetAxisRaw("Vertical");
        var h = Input.GetAxisRaw("Horizontal");

        if (h != 0 || v != 0)
        {
            rb.AddForce(new Vector3(h, v).normalized * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

        Boundary gameBounds = Game.Self.PlayableArea;
        if (!gameBounds.Contains(rb.position))
        {
            rb.position = gameBounds.ClosestPoint(rb.position);
        }
    }
}
