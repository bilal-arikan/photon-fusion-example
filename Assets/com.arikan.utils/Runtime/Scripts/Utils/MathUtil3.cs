using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class MathUtil
{

    /// <summary>
    /// Girilen 2 sayı arasındaki sayıları Array olarak döndürür
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static int[] Range(int from, int to)
    {
        int[] temp = new int[to - from];
        for (int i = 0; i < temp.Length; i++)
            temp[i] = from + i;
        return temp;
    }

    /// <summary>
    /// Girilen 2 sayı arasındaki sayıları karıştırıp verir
    /// </summary>
    /// <param name="from">Inclusive</param>
    /// <param name="to">Exclusive</param>
    /// <returns></returns>
    public static int[] MixedNumbers(int from, int to)
    {
        var temp = Range(from, to);
        ArikanExtensions.Shuffle(temp);
        return temp;
    }

    public static int ClampZeroMax(int number)
    {
        return Mathf.Clamp(number, 0, int.MaxValue);
    }
    public static float ClampZeroMax(float number)
    {
        return Mathf.Clamp(number, 0, float.MaxValue);
    }

    public static float ModePositive(float number, float mode)
    {
        return ((number % mode) + mode) % mode;
    }
    public static int ModePositive(int number, int mode)
    {
        return ((number % mode) + mode) % mode;
    }
}
