using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaitForFrames
{
    public static IEnumerator Frames(int frameCount)
    {
        if (frameCount <= 0)
        {
            throw new ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
        }

        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }
}
