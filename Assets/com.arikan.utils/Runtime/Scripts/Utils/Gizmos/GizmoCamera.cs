using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arikan
{
    public class GizmoCamera : MonoBehaviour
    {
        new Camera camera;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!camera)
                camera = GetComponent<Camera>();

            Matrix4x4 temp = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            if (camera.orthographic)
            {
                float spread = camera.farClipPlane - camera.nearClipPlane;
                float center = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
                Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, spread));
            }
            else
            {
                Gizmos.DrawFrustum(transform.position/* new Vector3(0, 0, (camera.nearClipPlane))*/, camera.fieldOfView, 20, camera.nearClipPlane, camera.aspect);
            }
            Gizmos.matrix = temp;
        }
#endif
    }
}
