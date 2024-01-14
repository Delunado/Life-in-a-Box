using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class ListUtils
{
    public static void ShuffleList<T>(this List<T> list, Random rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}