﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Combat Class")]
public class CombatClass : ScriptableObject
{
    //name
    public string ExternalName;
    //armor type profficiencies
    //graphics
    //passive(s)
    //V.O. pack?

    //stats
    public List<CombatClassStatTemplate> Stats = new List<CombatClassStatTemplate>()
    {
        new CombatClassStatTemplate(Const.MAXHP_SCALE_STAT, "Constitution"),
        new CombatClassStatTemplate(Const.PHYSICAL_SCALE_STAT, "Might"),
        new CombatClassStatTemplate(Const.MAGIC_SCALE_STAT, "Guile"),
        new CombatClassStatTemplate(Const.PHYSICAL_RESIST_STAT, "Armor"),
        new CombatClassStatTemplate(Const.MAGIC_RESIST_STAT, "Spell Resistance"),
    };
}

[System.Serializable]
public struct CombatClassStatTemplate
{
    [HideInInspector] public string InternalName;
    public string ExternalName;
    public float BaseValue;
    public float ValuePerLevel;

    public CombatClassStatTemplate(string internalName, string externalName)
    {
        InternalName = internalName;
        ExternalName = externalName;
        BaseValue = 0;
        ValuePerLevel = 0;
    }
}

[CustomPropertyDrawer(typeof(CombatClass))]
public class Insp_CombatClass : Editor
{
    SerializedProperty nameProp;
    SerializedProperty statListProp;

    private void OnEnable()
    {
        nameProp = serializedObject.FindProperty("ExternalName");
        statListProp = serializedObject.FindProperty("Stats");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(nameProp);
        GUILayout.Space(8);
        EditorGUILayout.LabelField("Stat / Base Value / Value per Level", EditorStyles.miniLabel);
        foreach (SerializedProperty statProp in statListProp)
        {
            EditorGUILayout.PropertyField(statProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

//[CustomPropertyDrawer(typeof(CombatClassStatTemplate))]
//public class KPD_CombatClassStatTemplate : KPropertyDrawer
//{
//    protected List<Xuple<PropertyDataSet, Rect>> _rects = new List<Xuple<PropertyDataSet, Rect>>();
//    protected Rect RegisterRect(PropertyDataSet pds, Rect rect)
//    {
//        _rects.Add(new Xuple<PropertyDataSet, Rect>(pds, rect));
//        return rect;
//    }
//    protected Rect GetRekt(string fieldName)
//    {
//        return GetRekt(FindProp(fieldName));
//    }
//    protected Rect GetRekt(PropertyDataSet pds)
//    {
//        return _rects.Find(x => x.Value1 == pds).Value2;
//    }

//    private List<PropertyDataSet> _localList = new List<PropertyDataSet>();
//    protected override List<PropertyDataSet> RegisterLocalList()
//    {
//        return _localList;
//    }
//    protected override void RegisterProps()
//    {
//        RegisterProp("Stat", content: GUIContent.none, hideForManualDraw: true);
//        RegisterProp("BaseValue", content: GUIContent.none, hideForManualDraw: true);
//        RegisterProp("ValuePerLevel", content: GUIContent.none, hideForManualDraw: true);
//    }
//    protected override void OnGUI_BeforeProps(Rect position, SerializedProperty property, GUIContent label)
//    {
//        Rect statRect = RegisterRect(FindProp("Stat"), new Rect(position) { width = position.width - 160 });
//        Rect baseValueRect = RegisterRect(FindProp("BaseValue"), new Rect(position) { x = statRect.xMax, width = 80 });
//        Rect valuePerLevelRect = RegisterRect(FindProp("ValuePerLevel"), new Rect(position) { x = baseValueRect.xMax, width = 80 });
//    }
//    protected override void OnGUI_AfterProps(Rect position, SerializedProperty property, GUIContent label)
//    {
//        if (PropIsSafe("Stat", property))
//            EditorGUI.LabelField(GetRekt("Stat"), FindProp("Stat").Property.stringValue);
//        FindProp("BaseValue").Draw(GetRekt("BaseValue"));
//        FindProp("ValuePerLevel").Draw(GetRekt("ValuePerLevel"));
//    }
//}