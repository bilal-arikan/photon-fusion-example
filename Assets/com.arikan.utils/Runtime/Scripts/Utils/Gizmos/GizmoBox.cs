using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arikan
{
    public class GizmoBox : MonoBehaviour
    {
        public Color color = Color.white;
        public Vector3 size = new Vector3(1, 1, 1);
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(transform.position, size);
        }
#endif
    }
}
