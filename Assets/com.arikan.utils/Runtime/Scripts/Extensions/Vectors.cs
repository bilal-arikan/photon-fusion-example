// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Swizzling.cs" company="Nick Prühs">
//   Copyright (c) Nick Prühs. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;

public static class Vectors
{
    public static Vector2 SetX(this Vector2 v, float n) => new Vector2(n, v.y);
    public static Vector2 SetY(this Vector2 v, float n) => new Vector2(v.x, n);

    public static Vector3 SetX(this Vector3 v, float n) => new Vector3(n, v.y, v.z);
    public static Vector3 SetY(this Vector3 v, float n) => new Vector3(v.x, n, v.z);
    public static Vector3 SetZ(this Vector3 v, float n) => new Vector3(v.x, v.y, n);

    public static Vector4 SetX(this Vector4 v, float n) => new Vector4(n, v.y, v.z, v.w);
    public static Vector4 SetY(this Vector4 v, float n) => new Vector4(v.x, n, v.z, v.w);
    public static Vector4 SetZ(this Vector4 v, float n) => new Vector4(v.x, v.y, n, v.w);
    public static Vector4 SetW(this Vector4 v, float n) => new Vector4(v.x, v.y, v.z, n);

    public static Vector2Int SetX(this Vector2Int v, int newX) => new Vector2Int(newX, v.y);
    public static Vector2Int SetY(this Vector2Int v, int newY) => new Vector2Int(v.x, newY);

    public static Vector3Int SetX(this Vector3Int v, int newX) => new Vector3Int(newX, v.y, v.z);
    public static Vector3Int SetY(this Vector3Int v, int newY) => new Vector3Int(v.x, newY, v.z);
    public static Vector3Int SetZ(this Vector3Int v, int newZ) => new Vector3Int(v.x, v.y, newZ);

    public static Vector2 AddX(this Vector2 v, float newX) => new Vector2(v.x + newX, v.y);
    public static Vector2 AddY(this Vector2 v, float newY) => new Vector2(v.x, v.y + newY);

    public static Vector3 AddX(this Vector3 v, float newX) => new Vector3(v.x + newX, v.y, v.z);
    public static Vector3 AddY(this Vector3 v, float newY) => new Vector3(v.x, v.y + newY, v.z);
    public static Vector3 AddZ(this Vector3 v, float newZ) => new Vector3(v.x, v.y, v.z + newZ);

    public static Vector4 AddX(this Vector4 v, float n) => new Vector4(v.x + n, v.y, v.z, v.w);
    public static Vector4 AddY(this Vector4 v, float n) => new Vector4(v.x, v.y + n, v.z, v.w);
    public static Vector4 AddZ(this Vector4 v, float n) => new Vector4(v.x, v.y, v.z + n, v.w);
    public static Vector4 AddW(this Vector4 v, float n) => new Vector4(v.x, v.y, v.z, v.w + n);

    public static Vector2Int AddX(this Vector2Int v, int newX) => new Vector2Int(v.x + newX, v.y);
    public static Vector2Int AddY(this Vector2Int v, int newY) => new Vector2Int(v.x, v.y + newY);
    public static Vector3Int AddX(this Vector3Int v, int newX) => new Vector3Int(v.x + newX, v.y, v.z);
    public static Vector3Int AddY(this Vector3Int v, int newY) => new Vector3Int(v.x, v.y + newY, v.z);
    public static Vector3Int AddZ(this Vector3Int v, int newZ) => new Vector3Int(v.x, v.y, v.z + newZ);

    public static Vector3 MinusX(this Vector3 v) => new Vector3(-v.x, v.y, v.z);
    public static Vector3 MinusY(this Vector3 v) => new Vector3(v.x, -v.y, v.z);
    public static Vector3 MinusZ(this Vector3 v) => new Vector3(v.x, v.y, -v.z);
    public static Vector3 ZeroX(this Vector3 v) => new Vector3(0, v.y, v.z);
    public static Vector3 ZeroY(this Vector3 v) => new Vector3(v.x, 0, v.z);
    public static Vector3 ZeroZ(this Vector3 v) => new Vector3(v.x, v.y, 0);

    public static Vector2 MultX(this Vector2 v, float newX) => new Vector2(v.x * newX, v.y);
    public static Vector2 MultY(this Vector2 v, float newY) => new Vector2(v.x, v.y * newY);
    public static Vector3 MultX(this Vector3 v, float newX) => new Vector3(v.x * newX, v.y, v.z);
    public static Vector3 MultY(this Vector3 v, float newY) => new Vector3(v.x, v.y * newY, v.z);
    public static Vector3 MultZ(this Vector3 v, float newZ) => new Vector3(v.x, v.y, v.z * newZ);

