using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dummy script in place for boss AI
public class AttackManager : MonoBehaviour {

    Actor _parentActor;
    //offset time in seconds
    public float skillOffsetTime = 2.5f;
    public float skillWait;

    public int skill = 0;
    private void OnEnable()
    {
        _parentActor = GetComponent<Actor>();
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(PrepareSkill(skill));
	}
    IEnumerator PrepareSkill(int slotIndex)
    {
        //Animation can play here
        //Wait for offset time
        yield return new WaitForSeconds(skillOffsetTime);
        //Channel ability
        _parentActor.CombatController.Skills.TryUse(slotIndex);
        yield return new WaitForSeconds(skillWait);
    }
}
