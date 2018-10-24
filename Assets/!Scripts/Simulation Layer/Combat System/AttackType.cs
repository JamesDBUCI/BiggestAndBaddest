using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class AttackType : ScriptableObject
{
    //prescribes the data needed for a player to attack in a particular way

    //aoe from origin
    //projectile

    public string ExternalName;
    public float AttackRate = 0.3f;     //to be improved later
    //public GameObject Graphics;   //particle or graphical effect object (later)

    public List<StatScaler> StatScaling = new List<StatScaler>();   //amount by which stats are factored when calculating damage for this attack (0.5 = half the stat value)
    public List<DamageWithType> DamagePotency = new List<DamageWithType>();
    public List<AttackEffectEnum> Effects = new List<AttackEffectEnum>();

    public virtual bool TryAttack(ICombatant combatant, out List<ICombatant> combatantsHit)
    {
        //attack timing control. this is temporary

        combatantsHit = null;

        //handle
        CombatController cc = combatant.GetCombatController();

        if (Time.time < cc.LastAttackTimestamp + AttackRate)
            return false;

        combatantsHit = Attack(combatant);

        cc.LastAttackTimestamp = Time.time;
        return true;
    }

    protected virtual DamagePacket GetDamagePacket(ICombatant combatant)
    {
        return CombatServices.ConstructDamagePacket(combatant, this);
    }
    protected abstract List<ICombatant> Attack(ICombatant combatant);       //return list of affected combatants, including allies, for instant-speed attacks
    protected List<AttackEffectDelegate> GetAttackEffects()
    {
        //can be overridden for whatever reason
        List<AttackEffectDelegate> effectDelegates = new List<AttackEffectDelegate>();
        foreach (AttackEffectEnum enumValue in Effects)
        {
            AttackEffectDelegate del;
            if (!AttackEffect.TryGet(enumValue, out del))
                continue;

            effectDelegates.Add(del);
        }
        return effectDelegates;
    }
    public virtual void ApplyAttackEffects(ICombatant origin, List<ICombatant> targets)
    {
        //use this overload if damage is calculated at time of impact
        ApplyAttackEffects(GetDamagePacket(origin), targets);
    }
    public virtual void ApplyAttackEffects(DamagePacket damagePacket, List<ICombatant> targets)
    {
        //use this overload if damage was already calculated (projectiles, etc...)
        if (targets.Count > 0)
        {
            foreach (AttackEffectDelegate effect in GetAttackEffects())
            {
                effect(damagePacket, targets);
            }
        }
    }
}

//[CustomEditor(typeof(AttackType))]
public abstract class Insp_AttackType : Editor
{
    SerializedProperty nameProp;
    SerializedProperty rateProp;
    SerializedProperty scaleListProp;
    SerializedProperty damageListProp;
    SerializedProperty effectsListProp;

    protected virtual void OnEnable()
    {
        nameProp = serializedObject.FindProperty("ExternalName");
        rateProp = serializedObject.FindProperty("AttackRate");
        scaleListProp = serializedObject.FindProperty("StatScaling");
        damageListProp = serializedObject.FindProperty("DamagePotency");
        effectsListProp = serializedObject.FindProperty("Effects");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        SectionHeading("Generic AttackType data");

        EditorGUILayout.PropertyField(nameProp);
        EditorGUILayout.PropertyField(rateProp);
        KEditorTools.ListBig(scaleListProp, new GUIContent("Scaled Stats", "List of all stats this attack scales on and by how much."), "Scaled Stat", false);
        KEditorTools.ListBig(damageListProp, new GUIContent("Damage Potency", "List of all types of damage per hit and what potency of stat scaling."), "Potency", true);
        KEditorTools.ListMini(effectsListProp, new GUIContent("Effects", "What will happen to each combatant hit by the attack?"), false);

        GUILayout.Space(8);

        SectionHeading(GetSubtypeName() + " data");
        AttackSubtypeField();

        serializedObject.ApplyModifiedProperties();
    }
    private void SectionHeading(string contentName)
    {
        EditorGUILayout.LabelField(contentName, EditorStyles.miniLabel);
    }

    protected abstract string GetSubtypeName();
    protected abstract void AttackSubtypeField();
}