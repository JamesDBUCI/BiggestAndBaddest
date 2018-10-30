using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum SkillPhaseTimingEnum
{
    ON_COMFIRM_USE, ON_CHANNEL_COMPLETE, ON_CHANNEL_INTERRUPT, ON_MAIN_HIT
}
[System.Serializable]
public class SkillPhase
{
    public SkillPhaseTimingEnum SkillPhaseTiming = SkillPhaseTimingEnum.ON_MAIN_HIT;
    public List<AttackEffectEnum> SkillEffects = new List<AttackEffectEnum>();
    public List<StatScaler> StatScaling = new List<StatScaler>();
    public List<DamageWithType> Potency = new List<DamageWithType>();
}

//[CustomPropertyDrawer(typeof(SkillPhase))]
//public class KPD_SkillPhase : KPropertyDrawer
//{
//    private List<PropertyDataSet> _localList = new List<PropertyDataSet>();
//    protected override List<PropertyDataSet> RegisterLocalList()
//    {
//        return _localList;
//    }
//    protected override void RegisterProps()
//    {
//        RegisterProp("SkillPhaseTiming", new GUIContent("Skill Phase Timing", "When does this Phase happen?"));
//        RegisterProp("SkillEffects", new GUIContent("Effects", "List of which effects are applied to Actors affected by this Skill."),
//            new PropertyFieldDisplayType.ListMini(false));
//        RegisterProp("StatScaling", new GUIContent("Stat Scaling", "List of which Stats this Skill's Damage and/or Healing scales on, and by how much."),
//            new PropertyFieldDisplayType.ListMini(false), true);
//        RegisterProp("Potency", new GUIContent("Damage/Healing Potency", "List of which types of Damage and/or Healing will be caused, and at what potency of stat scaling."),
//            new PropertyFieldDisplayType.ListBig("Potency", false), true);
//    }
//    protected override void OnGUI_AfterProps(Rect position, SerializedProperty property, GUIContent label)
//    {
//        var effectsListProp = FindProp("SkillEffects").Property;

//        bool effectsRequirePotency = false;
//        for (int i = 0; i < effectsListProp.arraySize; i++)
//        {
//            bool effectRequiresPotency;
//            if (!AttackEffect.TryGetPotencyRequirement(
//                KEditorTools.GetEnumValue<AttackEffectEnum>(effectsListProp.GetArrayElementAtIndex(i)),
//                out effectRequiresPotency))
//            {
//                //if the Effect wasn't found in the database
//                return;
//            }
//            effectsRequirePotency = effectRequiresPotency;
//            if (effectsRequirePotency)
//                break;
//        }

//        if (effectsRequirePotency)
//        {
//            FindProp("StatScaling").Draw();
//            FindProp("Potency").Draw();
//        }
//    }
//}