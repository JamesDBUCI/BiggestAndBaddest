using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct StatScaler
{
    //combination of a stat template and an amount by which that stat is factored (for doing damage calculations)
    public StatTemplate Stat;
    public float Scale;

    public StatScaler(StatTemplate template, float scale)   //script-side construction
    {
        Stat = template;
        Scale = scale;
    }
}

[CustomPropertyDrawer(typeof(StatScaler))]
public class PD_StatScaler : PropertyDrawer
{
    SerializedProperty statProp;
    SerializedProperty scaleProp;

    float scaleRectWidth = 60;
    float formulaBoxHeight = 20;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        statProp = property.FindPropertyRelative("Stat");
        scaleProp = property.FindPropertyRelative("Scale");

        Rect secondRect = EditorGUILayout.GetControlRect();
        Rect totalRect = new Rect(position) { height = position.height + formulaBoxHeight };

        Rect scaleRect = new Rect(position) { width = scaleRectWidth };
        Rect statRect = new Rect(scaleRect.xMax, position.y, position.width - scaleRect.width, position.height);

        Rect formulaRect = new Rect(secondRect) { height = formulaBoxHeight };

        EditorGUI.BeginProperty(totalRect, label, property);

        EditorGUI.PropertyField(scaleRect, scaleProp, GUIContent.none);
        EditorGUI.PropertyField(statRect, statProp, GUIContent.none);

        StatTemplate statObject = (StatTemplate)statProp.objectReferenceValue;

        if (statObject != null)
        {
            string formula = string.Format("{0}% of {1}.", scaleProp.floatValue * 100, statObject.ExternalName);
            GUI.Box(formulaRect, formula);
            //EditorGUI.HelpBox(secondRect, formula, MessageType.None);
        }        

        EditorGUI.EndProperty();
    }
}
