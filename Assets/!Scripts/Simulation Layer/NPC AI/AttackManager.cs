using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Dummy script in place for boss AI
public class AttackManager : MonoBehaviour {

    Actor _parentActor;
    //offset time in seconds
    public float SkillOffsetTime = 2.5f;
    public float SkillWait;

    public bool IsPreparing = false;
    public int Skill = 0;
    private void OnEnable()
    {
        _parentActor = GetComponent<Actor>();
    }
    // Use this for initialization
    void Start () {
        StartCoroutine(PrepareSkill(Skill));
	}
    IEnumerator PrepareSkill(int slotIndex)
    {
        //repeat indefinitely
        while (true)
        {
            IsPreparing = true;
            //Wait for offset time
            Debug.Log("Prepare Skill Start");
            yield return new WaitForSeconds(SkillOffsetTime);
            //Channel ability
            _parentActor.CombatController.Skills.TryUse(GearSlotEnum.HEAD);
            IsPreparing = false;
            Debug.Log("Now Waiting");
            yield return new WaitForSeconds(SkillWait);
        }
    }
}
