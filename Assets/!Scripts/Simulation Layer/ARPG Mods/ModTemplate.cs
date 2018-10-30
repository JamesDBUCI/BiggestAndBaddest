using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class ModTemplate : ScriptableObject
{
    //Unity's editable template for a mod

    public string NameExternal;
    public AffixSlotEnum AffixSlot;
    public bool Hidden;
    public bool ModIsDisabled;
}

public abstract class Insp_ModTemplate : Editor
{
    SerializedProperty nameProp;
    SerializedProperty affixProp;
    SerializedProperty hideProp;
    SerializedProperty disabledProp;

    protected void OnEnable()
    {
        nameProp = serializedObject.FindProperty("NameExternal");
        affixProp = serializedObject.FindProperty("AffixSlot");
        hideProp = serializedObject.FindProperty("Hidden");
        disabledProp = serializedObject.FindProperty("ModIsDisabled");

        OnEnable_Late();
    }
    protected virtual void OnEnable_Late() { }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (disabledProp.boolValue)
        {
            EditorGUILayout.HelpBox("This mod is disabled. It can not be generated on any moddable until re-enabled.", MessageType.Warning);
        }
        KEditorTools.ToggleGridSingle(disabledProp, new GUIContent("Disable this Mod", "Click to toggle whether this mod can be generated on a moddable."), 120);

        GUILayout.Space(8);

        EditorGUILayout.LabelField("External Name & Affix Slot");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(nameProp, GUIContent.none);
        EditorGUILayout.PropertyField(affixProp, GUIContent.none);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(hideProp, new GUIContent("Hidden from Player", "Players will not see this mod listed, but will still be in effect."));

        OnInspectorGUI_Late();

        serializedObject.ApplyModifiedProperties();
    }
    protected virtual void OnInspectorGUI_Late() { }
}