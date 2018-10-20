using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Mod System/New ModTemplate")]
public class ModTemplate : ScriptableObject
{
    //Unity's editable template for a mod

    public string NameExternal;
    public AffixSlotEnum AffixSlot;
    public bool Hidden;
    public List<StatChangeTemplate> StatChanges;
    public List<StatFlag> Flags;
    public bool ModIsDisabled;
}

[CustomEditor(typeof(ModTemplate))]
public class Insp_ModTemplate : Editor
{
    SerializedProperty nameProp;
    SerializedProperty affixProp;
    SerializedProperty hideProp;
    SerializedProperty changesListProp;
    SerializedProperty flagsListProp;
    SerializedProperty disabledProp;

    private void OnEnable()
    {
        nameProp = serializedObject.FindProperty("NameExternal");
        affixProp = serializedObject.FindProperty("AffixSlot");
        hideProp = serializedObject.FindProperty("Hidden");
        changesListProp = serializedObject.FindProperty("StatChanges");
        flagsListProp = serializedObject.FindProperty("Flags");
        disabledProp = serializedObject.FindProperty("ModIsDisabled");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

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
        KEditorTools.ListBig(changesListProp, new GUIContent("Stat Changes", "What stats will be affected and how?"), "Stat Change", true);
        KEditorTools.ListMini(flagsListProp, new GUIContent("Stat Flags", "Which flags are set to \"true\" by this mod?"), true);

        serializedObject.ApplyModifiedProperties();
    }
}