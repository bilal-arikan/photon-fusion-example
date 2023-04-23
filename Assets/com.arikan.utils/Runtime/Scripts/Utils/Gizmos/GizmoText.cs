using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GizmoText : MonoBehaviour
{
    public string text;
    public Vector3 localPose;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Camera.current)
        {
            var worldPose = transform.TransformPoint(localPose);
            var screenPoint = Camera.current.WorldToViewportPoint(worldPose);
            var onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 &&
                           screenPoint.y < 1;
            if (onScreen)
            {
                UnityEditor.Handles.Label(worldPose, text);
            }
        }
    }
#endif
}
