using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public struct DamageWithType
{
    public CoreDamageTypeEnum CoreDamageType;
    public List<DamageType> SecondaryTypes;
    public float Value;

    public DamageWithType(CoreDamageTypeEnum coreDamageType, float value, params DamageType[] secondaryTypes)
    {
        CoreDamageType = coreDamageType;
        SecondaryTypes = new List<DamageType>(secondaryTypes);
        Value = value;
    }
}

//[CustomPropertyDrawer(typeof(DamageWithType))]
//public class KPD_DamageWithType : KPropertyDrawer
//{
//    private List<PropertyDataSet> _localList = new List<PropertyDataSet>();
//    protected override List<PropertyDataSet> RegisterLocalList()
//    {
//        return _localList;
//    }
//    protected override void RegisterProps()
//    {
//        RegisterProp("Value", new GUIContent("Value", "The amount of damage or healing in this node."));
//        RegisterProp("CoreDamageType", new GUIContent("Core Damage Type", "What is the primary type of damage/healing being done?"));
//        RegisterProp("SecondaryTypes",
//            new GUIContent("Secondary Types", "What are the secondary types of damage/healing being done? "),
//            new PropertyFieldDisplayType.ListMini(false));
//    }
//}