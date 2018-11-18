using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dummy script in place for boss AI
public class BossShooter : MonoBehaviour {

    Actor _parentActor;

	// Use this for initialization
	void Start () {
        _parentActor = GetComponent<Actor>();
	}
	
	// Update is called once per frame
	void Update () {
        //shoot at a random interval
        if(Time.time % 10 == 0)
        {
            _parentActor.CombatController.Skills.TryUse(1);
        }
	}
}
