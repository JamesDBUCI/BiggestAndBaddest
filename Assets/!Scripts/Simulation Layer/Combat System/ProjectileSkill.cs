using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Skill/Projectile Skill")]
public class ProjectileSkill : Skill
{
    public ProjectileController ProjectilePrefab;

    public override AttackType GetAttackType()
    {
        return AttackType.Projectile;
    }
}

//[CustomEditor(typeof(ProjectileSkill))]
public class Insp_ProjectileSkill : Insp_Skill
{
    SerializedProperty prefabProp;
    readonly GUIContent prefabCont = new GUIContent("Projectile Prefab", "The prefab of the Projectile that will be fired by this Skill.");

    protected override string GetSubtypeName()
    {
        return "Projectile";
    }
    protected override void AdditionalOnEnable()
    {
        prefabProp = serializedObject.FindProperty("ProjectilePrefab");
    }
    protected override void AdditionalOnInspectorGUI()
    {
        EditorGUILayout.PropertyField(prefabProp, prefabCont);
    }
}