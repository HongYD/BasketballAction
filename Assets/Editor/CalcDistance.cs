using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CalcDistance
{
    [MenuItem("Utils/CalcThreePointDist")]
    public static void CalcThreePointDistance()
    {
        float dist = Vector2.Distance(new Vector3(0,0, 5.584f).ToVector2(), new Vector3(-0.001f, -0.2522868f, 11.405f).ToVector2());
        Debug.Log($"三分线距离是:{dist}");
    }
}
