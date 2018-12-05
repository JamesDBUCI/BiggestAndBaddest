using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Combat Class")]
public class ActorClass : ScriptableObject
{
    //name
    public string ExternalName;
    //armor type profficiencies
    //graphics
    //passive(s)
    //V.O. pack?

    //stats
    public List<ActorClassStatTemplate> Stats = new List<ActorClassStatTemplate>()
    {
        new ActorClassStatTemplate(Const.MAXHP_SCALE_STAT, "Constitution"),
        new ActorClassStatTemplate(Const.PHYSICAL_SCALE_STAT, "Might"),
        new ActorClassStatTemplate(Const.MAGIC_SCALE_STAT, "Guile"),
        new ActorClassStatTemplate(Const.PHYSICAL_RESIST_STAT, "Armor"),
        new ActorClassStatTemplate(Const.MAGIC_RESIST_STAT, "Spell Resistance"),
    };

    //modifier immunities
    public List<DirectModifierTemplate> ModifierImmunity = new List<DirectModifierTemplate>();
}

[System.Serializable]
public struct ActorClassStatTemplate
{
    [HideInInspector] public string InternalName;
    public string ExternalName;
    public float BaseValue;
    public float ValuePerLevel;

    public ActorClassStatTemplate(string internalName, string externalName)
    {
        InternalName = internalName;
        ExternalName = externalName;
        BaseValue = 0;
        ValuePerLevel = 0;
    }
}

[CustomPropertyDrawer(typeof(ActorClass))]
public class Insp_ActorClass : Editor
{
    SerializedProperty nameProp;
    SerializedProperty statListProp;
    SerializedProperty modifierProp;

    private void OnEnable()
    {
        nameProp = serializedObject.FindProperty("ExternalName");
        statListProp = serializedObject.FindProperty("Stats");
        modifierProp = serializedObject.FindProperty("ModifierImmunity");
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
        KEditorTools.ListMini(modifierProp, new GUIContent("Modifier Immunity"), true);

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