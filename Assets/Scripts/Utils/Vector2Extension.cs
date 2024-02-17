using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{
    public static Vector2 ToVector2(this Vector3 vector3)
    {
        Vector2 vector2 = new Vector2(vector3.x, vector3.z);
        return vector2;
    }
}
