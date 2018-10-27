using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

//public abstract class KPropertyDrawer : PropertyDrawer
//{
//    protected Dictionary<string, List<PropertyDataSet>> _registry = new Dictionary<string, List<PropertyDataSet>>();
//    protected List<PropertyDataSet> GetLocalList(string key)
//    {
//        List<PropertyDataSet> list;
//        _registry.TryGetValue(key, out list);
//        return list;
//    }

//    protected abstract void RegisterLocalList();

//    protected void RegisterProp(string fieldName, GUIContent content = null, PropertyFieldDisplayType displayType = null, bool hideForManualDraw = false)
//    {
//        RegisterProp(new PropertyDataSet(fieldName, content, displayType, hideForManualDraw));
//    }
//    protected void RegisterProp(PropertyDataSet propertyDataSet)
//    {
//        //Debug.Log("Registered new PDS: " + propertyDataSet.FieldName + " (#" + _props.Count + ")");
//        RegisterLocalList().Add(propertyDataSet);
//    }

//    protected bool PropExists(string fieldName)
//    {
//        return RegisterLocalList().Exists(p => p.FieldName == fieldName);
//    }
//    protected bool PropIsSafe(string fieldName, SerializedProperty coreProp)
//    {
//        var prop = FindProp(fieldName);
//        if (prop != null)
//            return prop.Property != null;
//        return false;
//    }
//    protected IEnumerable<PropertyDataSet> PropsWhere(System.Func<PropertyDataSet, bool> match)
//    {
//        return RegisterLocalList().Where(match);
//    }
//    protected PropertyDataSet FindProp(string fieldName)
//    {
//        return RegisterLocalList().Find(p => p.FieldName == fieldName);
//    }
//    protected void ForEachProp(System.Action<PropertyDataSet> actionToPerform)
//    {
//        foreach (var pds in RegisterLocalList())
//        {
//            actionToPerform(pds);
//        }
//    }

//    public KPropertyDrawer(string localKey)
//    {
//        RegisterProps(localKey);
//    }

//    private void RegisterProps(string localKey)
//    {
//        RegisterProps();
//    }
//    protected abstract void RegisterProps();

//    protected virtual void OnGUI_Top(Rect position, SerializedProperty property, GUIContent label) { }
//    protected virtual void OnGUI_BeforeProps(Rect position, SerializedProperty property, GUIContent label) { }
//    protected virtual void OnGUI_AfterProps(Rect position, SerializedProperty property, GUIContent label) { }
//    protected virtual void OnGUI_Bottom(Rect position, SerializedProperty property, GUIContent label) { }

//    public override void OnGUI(Rect position, SerializedProperty coreProperty, GUIContent label)
//    {
//        //Debug.Log("Updating all props. core prop is " + coreProperty.name);
//        foreach (var item in GetLocalList())
//        {
//            item.UpdateProp(coreProperty);
//        }

//        OnGUI_Top(position, coreProperty, label);

//        EditorGUI.BeginProperty(position, label, coreProperty);

//        OnGUI_BeforeProps(position, coreProperty, label);

//        //props
//        //Debug.Log("drawing all props. core prop is " + coreProperty.type);
//        PropsWhere(pds => !pds.HideForManualDraw).ToList().ForEach(pds => pds.Draw());

//        OnGUI_AfterProps(position, coreProperty, label);

//        EditorGUI.EndProperty();

//        OnGUI_Bottom(position, coreProperty, label);
//    }
//}
