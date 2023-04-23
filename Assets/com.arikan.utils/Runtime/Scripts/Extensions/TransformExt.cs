using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformExt
{
    /// <summary>
    ///   Resets the local position, rotation and scale of a transform.
    /// </summary>
    /// <param name="transform">Transform to reset.</param>
    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static Transform FindOrCreate(this Transform t, string name)
    {
        var obj = t.Find(name);
        if (obj == null)
        {
            obj = new GameObject(name).transform;
            obj.SetParent(t, false);
            obj.Reset();
        }
        return obj;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="arounds"></param>
    /// <returns></returns>
    public static Transform Closest(this Transform t, List<Transform> arounds)
    {
        if (arounds.Count == 0)
        {
            return null;
        }

        Transform closest = arounds[0];
        float magnitude = (arounds[0].position - t.position).magnitude;
        // 0. indexi default olarak aldık
        for (int i = 1; i < arounds.Count; i++)
        {
            float temp = (arounds[i].position - t.position).magnitude;
            if (temp < magnitude)
            {
                closest = arounds[i];
                magnitude = temp;
            }
        }
        return closest;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="arounds"></param>
    /// <returns></returns>
    public static Transform Closest(this Transform t, params Transform[] arounds)
    {
        if (arounds.Length == 0)
        {
            return null;
        }

        Transform closest = arounds[0];
        float magnitude = (arounds[0].position - t.position).magnitude;
        // 0. indexi default olarak aldık
        for (int i = 1; i < arounds.Length; i++)
        {
            float temp = (arounds[i].position - t.position).magnitude;
            if (temp < magnitude)
            {
                closest = arounds[i];
                magnitude = temp;
            }
        }
        return closest;
    }


}