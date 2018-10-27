using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CreateAssetMenu(menuName = "Skill")]
public abstract class Skill : ScriptableObject
{
    //data-only Unity-editable recipe for a skill

    public abstract AttackType GetAttackType();   //defined by subclass

    public string ExternalName = "Untitled Skill";

    //visual
    public Sprite Icon = null;
    //public GameObject Graphics;   //particle or graphical effect object (later)

    //timing
    public SkillTimeline SkillTimeline = new SkillTimeline();
    public SkillInfo_InstantSpeed InstantSpeedInfo { get { return SkillTimeline.InstantSpeedInfo; } }
    public SkillInfo_Cooldown CooldownInfo { get { return SkillTimeline.CooldownInfo; } }
    public SkillInfo_SkillLock SkillLockInfo { get { return SkillTimeline.SkillLockInfo; } }
    public SkillInfo_Channeling ChannelingInfo { get { return SkillTimeline.ChannelingInfo; } }

    //skill phases
    public List<SkillPhase> SkillPhases = new List<SkillPhase>();

    //convenience checks
    public bool IsInstant { get { return InstantSpeedInfo.HasInstantSpeed; } }
    public bool HasCooldown { get { return CooldownInfo.HasCooldown || IsInstant; } }
    public bool IsChanneled { get { return ChannelingInfo.IsChanneledSkill && !IsInstant; } }
}

[System.Serializable]
public class SkillInfo_InstantSpeed
{
    public bool HasInstantSpeed = false;
    public bool CanUseWhileChanneling = true;
}
[System.Serializable]
public class SkillInfo_Channeling
{
    public bool IsChanneledSkill = false;
    public float ChannelDuration = 1;
    public bool EnemiesCanInterrupt = true;
    public CrowdControlTemplate SelfInflictedCC = null;
    //public bool BypassCCImmunity = false;
    public bool MovingCancelsChannel = true;
}
[System.Serializable]
public class SkillInfo_Cooldown
{
    public bool HasCooldown = true;
    public float CooldownDuration = Const.MIN_COOLDOWN_DURATION;
}
[System.Serializable]
public class SkillInfo_SkillLock
{
    public float SkillLockDuration = Const.MIN_SKILL_LOCK_DURATION;
}

[System.Serializable]
public class SkillTimeline
{
    public SkillInfo_InstantSpeed InstantSpeedInfo = new SkillInfo_InstantSpeed();
    public SkillInfo_Cooldown CooldownInfo = new SkillInfo_Cooldown();
    public SkillInfo_SkillLock SkillLockInfo = new SkillInfo_SkillLock();
    public SkillInfo_Channeling ChannelingInfo = new SkillInfo_Channeling();
}
//[CustomPropertyDrawer(typeof(SkillTimeline))]
//public class PD_SkillTimeline : PropertyDrawer
//{
//    SerializedProperty instantSpeedProp;
//    SerializedProperty cooldownProp;
//    SerializedProperty skillLockProp;
//    SerializedProperty channelProp;

//    float totalHeight = 150;
//    float graphicHeight = 10f;
//    float label1Height = 
//    Color channelColor = Color.blue;
//    Color postChannelColor = Color.red;
    

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        Rect graphicPosition = new Rect(position) { height = graphicHeight };

//        return totalHeight;
//    }
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        instantSpeedProp = property.FindPropertyRelative("InstantSpeedInfo");
//        cooldownProp = property.FindPropertyRelative("CooldownInfo");
//        skillLockProp = property.FindPropertyRelative("SkillLockInfo");
//        channelProp = property.FindPropertyRelative("ChannelingInfo");

//        EditorGUI.BeginProperty(position, label, property);

//        DrawGraphic(graphicPosition, channelProp.FindPropertyRelative("ChannelDuration").floatValue, skillLockProp.FindPropertyRelative("SkillLockDuration").floatValue);

//        EditorGUI.EndProperty();
//    }
//    protected void DrawGraphic(Rect position, float usePoint, float animationLength)
//    {
//        animationLength = Mathf.Max(0, Const.MIN_SKILL_LOCK_DURATION);
//        usePoint = Mathf.Clamp(usePoint, 0, animationLength);

        

//        usePoint = 2;
//        animationLength = 4;

//        Rect channelRect = new Rect(position) { width = position.width * (usePoint / animationLength) };
//        Rect postChannelRect = new Rect(position) { x = channelRect.xMax, width = position.width - channelRect.width };

//        EditorGUI.DrawRect(channelRect, channelColor);
//        EditorGUI.DrawRect(postChannelRect, postChannelColor);
//    }
//}

