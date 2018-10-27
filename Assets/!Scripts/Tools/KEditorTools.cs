using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Simplification and workarounds for common Unity Editor tasks
public static class KEditorTools
{
    //constants
    private static readonly Color SELECTED_BUTTON_COLOR = new Color(1f, 0.71875f, 0.3125f);

    //create a single-line editor list from a serialized array property
    public static bool ListMini(SerializedProperty listProp, GUIContent label, bool canBeEmpty)
    {
        if (listProp == null)
            return false;

        //if the list has zero elements and it is not allowed to be empty
        if (listProp.arraySize < 1 && !canBeEmpty)
        {
            //add a new default element
            listProp.InsertArrayElementAtIndex(0);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label);      //label

        //small, "new" button for adding to the end of the list
        if (GUILayout.Button("+New", EditorStyles.miniButton, GUILayout.Width(40)))
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
        }
        EditorGUILayout.EndHorizontal();

        float xButtonWidth = 20;
        //iterate over the serialized array
        for (int i = 0; i < listProp.arraySize; i++)
        {
            EditorGUI.indentLevel++;    //indent inward
            Rect position = EditorGUILayout.GetControlRect();   //get a new rect from Unity internal system
            Rect fieldPos = new Rect(position.position, new Vector2(position.width - xButtonWidth, position.height));   //carve out a rect for the field
            Rect xButtonPos = new Rect(position.xMax - xButtonWidth, position.y, xButtonWidth, position.height);    //carve out a rect for the "x" button

            //use default field for the underlying property type
            EditorGUI.PropertyField(fieldPos, listProp.GetArrayElementAtIndex(i), GUIContent.none, true);

            //small, "x" button for deleting the corresponding entry from the list
            if (GUI.Button(xButtonPos, "x", EditorStyles.miniButton))
            {
                //check if the list is allowed to be empty
                if (listProp.arraySize > 0 || canBeEmpty)
                {
                    listProp.DeleteArrayElementAtIndex(i);  //delete the element
                }
                break;  //<-- break because we have modified the list we're iterating
            }
            EditorGUI.indentLevel--;    //indent back to starting indent level
        }
        return listProp.arraySize > 0;  //return true if there are elements in the array
    }

    //create a multi-line editor list from a serialized array property
    public static bool ListBig(SerializedProperty listProp, GUIContent label, string contentType, bool canBeEmpty)
    {
        if (!canBeEmpty && listProp.arraySize < 1)
        {
            listProp.InsertArrayElementAtIndex(0);
        }

        GUILayout.Space(8);
        if (GUILayout.Button("Add new " + contentType))
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
        }

        //iterating across the List
        for (int i = 0; i < listProp.arraySize; i++)
        {
            //GUILayout.Space(2);
            EditorGUILayout.BeginVertical(new GUIStyle("Box"));
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(string.Format("{0} #{1}:", contentType, i + 1), EditorStyles.boldLabel);
            if (GUILayout.Button("Insert Above", GUILayout.Width(85)))
            {
                listProp.InsertArrayElementAtIndex(i);
                break;
            }
            if (GUILayout.Button("Remove", GUILayout.Width(85)))
            {
                if (listProp.arraySize > 1 || canBeEmpty)
                {
                    listProp.DeleteArrayElementAtIndex(i);
                    break;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(listProp.GetArrayElementAtIndex(i), true);
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }
        return listProp.arraySize > 0;
    }
    
    #region "ToggleGrid Overloads"
    public static void ToggleGridSingle(SerializedProperty boolProp)
    {
        ToggleGridSingle(boolProp, EditorGUIUtility.currentViewWidth);
    }
    public static void ToggleGridSingle(SerializedProperty boolProp, float buttonMaxWidth)
    {
        ToggleGrid(new List<SerializedProperty>() { boolProp }, GUIContent.none, 1, buttonMaxWidth);
    }
    public static void ToggleGridSingle(SerializedProperty boolProp, GUIContent buttonContent)
    {
        ToggleGridSingle(boolProp, buttonContent, EditorGUIUtility.currentViewWidth);
    }
    public static void ToggleGridSingle(SerializedProperty boolProp, GUIContent buttonContent, float buttonMaxWidth)
    {
        ToggleGrid(new List<GUIContent>() { buttonContent }, new List<SerializedProperty>() { boolProp }, GUIContent.none, 1, buttonMaxWidth);
    }
    public static void ToggleGrid(List<SerializedProperty> boolProps, GUIContent label, int horizontalCount)
    {
        ToggleGrid(boolProps, label, horizontalCount, EditorGUIUtility.currentViewWidth);
    }
    public static void ToggleGrid(List<SerializedProperty> boolProps, GUIContent label, int horizontalCount, float buttonMaxWidth)
    {
        List<GUIContent> buttonContent = new List<GUIContent>();
        boolProps.ForEach(prop => buttonContent.Add(new GUIContent(prop.displayName)));

        ToggleGrid(buttonContent, boolProps, label, horizontalCount, buttonMaxWidth);
    }
    public static bool ToggleGrid(List<GUIContent> buttonContent, List<SerializedProperty> boolProps, GUIContent label, int horizontalCount)
    {
        return ToggleGrid(buttonContent, boolProps, label, horizontalCount, EditorGUIUtility.currentViewWidth);
    }
    #endregion

    //create a quick editor grid of boolean values (as toggled buttons)
    public static bool ToggleGrid(List<GUIContent> buttonContent, List<SerializedProperty> boolProps, GUIContent label, int horizontalCount, float buttonMaxWidth)
    {
        //returns false in the event of a content/bool mismatch

        if (buttonContent.Count != boolProps.Count)
        {
            //Debug.LogWarning("ToggleGrid reports content and bool mismatch.");
            return false;
        }

        bool hasLabel = label != GUIContent.none;
        if (hasLabel)
        {
            EditorGUILayout.LabelField(label);
        }

        float buttonSpacerH = 4;
        float buttonHeight = EditorGUIUtility.singleLineHeight;
        float expectedHeight = Mathf.Ceil((float)boolProps.Count / (float)horizontalCount);

        //this line must be here to tell Unity's Layout tracker to account for it
        Rect position = EditorGUILayout.GetControlRect(hasLabel, (buttonHeight * expectedHeight) + (buttonSpacerH * expectedHeight));

        int buttonIndex = 0;
        int h = 0;

        float buttonSpacerW = 4;
        float buttonWidth = (position.width / horizontalCount) - buttonSpacerW;

        if (buttonWidth > buttonMaxWidth)
        {
            buttonSpacerW += buttonWidth - buttonMaxWidth;
            buttonWidth = buttonMaxWidth;
        }

        while (buttonIndex < boolProps.Count)
        {
            for (int w = 0; w < horizontalCount; w++)
            {
                if (buttonIndex >= boolProps.Count)
                    continue;

                Rect buttonRect = new Rect(
                    position.x + (buttonWidth * w) + (buttonSpacerW * w) + (buttonSpacerW / 2),
                    position.y + (buttonHeight * h) + (buttonSpacerH * h) + (buttonSpacerH / 2),
                    buttonWidth,
                    buttonHeight);

                ToggleGrid_Button(buttonRect, buttonContent[buttonIndex], boolProps[buttonIndex]);
                buttonIndex++;
            }
            h++;
        }
        return true;
    }
    private static void ToggleGrid_Button(Rect position, GUIContent buttonContent, SerializedProperty boolProp)
    {
        if (boolProp == null)
            return;

        Color originalGUIColor = GUI.backgroundColor;
        if (boolProp.boolValue == true)
        {
            //nice muted orange
            //float r = 255f / 255f;
            //float g = 183f / 255f;
            //float b = 79f / 255f;
            GUI.backgroundColor = SELECTED_BUTTON_COLOR;
        }
        if (GUI.Button(position, new GUIContent(buttonContent.text + (boolProp.boolValue ? " ✓" : ""), buttonContent.tooltip + (boolProp.boolValue ? "\n[TRUE]" : "\n[FALSE]")), EditorStyles.miniButton))
        {
            boolProp.boolValue = !boolProp.boolValue;
        }
        GUI.backgroundColor = originalGUIColor;

    }

    //make a label field for a property if it needs one
    public static bool TryLabelField(Rect controlRect, GUIContent label, bool boldLabel)
    {
        bool hasLabel = label != GUIContent.none;
        if (hasLabel)
        {
            EditorGUI.LabelField(controlRect, label, boldLabel ? EditorStyles.boldLabel : EditorStyles.label);
        }
        return hasLabel;
    }

    //create a quick property field for a Sprite which displays a thumbnail of that sprite when filled
    public static bool SpriteFieldWithPreview(SerializedProperty spriteProp, GUIContent label, int previewWidth, int previewHeight, bool centered)
    {
        if (spriteProp == null)
        {
            Debug.Log("Called SpriteFieldWithPreview with null SerializedProperty.");
            return false;
        }
        Sprite sp = null;
        EditorGUILayout.PropertyField(spriteProp, label);
        if (spriteProp.objectReferenceValue != null)
        {
            sp = (Sprite)spriteProp.objectReferenceValue;
            Rect lowerRect = EditorGUILayout.GetControlRect(false, previewHeight);

            float offset = 0;
            if (centered)
                offset = (EditorGUIUtility.currentViewWidth / 2) - (previewWidth / 2);

            Rect iconRect = new Rect(lowerRect.position.x + offset, lowerRect.y, previewWidth, previewHeight);

            GUI.Box(iconRect, sp.texture);
        }
        return sp != null;
    }
    
    #region "DividerLine Overloads"
    public static void DividerLine()
    {
        DividerLine(EditorGUILayout.GetControlRect());
    }
    public static void DividerLine(float length, bool centered)
    {
        Rect rect = EditorGUILayout.GetControlRect();
        rect.width = length;
        rect.x += centered ? (length / 2) : 0;
        DividerLine(rect);
    }
    #endregion

    //create a quick editor line, typically for seperating inspector elements
    public static void DividerLine(Rect rect)
    {
        EditorGUI.LabelField(rect, GUIContent.none, GUI.skin.horizontalSlider);
    }

    //create a property field for an integer value which displays a percent sign (%) before or after the field
    public static int IntPercentField(Rect position, GUIContent label, int value, bool labelOnLeft)
    {
        bool hasLabel = label != GUIContent.none;

        float symbolSize = 40;
        float fieldSize = 50;
        float labelSize = hasLabel ? EditorGUIUtility.labelWidth : 0;

        Rect
            labelRect = new Rect(position) { width = labelSize },
            fieldRect = new Rect(position) { width = fieldSize },
            symbolRect = new Rect(position) { width = symbolSize };

        if (labelOnLeft)
        {
            fieldRect.x = labelRect.xMax;
            symbolRect.x = fieldRect.xMax - 15;
        }
        else
        {
            symbolRect.x = fieldRect.xMax - 15;
            labelRect.x = symbolRect.xMax;
        }

        if (hasLabel)
            EditorGUI.LabelField(labelRect, label);
        EditorGUI.LabelField(symbolRect, "%");
        return EditorGUI.IntField(fieldRect, value);
    }

    //get the specific enum value from a serialized enum property (only works with unbroken integer spaced enums (values 0, 1, 2, 3, 4, etc...))
    //MyEnum myValue = GetEnumValue<MyEnum>(SerializedProperty enumProperty)    //myValue: MyEnum.MY_VALUE
    public static EnumType GetEnumValue<EnumType>(SerializedProperty enumProp)
    {
        if (enumProp.propertyType != SerializedPropertyType.Enum)
        {
            Debug.LogException(new System.ArgumentException("Unable to extract Enum value from non-enum type.", "enumProp"));
            return default(EnumType);
        }
        return (EnumType)System.Enum.GetValues(typeof(EnumType)).GetValue(enumProp.enumValueIndex);
    }

    //create a GUIContent object from a serialized GUIContent instance (for when you want to keep GUIContent data as fields in the class itself)
    public static GUIContent RebuildGUIContent(SerializedProperty guiContentProp)
    {
        if (guiContentProp == null)
            return GUIContent.none;

        string text = guiContentProp.FindPropertyRelative("text").stringValue ?? "";
        string tool = guiContentProp.FindPropertyRelative("tooltip").stringValue ?? "";
        Texture image = (Texture)guiContentProp.FindPropertyRelative("image").objectReferenceValue ?? default(Texture);

        return new GUIContent(text, image, tool);
    }
}

