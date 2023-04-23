using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GizmosUtils
{
    public static void DrawPath(Vector3[] points)
    {
#if UNITY_EDITOR
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
#endif
    }


    public static void DrawText(string text, Vector3 worldPose) => DrawText(text, worldPose, null);
    public static void DrawText(string text, Vector3 worldPose, GUIStyle style)
    {
#if UNITY_EDITOR
        if (Camera.current)
        {
            var screenPoint = Camera.current.WorldToViewportPoint(worldPose);
            var onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 &&
                           screenPoint.y < 1;
            if (onScreen)
            {
                if (style == null)
                    UnityEditor.Handles.Label(worldPose, text);
                else
                    UnityEditor.Handles.Label(worldPose, text, style);
            }
        }
#endif
    }
}
