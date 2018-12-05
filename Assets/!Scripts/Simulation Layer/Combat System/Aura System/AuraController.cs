using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AuraController : MonoBehaviour
{
    public CircleCollider2D AuraCollider { get; private set; }
    public AuraInstance AuraInstance { get; private set; }

    private void OnEnable()
    {
        AuraCollider = GetComponent<CircleCollider2D>();
    }
    public void SetAura(AuraInstance aura)
    {
        AuraInstance = aura;
        AuraCollider.radius = aura.RadiusMultiplier;
    }
}

