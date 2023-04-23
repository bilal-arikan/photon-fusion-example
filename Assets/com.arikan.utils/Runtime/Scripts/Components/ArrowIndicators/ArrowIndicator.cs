using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arikan
{
    public abstract class ArrowIndicator : MonoBehaviour
    {
        public static List<ArrowIndicator> activeIndicators = new List<ArrowIndicator>();

        public Camera uiCamera;
        public RectTransform area;
        public RectTransform overrideInArea = null;
        public Transform target;
        [Header("Disatnce")]
        public Text distanceText;
        public string distanceTextFormat = "{0}m";
        public float distanceMultiplier = 1f;
        [Space]
        public Vector3 offsetWorldPosition;
        public Vector2 offsetScreenPosition;
        public RectTransform pointerDirectionTransform;

        public UnityEvent InArea;
        public UnityEvent OutArea;

        public bool IsClamped { get; protected set; }

        protected Canvas rootCanvas;
        protected RectTransform rectTransform;
        protected Vector2 onScreenPosition;
        protected Vector2 clampedPosition;
        protected readonly Quaternion defRot = Quaternion.AngleAxis(-180, Vector3.forward);
        protected bool isFirstFrame = true;
        protected CanvasScaler rootCanvasScaler;

        protected virtual void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
            rootCanvas = rectTransform.GetComponentInParent<Canvas>()?.rootCanvas;
            rootCanvasScaler = GetComponentInParent<CanvasScaler>();
            // Application.onBeforeRender += CalculatePosition;
            activeIndicators.Add(this);
        }
        protected virtual void OnDisable()
        {
            // Application.onBeforeRender -= CalculatePosition;
            activeIndicators.Remove(this);
        }

        protected virtual void Update() => CalculatePosition();
        protected abstract void CalculatePosition();

        protected static Vector2 ClampInside(Rect r, Vector2 point, Vector2 deathZone)
        {
            return new Vector2(
                (r.xMin + deathZone.x > r.xMax - deathZone.x) ? r.center.x : Mathf.Clamp(point.x, r.xMin + deathZone.x, r.xMax - deathZone.x),
                (r.yMin + deathZone.y > r.yMax - deathZone.y) ? r.center.y : Mathf.Clamp(point.y, r.yMin + deathZone.y, r.yMax - deathZone.y));
        }
    }
}
