using UnityEngine;
using UnityEditor;

[System.Serializable]
public class OptionalFloat
{
    public float Value = 0;
    public bool Enabled = true;
    public object ValueIfEnabled { get { return Enabled ? (object)Value : (object)Enabled; } }

    public OptionalFloat() { }
    public OptionalFloat(OptionalFloat blueprint)
    {
        Value = blueprint.Value;
        Enabled = blueprint.Enabled;
    }
    public OptionalFloat(float value)
    {
        Value = value;
    }
    public OptionalFloat(bool option)
    {
        Enabled = false;
    }
    public OptionalFloat(float value, bool enabled)
    {
        Value = value; Enabled = enabled;
    }
    public OptionalFloat(float? nullableFloat)
    {
        if (nullableFloat.HasValue)
        {
            Value = nullableFloat.Value;
        }
        else
        {
            Enabled = false;
        }
    }
    public OptionalFloat Clone()
    {
        return new OptionalFloat(Value, Enabled);
    }
    public float? ToNullableFloat()
    {
        return Enabled ? Value : (float?)null;
    }
}

[CustomPropertyDrawer(typeof(OptionalFloat))]
public class PD_OptionalFloat : PropertyDrawer
{
    //draws OptionalFloat objects in the unity inspector

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (label != GUIContent.none)
        {
            EditorGUI.LabelField(new Rect(position.position, new Vector2(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)), label);
            position = new Rect(position.position.x + EditorGUIUtility.labelWidth, position.position.y, position.width - EditorGUIUtility.labelWidth, position.height);
        }

        SerializedProperty boolProp = property.FindPropertyRelative("Enabled");
        SerializedProperty intProp = property.FindPropertyRelative("Value");

        float toggleButtonWidth = EditorGUIUtility.singleLineHeight;

        Rect buttonPos = new Rect(position.position, new Vector2(position.height, toggleButtonWidth));
        Rect fieldPos = new Rect(position.position.x + toggleButtonWidth, position.position.y, position.width - toggleButtonWidth, position.height);

        var oldIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        //toggle button
        EditorGUI.PropertyField(buttonPos, boolProp, GUIContent.none);

        //number field
        EditorGUI.BeginDisabledGroup(!boolProp.boolValue);
        EditorGUI.PropertyField(fieldPos, intProp, GUIContent.none);
        EditorGUI.EndDisabledGroup();

        EditorGUI.indentLevel = oldIndent;

    }
}