using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomNumberGenerator
{
    public static float GenerateRandomNumber()
    {
        System.Random rand = new System.Random(DateTime.Now.Millisecond);

        float randomNumber = (float)rand.NextDouble();

        return randomNumber;
    }
}
