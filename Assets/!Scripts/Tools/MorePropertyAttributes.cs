using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ToggleLeftAttribute : PropertyAttribute
{
    public ToggleLeftAttribute() { }
}
[CustomPropertyDrawer(typeof(ToggleLeftAttribute))]
public class PD_ToggleLeftAttribute : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        property.boolValue = EditorGUI.ToggleLeft(position, label, property.boolValue);
    }
}
public class ListBigAttribute : PropertyAttribute
{
    public string ContentType;
    public bool CanBeEmpty;

    public ListBigAttribute(string contentType, bool canBeEmpty)
    {
        ContentType = contentType;
        CanBeEmpty = canBeEmpty;
    }
}
[CustomPropertyDrawer(typeof(ListBigAttribute))]
public class PD_ListBigAttribute : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        var att = (ListBigAttribute)attribute;
        KEditorTools.ListBig(property, label, att.ContentType, att.CanBeEmpty);
    }
}
