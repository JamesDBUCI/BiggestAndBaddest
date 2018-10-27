using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Skill/Melee Skill")]
public class MeleeSkill : Skill
{
    public float MeleeRange;

    public override AttackType GetAttackType()
    {
        return AttackType.Melee;
    }
}

//[CustomEditor(typeof(MeleeSkill))]
public class Insp_MeleeSkill : Insp_Skill
{
    SerializedProperty rangeProp;
    readonly GUIContent rangeCont = new GUIContent("Melee Range", "How far will the melee attack reach?");

    protected override string GetSubtypeName()
    {
        return "Melee";
    }
    protected override void AdditionalOnEnable()
    {
        rangeProp = serializedObject.FindProperty("MeleeRange");
    }
    protected override void AdditionalOnInspectorGUI()
    {
        rangeProp.floatValue = Mathf.Max(EditorGUILayout.FloatField(rangeCont, rangeProp.floatValue), 0);
    }
}
