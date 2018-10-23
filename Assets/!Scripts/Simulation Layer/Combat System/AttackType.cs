using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class AttackType : ScriptableObject
{
    //prescribes the data needed for a player to attack in a particular way

    //aoe from origin
    //projectile

    public string ExternalName;
    public float AttackRate = 0.3f;
    //public GameObject AttackEffect;   //particle or graphical effect object (later)

    public List<StatScaler> StatScaling = new List<StatScaler>();   //amount by which stats are factored when calculating damage for this attack (0.5 = half the stat value)
    public List<DamageWithType> DamagePotency = new List<DamageWithType>();

    public virtual bool TryAttack(ICombatant combatant)
    {
        //attack timing control. this is temporary

        //handle
        CombatController cc = combatant.GetCombatController();

        if (Time.time < cc.LastAttackTimestamp + AttackRate)
            return false;

        Attack(combatant);
        cc.LastAttackTimestamp = Time.time;
        return true;
    }

    protected virtual DamagePacket GetDamagePacket(ICombatant combatant)
    {
        return CombatServices.ConstructDamagePacket(combatant, this);
    }
    protected abstract List<ICombatant> Attack(ICombatant combatant);       //return list of affected combatants (including allies)
}

//[CustomEditor(typeof(AttackType))]
public abstract class Insp_AttackType : Editor
{
    SerializedProperty nameProp;
    SerializedProperty rateProp;
    SerializedProperty scaleListProp;
    SerializedProperty damageListProp;

    protected virtual void OnEnable()
    {
        nameProp = serializedObject.FindProperty("ExternalName");
        rateProp = serializedObject.FindProperty("AttackRate");
        scaleListProp = serializedObject.FindProperty("StatScaling");
        damageListProp = serializedObject.FindProperty("DamagePotency");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        SectionHeading("Generic AttackType data");

        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(rateProp);
        //EditorGUILayout.PropertyField(scaleListProp);
        KEditorTools.ListBig(scaleListProp, new GUIContent("Scaled Stats", "List of all stats this attack scales on and by how much."), "Scaled Stat", false);
        //EditorGUILayout.PropertyField(damageListProp);
        KEditorTools.ListBig(damageListProp, new GUIContent("Damage Potency", "List of all types of damage per hit and what potency of stat scaling."), "Potency", true);

        GUILayout.Space(8);

        SectionHeading(GetSubtypeName() + " data");
        AttackSubtypeField();

        serializedObject.ApplyModifiedProperties();
    }
    private void SectionHeading(string contentName)
    {
        EditorGUILayout.LabelField(contentName, EditorStyles.miniLabel);
    }

    protected abstract string GetSubtypeName();
    protected abstract void AttackSubtypeField();
}