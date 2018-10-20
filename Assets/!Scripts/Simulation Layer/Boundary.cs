using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public Vector2 X;
    public Vector2 Y;

    public bool XContains(float value)
    {
        return value > X.x && value < X.y;
    }
    public bool YContains(float value)
    {
        return value > Y.x && value < Y.y;
    }
    public bool Contains(Vector2 point)
    {
        return XContains(point.x) && YContains(point.y);
    }
    public Vector2 ClosestPoint(Vector2 originalPoint)
    {
        return new Vector2
        (
            Mathf.Clamp(originalPoint.x, X.x, X.y),
            Mathf.Clamp(originalPoint.y, Y.x, Y.y)
        );
    }
}
