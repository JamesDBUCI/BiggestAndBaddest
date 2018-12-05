using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Mods/Gear Mod")]
public class GearModTemplate : ModTemplate
{
    public List<StatChangeTemplate> StatChanges = new List<StatChangeTemplate>();
    public List<StatusFlag> StatusFlags = new List<StatusFlag>();
}

[CustomEditor(typeof(GearModTemplate))]
public class Insp_GearModTemplate : Insp_ModTemplate
{
    SerializedProperty changesProp;
    SerializedProperty flagsProp;

    protected override void OnEnable_Late()
    {
        changesProp = serializedObject.FindProperty("StatChanges");
        flagsProp = serializedObject.FindProperty("StatusFlags");
    }

    protected override void OnInspectorGUI_Late()
    {
        KEditorTools.ListBig(changesProp, new GUIContent("Stat Changes"), "Stat Change", true);
        KEditorTools.ListMini(flagsProp, new GUIContent("Status Flags"), true);
    }
}