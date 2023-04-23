using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arikan
{
    public class GizmoVerticalLine : MonoBehaviour
    {
        public float Length = 100;
        public Color color = Color.white;
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawLine(transform.position, transform.position.AddY(Length));
        }
#endif
    }
}