[System.Serializable]
public sealed class EditorPageFlipper
{
    public SerializedProperty List { get; private set; }
    public GUIContent HeaderInfo { get; private set; }
    public System.Func<SerializedProperty, int, string> PageTitleRetriever { get; private set; }

    public int SelectedPage { get; private set; }

    public EditorPageFlipper(SerializedProperty listProp, GUIContent headerInfo = null, System.Func<SerializedProperty, int, string> pageTitleRetriever = null)
    {
        List = listProp;
        HeaderInfo = headerInfo ?? GUIContent.none;
        PageTitleRetriever = pageTitleRetriever;
        SelectedPage = 0;
    }
    public bool Draw()
    {
        if (List == null)
            return false;

        if (List.arraySize == 0)
        {
            List.InsertArrayElementAtIndex(0);
        }

        //header
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(HeaderInfo, EditorStyles.boldLabel);
        if (GUILayout.Button("Insert", EditorStyles.miniButtonLeft, GUILayout.Width(70)))
        {
            List.InsertArrayElementAtIndex(List.arraySize);
            return true;
        }
        if (GUILayout.Button("Remove", EditorStyles.miniButtonRight, GUILayout.Width(80)))
        {
            if (List.arraySize > 1)
            {
                List.DeleteArrayElementAtIndex(SelectedPage);
                SelectedPage = Mathf.Max(0, SelectedPage - 1);
                return true;
            }
        }
        EditorGUILayout.EndHorizontal();

        SelectedPage = EditorGUILayout.IntSlider(SelectedPage, 0, List.arraySize - 1);
        GUILayout.Space(8);

        //body
        return DrawPage(SelectedPage);
    }
    private bool DrawPage(int index)
    {
        if (index > List.arraySize)
            return false;

        SerializedProperty page = List.GetArrayElementAtIndex(index);

        if (page == null)
            return false;

        if (PageTitleRetriever != null)
            EditorGUILayout.LabelField(PageTitleRetriever(page, index), EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(page, true);
        return true;
    }
}
public class PropertyDataSet
{
    private const string KPD_ERROR_MAIN = "KEditorTools Error:";
    private static void LogError(string message)
    {
        Debug.LogError(KPD_ERROR_MAIN + " " + message);
    }

