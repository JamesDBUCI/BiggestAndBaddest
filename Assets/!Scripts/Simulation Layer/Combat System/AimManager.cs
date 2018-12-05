using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAimer : IActorOwned
{
    AimManager AimManager { get; }
}
public class AimManager
{
    protected Transform _aimTransform;

    public AimManager(Transform aimTransform)
    {
        _aimTransform = aimTransform;
    }
    public Vector2 GetLocation()
    {
        return _aimTransform.position;
    }
    public float GetSimpleRotation()
    {
        return GetComplexRotation().eulerAngles.z;
    }
    public Quaternion GetComplexRotation()
    {
        return _aimTransform.rotation;
    }
    public void RotateByDegrees(float degreesDelta)
    {
        _aimTransform.Rotate(0, 0, degreesDelta);
    }
    public void RotateByComplexRotation(Quaternion rotationDelta)
    {
        _aimTransform.rotation *= rotationDelta;
    }
    public Vector2 GetAimDirection()
    {
        return _aimTransform.up;
    }
    public float DistanceToWorldPoint(Vector2 worldPoint)
    {
        return Vector2.Distance(worldPoint, _aimTransform.position);
    }
    public float GetDeltaRotationToAimAtPoint(Vector2 worldPoint)
    {
        return Vector2.SignedAngle(GetAimDirection(), worldPoint - GetLocation());
    }
    public void AimAtPoint(Vector2 worldPoint)
    {
        _aimTransform.LookAt(worldPoint, Vector3.back);
    }
}
