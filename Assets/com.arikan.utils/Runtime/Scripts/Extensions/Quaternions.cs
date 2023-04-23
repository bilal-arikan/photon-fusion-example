using System;
using System.Collections.Generic;
using UnityEngine;

public static class Quaternions
{
    public static Quaternion WithRandomX(this Quaternion q)
    {
        return Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), q.eulerAngles.y, q.eulerAngles.z);
    }
    public static Quaternion WithRandomY(this Quaternion q)
    {
        return Quaternion.Euler( q.eulerAngles.x, UnityEngine.Random.Range(0f, 360f), q.eulerAngles.z);
    }
    public static Quaternion WithRandomZ(this Quaternion q)
    {
        return Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, UnityEngine.Random.Range(0f, 360f));
    }

    public static Quaternion WithRandomX()
    {
        var q = Quaternion.identity;
        return Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), q.eulerAngles.y, q.eulerAngles.z);
    }
    public static Quaternion WithRandomY()
    {
        var q = Quaternion.identity;
        return Quaternion.Euler(q.eulerAngles.x, UnityEngine.Random.Range(0f, 360f), q.eulerAngles.z);
    }
    public static Quaternion WithRandomZ()
    {
        var q = Quaternion.identity;
        return Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, UnityEngine.Random.Range(0f, 360f));
    }
}
