using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Damage Type")]
public class DamageType : ScriptableObject
{
    public string ExternalName;
    public StatScaler ResistanceStat;
}

[CustomEditor(typeof(DamageType))]
public class Insp_DamageType : Editor
{
    SerializedProperty nameProp;
    SerializedProperty resistProp;

    private void OnEnable()
    {
        nameProp = serializedObject.FindProperty("ExternalName");
        resistProp = serializedObject.FindProperty("ResistanceStat");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(nameProp);

        GUILayout.Space(8);

        EditorGUILayout.LabelField(new GUIContent("Resistance Stat and Scale Factor", "The combat stat which reduces incoming damage of this type, and the amount by which that stat is factored.\n" +
                                                                     "[56 resistance stat * 0.5 scale = 28 resistance]"), EditorStyles.miniLabel);
        EditorGUILayout.PropertyField(resistProp, GUIContent.none);

        serializedObject.ApplyModifiedProperties();
    }
}