public abstract class Insp_Skill : Editor
{
    //editor-only variables
    int selectedPage = 0;
    bool showinstantSpeedInfo = false;
    //bool showSkillLockInfo = false;
    bool showCooldownInfo = false;
    bool showChannelingInfo = false;

    //SerializedProperty section

    SerializedProperty nameProp;
    
    //visual
    SerializedProperty iconProp;
    //graphics here

    //timing
    SerializedProperty timelineProp;

    //skill phases
    EditorPageFlipper phasesFlipper;
    SerializedProperty phasesListProp;

    //subtype props supplied by subtype

    //GUIContent section

    readonly GUIContent nameCont = new GUIContent("External Name", "Skill name the player will see.");

    //visual
    readonly GUIContent iconCont = new GUIContent("Hotbar Icon", "Icon shown on the player's hotbar.");
    //graphics go here

    //timing

    //skill phases
    readonly GUIContent phasesListCont = new GUIContent("Skill Phases", "List of all skill phases and effects, at which times they will occur.");

    //subtype tooltips supplied by subtype

    protected abstract string GetSubtypeName();
    protected void OnEnable()
    {
        nameProp = serializedObject.FindProperty("ExternalName");

        //visual
        iconProp = serializedObject.FindProperty("Icon");
        //graphics go here

        //timing
        timelineProp = serializedObject.FindProperty("SkillTimeline");

        //skill phases
        phasesListProp = serializedObject.FindProperty("SkillPhases");
        phasesFlipper = new EditorPageFlipper(phasesListProp, phasesListCont, (page, num) => string.Format("Skill Phase #{0}", num + 1));

        //subtype info
        AdditionalOnEnable();
    }
    protected abstract void AdditionalOnEnable();

    public override void OnInspectorGUI()
    {
        //must have or die
        serializedObject.Update();

        //law-enforcement section

        //force value constraints
        //FloatPropMinimumValue(channelProp.FindPropertyRelative("ChannelDuration"), 0); //min channel duration
        //FloatPropMinimumValue(skillLockProp.FindPropertyRelative("SkillLockDuration"), Const.MIN_SKILL_LOCK_DURATION); //min skill-lock duration
        //FloatPropMinimumValue(cooldownProp.FindPropertyRelative("CooldownDuration"), Const.MIN_COOLDOWN_DURATION); //min cooldown duration

        ////force options for instant speed skills
        //if (instantSpeedProp.FindPropertyRelative("HasInstantSpeed").boolValue)
        //{
        //    cooldownProp.FindPropertyRelative("HasCooldown").boolValue = true;
        //    showCooldownInfo = true;
        //    channelProp.FindPropertyRelative("IsChanneledSkill").boolValue = false;
        //    showChannelingInfo = false;
        //}

        //top of inspector

        //name is visible from all pages
        EditorGUILayout.PropertyField(nameProp, nameCont);
        GUILayout.Space(8);

        //toolbar to select which page of data we're looking at
        selectedPage = GUILayout.Toolbar(selectedPage, new string[] { "Visual", "Timing", "Effects", GetSubtypeName() });

        //visual
        if (selectedPage == 0)
        {
            KEditorTools.SpriteFieldWithPreview(iconProp, iconCont, 64, 64, true);
        }

        //timing
        if (selectedPage == 1)
        {
            EditorGUILayout.PropertyField(timelineProp, true);
            //InfoPage(ref showinstantSpeedInfo, instantSpeedProp, "Instant Speed Info");
            //InfoPage(ref showCooldownInfo, cooldownProp, "Cooldown Info");
            //InfoPage(ref showChannelingInfo, channelProp, "Channeling Info");
        }

        //skill phases
        if (selectedPage == 2)
        {
            //KEditorTools.ListBig(phasesListProp, phasesListCont, "Skill Phase", false);
            phasesFlipper.Draw();
        }

        //subtype info
        if (selectedPage == 3)
        {
            AdditionalOnInspectorGUI();
        }            

        //must have or die
        serializedObject.ApplyModifiedProperties();
    }
    protected void FloatPropMinimumValue(SerializedProperty floatProp, float value)
    {
        floatProp.floatValue = Mathf.Max(floatProp.floatValue, value);    //minimum of value
    }
    protected void InfoPage(ref bool toggleOn, SerializedProperty infoProp, string name)
    {
        if (GUILayout.Button(name))
        {
            toggleOn = !toggleOn;
        }
        EditorGUI.BeginDisabledGroup(!toggleOn);
        EditorGUILayout.PropertyField(infoProp, true);
        EditorGUI.EndDisabledGroup();
    }

    protected abstract void AdditionalOnInspectorGUI();
}