    public SerializedProperty Property { get; private set; }
    public GUIContent Content { get; private set; }
    public string FieldName { get; private set; }

    public PropertyFieldDisplayType DisplayType { get; private set; }
    public bool HideForManualDraw { get; private set; }

    public PropertyDataSet(string fieldName, GUIContent content = null, PropertyFieldDisplayType displayType = null, bool hideForManualDraw = false)
    {
        Property = null;
        FieldName = fieldName;
        DisplayType = displayType ?? PropertyFieldDisplayType.Basic;
        Content = content;
        HideForManualDraw = hideForManualDraw;
    }
    public void UpdateProp(SerializedProperty coreProp)
    {
        if (coreProp == null)
        {
            LogError("UpdateProp() failed because the core property was null.");
            return;
        }

        //Debug.Log(coreProperty.type + " " + coreProperty.CountInProperty());
        Property = coreProp.FindPropertyRelative(FieldName);

        if (Property == null)
        {
            LogError("UpdateProp() failed because a sub-property with field name \"" + FieldName + "\" was not found.");
            return;
        }

        if (Content == null)
            Content = GetDefaultContent();
    }
    //public SerializedProperty GetProp(SerializedProperty coreProp)
    //{
    //    return coreProp.FindPropertyRelative(FieldName);
    //}
    public GUIContent GetDefaultContent()
    {
        if (Property == null)
        {
            LogError("GetDefaultContent() failed because Property was null.");
            return GUIContent.none;
        }
        return new GUIContent(Property.displayName, Property.tooltip);
    }
    public void Draw()
    {
        if (Property == null)
        {
            LogError("Draw() failed because Property was null.");
            return;
        }
        DisplayType.Display(Property, this);
        //DisplayType.Display(GetProp(coreProp), this);
    }
    public void Draw(Rect rect)
    {
        DisplayType.Display(Property, this, rect);
    }
}
public class PropertyFieldDisplayType
{
    //contains info about displaying PropertyDrawer fields

