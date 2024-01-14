using System;
using System.Collections;
using System.Collections.Generic;

public static class RandomExtensions
{
    public static float NextFloat(this Random random, double minValue, double maxValue)
    {
        return (float) (random.NextDouble() * (maxValue - minValue) + minValue);
    }
}