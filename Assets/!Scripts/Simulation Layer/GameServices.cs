using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameServices
{
    public static Vector3 WorldPositionOfMouse()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
