using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dummy script in place for boss AI
public class BossShooter : MonoBehaviour {

    Actor _parentActor;
    //offset time in seconds
    public float skillOffsetTime = 2.5f;

	// Use this for initialization
	void Start () {
        _parentActor = GetComponent<Actor>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    void PrepareSkill()
    {
        //Animation can play here
        //Wait for offset time
        //Channel ability
        _parentActor.CombatController.Skills.TryUse(1);
    }
}
