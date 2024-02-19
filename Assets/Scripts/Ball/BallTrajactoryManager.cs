using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrajectoryType
{
    CloseShoot,
    MiddleShoot,
    LongShoot,
}

public struct TrajectoryMuzzleV
{
    public const float CloseShootV = 5.0f;
    public const float MiddleShootV = 8.0f;
    public const float LongShootV = 10.0f;
}

public struct TrajectorySpeedOffSet
{
    public const float CloseShootV = 0.2f;
    public const float MiddleShootV = 0.15f;
    public const float LongShootV = 0.13f;
}

public static class BallTrajactoryManager
{
    public static float gravityF = -9.8f;
    public static float frameRate = 1.0f / 30.0f;

    public static List<Vector3> CalculateBallTrajactory(Vector3 start, Vector3 end, float muzzleV, float speedOffset)
    {
        List<Vector3> traj = new List<Vector3>();
        float time = 0;
        Vector3 calcDir = CalculateOutDirection(start, end, muzzleV, out time);
        float t = 0;
        Vector3 gravity = new Vector3(0, gravityF, 0);
        while(t< time)
        {
            t += time * frameRate * speedOffset;
            Vector3 pt = start + calcDir * muzzleV * t + (gravity * t * t) / 2;
            traj.Add(pt);
        }
        return traj;
    }

    private static Vector3 CalculateOutDirection(Vector3 start, Vector3 end, float muzzleV, out float ttt)
    {
        ttt = 0f;
        Vector3 gravity = new Vector3(0,gravityF,0);
        Vector3 delta = end - start;
        float a = Vector3.Dot(gravity,gravity);
        float b = -4 * (muzzleV * muzzleV - Vector3.Dot(gravity,delta));
        float c = 4*Vector3.Dot(delta,delta);

        if (4 * a * c > b * b)
        {
            return Vector3.zero;
        }
        else
        {
            float time0 = Mathf.Sqrt((-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a));
            float time1 = Mathf.Sqrt((-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a));
            if (time0 < 0)
            {
                if (time1 < 0)
                {
                    return Vector3.zero;
                }
                else
                {
                    ttt = time1;
                }
            }
            else
            {
                if (time1 < 0)
                {
                    ttt = time0;
                }
                else
                {
                    ttt = Mathf.Max(time0, time1);
                }
            }
            return (2 * delta - gravity * ttt * ttt) / (2 * muzzleV * ttt);
        }
    }
}
