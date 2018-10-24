using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Combat System/New Melee Attack")]
public class MeleeAttack : AttackType
{
    public float Range = 1;
    public int TargetCount = 1;

    protected override List<ICombatant> Attack(ICombatant combatant)
    {
        //draw a raycast with provided max range and return the enemies hit.

        //handles
        Transform projTrans = combatant.GetProjectileTransform();

        //get all objects hit by the raycast (it's usually a small ray, so we'll have 0~1 hits much of the time)
        RaycastHit2D[] hits = Physics2D.RaycastAll(projTrans.transform.position, projTrans.up, Range);

        //debug ray
        Debug.DrawRay(projTrans.transform.position, projTrans.up * Range, Color.green, AttackRate);

        //if there were no hits for some reason, just leave right here;
        if (hits == null)
            return null;

        //make a list of the objects hit
        List<RaycastHit2D> hitsList = new List<RaycastHit2D>(hits);

        //make an empty list for the ICombatants (Actors) we're going to hit
        List<ICombatant> combatantsHit = new List<ICombatant>();

        //iterate the list of raycast hits
        foreach (RaycastHit2D hit in hitsList)
        {
            //try to get an Actor component off the object hit
            Actor actorComponent = hit.transform.gameObject.GetComponent<Actor>();

            //if there wasn't one, it wasn't an ICombatant, so we're not able to do damage to it (next hit object, please)
            if (actorComponent == null)
                continue;

            //stop hitting yourself
            if (actorComponent == combatant as Actor)
                continue;

            //if we got here, it was an actor and we're gonna say we hit it (enemies or allies)
            combatantsHit.Add(actorComponent);

            //if we've maxed out the number of targets this attack can hit, we stop checking hits
            if (combatantsHit.Count == TargetCount)
                break;
        }

        //Debug.Log("count = " + combatantsHit.Count);

        return combatantsHit;
    }
}

[CustomEditor(typeof(MeleeAttack))]
public class Insp_MeleeAttack : Insp_AttackType
{
    SerializedProperty rangeProp;
    SerializedProperty targetCountProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        rangeProp = serializedObject.FindProperty("Range");
        targetCountProp = serializedObject.FindProperty("TargetCount");
    }
    protected override string GetSubtypeName()
    {
        return "MeleeAttack";
    }
    protected override void AttackSubtypeField()
    {
        EditorGUILayout.PropertyField(rangeProp);
        EditorGUILayout.PropertyField(targetCountProp);
    }
}