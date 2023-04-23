using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class ArikanExtensions
{

    public static Vector2 TopRight(this Rect r) => r.max;
    public static Vector2 BottomLeft(this Rect r) => r.min;
    public static Vector2 TopLeft(this Rect r) => new Vector2(r.min.x, r.max.y);
    public static Vector2 BottomRight(this Rect r) => new Vector2(r.max.x, r.min.y);


    public static Vector2 RandomPointInside(this Rect r)
    {
        return r.center + new Vector2(
           (Random.value - 0.5f) * r.size.x,
           (Random.value - 0.5f) * r.size.y
        );
    }
    public static Vector2 NormalizedToPoint(this Rect r, Vector2 normRectCoords)
    {
        return Rect.NormalizedToPoint(r, normRectCoords);
    }
    public static Vector2 PointToNormalized(this Rect r, Vector2 point)
    {
        return Rect.PointToNormalized(r, point);
    }

    public static Rect SubRect(this Rect r, Vector2 fromAlpha, Vector2 toAlpha)
    {
        var from = r.NormalizedToPoint(fromAlpha);
        var to = r.NormalizedToPoint(toAlpha);
        return Rect.MinMaxRect(from.x, from.y, to.x, to.y);
    }
    public static Vector2 ClampInside(this Rect r, Vector2 point)
    {
        return new Vector2(
            Mathf.Clamp(point.x, r.xMin, r.xMax),
            Mathf.Clamp(point.y, r.yMin, r.yMax)
            );
    }
    public static Vector2 ClampInside(this Rect r, Vector2 point, Vector2 deathZone)
    {
        return new Vector2(
            Mathf.Clamp(point.x, r.xMin + deathZone.x, r.xMax - deathZone.x),
            Mathf.Clamp(point.y, r.yMin + deathZone.y, r.yMax - deathZone.x)
            );
    }

    public static Vector3 RandomPointInside(this Bounds r)
    {
        return r.center + new Vector3(
           (Random.value - 0.5f) * r.size.x,
           (Random.value - 0.5f) * r.size.y,
           (Random.value - 0.5f) * r.size.z
        );
    }
    public static float HypotenuseLength(this Rect r)
    {
        return Mathf.Sqrt(r.width * r.width + r.height * r.height);
    }
}
