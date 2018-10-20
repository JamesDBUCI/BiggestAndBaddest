using UnityEditor;
using UnityEngine;

[System.Serializable]
public class OptionalInt
{
    public int Value = 0;
    public bool Enabled = true;
    public object ValueIfEnabled { get { return Enabled ? (object)Value : (object)Enabled; } }

    public OptionalInt() { }
    public OptionalInt(OptionalInt blueprint)
    {
        Value = blueprint.Value;
        Enabled = blueprint.Enabled;
    }
    public OptionalInt(int value)
    {
        Value = value;
    }
    public OptionalInt(bool option)
    {
        Enabled = false;
    }
    public OptionalInt(int value, bool enabled)
    {
        Value = value; Enabled = enabled;
    }
    public OptionalInt(int? nullableInt)
    {
        if (nullableInt.HasValue)
        {
            Value = nullableInt.Value;
        }
        else
        {
            Enabled = false;
        }
    }
    public OptionalInt Clone()
    {
        return new OptionalInt(Value, Enabled);
    }
    public int? ToNullableInt()
    {
        return Enabled ? Value : (int?)null;
    }
}

[CustomPropertyDrawer(typeof(OptionalInt))]
public class PD_OptionalInt : PropertyDrawer
{
    //draws OptionalInt objects in the unity inspector

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

        EditorGUI.PropertyField(buttonPos, boolProp, GUIContent.none);

        EditorGUI.BeginDisabledGroup(!boolProp.boolValue);
        EditorGUI.PropertyField(fieldPos, intProp, GUIContent.none);
        EditorGUI.EndDisabledGroup();
    }
}