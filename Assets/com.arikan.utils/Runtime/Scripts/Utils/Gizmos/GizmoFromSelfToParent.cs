using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arikan
{
    public class GizmoFromSelfToParent : MonoBehaviour
    {
        public Color color = Color.white;
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!transform.parent)
                return;
            Gizmos.color = color;
            Gizmos.DrawLine(transform.position, transform.parent.position);
        }
#endif
    }
}
