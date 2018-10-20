using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class StatChangeTemplate
{
    //public string StatInternalName = "health";
    public StatTemplate AffectedStat;       //draggy-drop in Unity
    public StatChangeTypeEnum ChangeType = StatChangeTypeEnum.PLUS;
    public float MinValue = 1;
    public float MaxValue = 10;
    public float Precision = 1;
}

public class StatChange
{
    //generated from template, not modified in Unity

    public readonly string StatInternalName;
    public readonly StatChangeTypeEnum ChangeType;
    public readonly float Value;

    public StatChange(string statInternalName, StatChangeTypeEnum enumValue, float value)
    {
        StatInternalName = statInternalName;
        ChangeType = enumValue;
        Value = value;
    }
    public StatChange(StatChangeTemplate template, float value)
    {
        if (template == null)
            StatInternalName = "Undefined Stat";
        else
            StatInternalName = template.AffectedStat.name;

        ChangeType = template.ChangeType;
        Value = value;
    }
}

[CustomPropertyDrawer(typeof(StatChangeTemplate))]
public class PD_StatChangeTemplate : PropertyDrawer
{
    SerializedProperty statProp;
    SerializedProperty changeTypeProp;
    SerializedProperty minProp;
    SerializedProperty maxProp;
    SerializedProperty precProp;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);

        //get values
        statProp = property.FindPropertyRelative("AffectedStat");
        changeTypeProp = property.FindPropertyRelative("ChangeType");
        minProp = property.FindPropertyRelative("MinValue");
        maxProp = property.FindPropertyRelative("MaxValue");
        precProp = property.FindPropertyRelative("Precision");

        //calculate positions. Diagram below:
        //changeType stat
        //min ~ max prec
        Rect secondRect = EditorGUILayout.GetControlRect();
        Rect thirdRect = EditorGUILayout.GetControlRect();
        Rect totalPosition = new Rect(position.x, position.y, position.width, position.height + secondRect.height + thirdRect.height);
        
        Rect changeTypeRect = new Rect(position.position, new Vector2(100, position.height));
        Rect statRect = new Rect(changeTypeRect.xMax, position.y, position.width - changeTypeRect.width, position.height);

        float secondRowWidth = secondRect.width / 3;

        Rect minRect = new Rect(secondRect.x, secondRect.y, secondRowWidth, secondRect.height);
        Rect maxRect = new Rect(minRect.xMax, secondRect.y, secondRowWidth, secondRect.height);
        Rect precRect = new Rect(maxRect.xMax, secondRect.y, secondRowWidth, secondRect.height);

        EditorGUI.BeginProperty(totalPosition, label, property);

        //first row content
        EditorGUI.PropertyField(changeTypeRect, changeTypeProp, GUIContent.none);
        EditorGUI.PropertyField(statRect, statProp, GUIContent.none);

        //second row content
        float defaultLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 80;

        EditorGUI.PropertyField(minRect, minProp, new GUIContent("Min:", "Minimum value that can be rolled."));
        EditorGUI.PropertyField(maxRect, maxProp, new GUIContent("Max:", "Maximum value that can be rolled."));
        EditorGUI.PropertyField(precRect, precProp, new GUIContent("Precision:", "Define how random rolls snap to closest values. 1 = integers only, 2 = even numbers, 0.5 = half-values only, 0.1 = tenths place precision, etc...\n0 = divide by zero error you silly fool"));

        EditorGUIUtility.labelWidth = defaultLabelWidth;

        //third row content
        if (statProp.objectReferenceValue != null)
        {
            StatChangeTypeEnum changeTypeEnumValue = KEditorTools.GetEnumValue<StatChangeTypeEnum>(changeTypeProp);

            StatChangeType tempType;
            if (!StatChangeType.TryGet(changeTypeEnumValue, out tempType))
                return;     //shouldn't happen at all

            StatTemplate tempStatTemplate = (StatTemplate)statProp.objectReferenceValue;
            string formattedValueString = tempType.GetFormattedValueString(minProp.floatValue, maxProp.floatValue, tempStatTemplate.ExternalName);
            string description = formattedValueString + string.Format(" (Precision: {0})", precProp.floatValue);

            EditorGUI.HelpBox(thirdRect, description, MessageType.None);
        }        

        EditorGUI.EndProperty();
    }
}