    //static instances
    public static PropertyFieldDisplayType Basic =
        new PropertyFieldDisplayType((prop, pds) => EditorGUILayout.PropertyField(prop, pds.Content),
                                     (prop, pds, rect) => EditorGUI.PropertyField(rect, prop, pds.Content));

    //instance members
    public readonly System.Action<SerializedProperty, PropertyDataSet> DisplayLayout;
    public readonly System.Action<SerializedProperty, PropertyDataSet, Rect> DisplayManual;

    public PropertyFieldDisplayType(System.Action<SerializedProperty,
        PropertyDataSet> layoutDisplayMethod,
        System.Action<SerializedProperty, PropertyDataSet, Rect> manualDisplayMethod)
    {
        DisplayLayout = layoutDisplayMethod;
        DisplayManual = manualDisplayMethod;
    }
    public void Display(SerializedProperty property, PropertyDataSet pds)
    {
        if (DisplayLayout != null)
            DisplayLayout(property, pds);
    }
    public void Display(SerializedProperty property, PropertyDataSet pds, Rect rect)
    {
        if (DisplayManual != null)
            DisplayManual(property, pds, rect);
    }

    public class ListBig : PropertyFieldDisplayType
    {
        public ListBig(bool canBeEmpty)
            : this("", canBeEmpty) { }
        public ListBig(string contentType, bool canBeEmpty)
            : base((prop, pds) => KEditorTools.ListBig(prop, pds.Content, contentType, canBeEmpty),     //layout
                    null) { }                                                                           //no manual version
    }
    public class ListMini : PropertyFieldDisplayType
    {
        public ListMini(bool canBeEmpty)
            : base((prop, pds) => KEditorTools.ListMini(prop, pds.Content, canBeEmpty),                 //layout
                    null) { }                                                                           //no manual version
    }
}