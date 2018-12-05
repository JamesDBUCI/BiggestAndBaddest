using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Crowd Control")]
public class CrowdControlTemplate : AbstractAssetTemplate
{
    [Range(0, 1)]
    public float MoveSpeed = 1;
    [Range(0, 1)]
    public float AimSpeed = 1;
    [Range(0, 360)]
    public float MoveClockwiseOffset = 0;
    [Range(0, 360)]
    public float AimClockwiseOffset = 0;
    public bool StopUsingFeats = false;
    public bool StopUsingSpells = false;
}
