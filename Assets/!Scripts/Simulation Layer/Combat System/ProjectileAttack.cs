using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Combat System/New Projectile Attack")]
public class ProjectileAttack : AttackType
{
    public ProjectileController ProjectilePrefab;

    protected override List<ICombatant> Attack(ICombatant combatant)
    {
        Transform projTrans = combatant.GetProjectileTransform();
        ProjectileController pc = Instantiate(ProjectilePrefab.gameObject, projTrans.position, projTrans.rotation).GetComponent<ProjectileController>();
        pc.Damage = GetDamagePacket(combatant);
        pc.OriginalAttackType = this;

        return new List<ICombatant>();
    }
}
[CustomEditor(typeof(ProjectileAttack))]
public class Insp_ProjectileAttack : Insp_AttackType
{
    //most fields supplied by Insp_AttackType
    SerializedProperty projProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        projProp = serializedObject.FindProperty("ProjectilePrefab");
    }
    protected override string GetSubtypeName()
    {
        return "ProjectileAttack";
    }
    protected override void AttackSubtypeField()
    {
        EditorGUILayout.PropertyField(projProp);
    }
}