    public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);
    public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);
    public static Vector2 YZ(this Vector3 v) => new Vector2(v.y, v.z);

    public static Vector3 X_Z(this Vector2 v) => new Vector3(v.x, 0, v.y);
    public static Vector3 _YZ(this Vector2 v) => new Vector3(0, v.x, v.y);

    public static Vector2Int ToInt(this Vector2 v) => new Vector2Int((int)v.x, (int)v.y);
    public static Vector3Int ToInt(this Vector3 v) => new Vector3Int((int)v.x, (int)v.y, (int)v.z);

    public static Vector3 MultiplyEach(this Vector3 v, Vector3 m) => new Vector3(v.x * m.x, v.y * m.y, v.z * m.z);

    public static int ClosestIndex(this Vector2 v, Vector2[] arounds)
    {
        if (arounds.Length == 0)
        {
            Debug.LogError("arounds.Length must be greater than Zero(0)");
            return -1;
        }

        int closestIndex = -1;
        Vector2 closest = arounds[0];
        float magnitude = (arounds[0] - v).magnitude;
        // 0. indexi default olarak aldık
        for (int i = 1; i < arounds.Length; i++)
        {
            float temp = (arounds[i] - v).magnitude;
            if (temp < magnitude)
            {
                closest = arounds[i];
                closestIndex = i;
                magnitude = temp;
            }
        }
        return closestIndex;
    }

    public static int ClosestIndex(this Vector3 v, Vector3[] arounds)
    {
        if (arounds.Length == 0)
        {
            Debug.LogError("arounds.Length must be greater than Zero(0)");
            return -1;
        }

        int closestIndex = -1;
        Vector3 closest = arounds[0];
        float magnitude = (arounds[0] - v).magnitude;
        // 0. indexi default olarak aldık
        for (int i = 1; i < arounds.Length; i++)
        {
            float temp = (arounds[i] - v).magnitude;
            if (temp < magnitude)
            {
                closest = arounds[i];
                closestIndex = i;
                magnitude = temp;
            }
        }
        return closestIndex;
    }

    //public static float GetMaxScalar(Vector2 v) => Mathf.Max(v.x, v.y);
    //public static float GetMaxScalar(Vector3 v) => Mathf.Max(v.x, v.y, v.z);
    //public static float GetMaxScalar(Vector4 v) => Mathf.Max(v.x, v.y, v.z, v.z);
    //public static float GetMinScalar(Vector2 v) => Mathf.Min(v.x, v.y);
    //public static float GetMinScalar(Vector3 v) => Mathf.Min(v.x, v.y, v.z);
    //public static float GetMinScalar(Vector4 v) => Mathf.Min(v.x, v.y, v.z, v.z);

    public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        return new Vector3
        (
            Mathf.Clamp(value.x, min.x, max.x),
            Mathf.Clamp(value.y, min.y, max.y),
            Mathf.Clamp(value.z, min.z, max.z)
        );
    }
    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2
        (
            Mathf.Clamp(value.x, min.x, max.x),
            Mathf.Clamp(value.y, min.y, max.y)
        );
    }

    public static Vector2 ClampX(this Vector2 v, float min = 0, float max = float.MaxValue) => new Vector2(Mathf.Clamp(v.x, min, max), v.y);
    public static Vector2 ClampY(this Vector2 v, float min = 0, float max = float.MaxValue) => new Vector2(v.x, Mathf.Clamp(v.y, min, max));

    public static Vector3 ClampX(this Vector3 v, float min = 0, float max = float.MaxValue) => new Vector3(Mathf.Clamp(v.x, min, max), v.y, v.z);
    public static Vector3 ClampY(this Vector3 v, float min = 0, float max = float.MaxValue) => new Vector3(v.x, Mathf.Clamp(v.y, min, max), v.z);
    public static Vector3 ClampZ(this Vector3 v, float min = 0, float max = float.MaxValue) => new Vector3(v.x, v.y, Mathf.Clamp(v.z, min, max));

    public static Vector3 ClampAngleX(this Vector3 v, float min, float max) => new Vector3(ClampAngle360(v.x, min, max), v.y, v.z);
    public static Vector3 ClampAngleY(this Vector3 v, float min, float max) => new Vector3(v.x, ClampAngle360(v.y, min, max), v.z);
    public static Vector3 ClampAngleZ(this Vector3 v, float min, float max) => new Vector3(v.x, v.y, ClampAngle360(v.z, min, max));
    public static float ClampAngle360(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f)
            return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    public static Vector2Int ClampX(this Vector2Int v, int min = 0, int max = int.MaxValue) => new Vector2Int(Mathf.Clamp(v.x, min, max), v.y);
    public static Vector2Int ClampY(this Vector2Int v, int min = 0, int max = int.MaxValue) => new Vector2Int(v.x, Mathf.Clamp(v.y, min, max));

    public static Vector3Int ClampX(this Vector3Int v, int min = 0, int max = int.MaxValue) => new Vector3Int(Mathf.Clamp(v.x, min, max), v.y, v.z);
    public static Vector3Int ClampY(this Vector3Int v, int min = 0, int max = int.MaxValue) => new Vector3Int(v.x, Mathf.Clamp(v.y, min, max), v.z);
    public static Vector3Int ClampZ(this Vector3Int v, int min = 0, int max = int.MaxValue) => new Vector3Int(v.x, v.y, Mathf.Clamp(v.z, min, max));

    public static Vector2 Rotate(this Vector2 v, float deltaAngle)
    {
        var delta = deltaAngle * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
}
