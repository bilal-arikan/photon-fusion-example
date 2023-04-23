using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arikan
{
    public class ArrowIndicatorCircular : ArrowIndicator
    {
        [Header("Arrow Cicular")]
        public float overrideRadius = -1;

        protected override void CalculatePosition()
        {
            if (!target)
                return;
            if (!uiCamera)
            {
                uiCamera = Camera.main;
                if (!uiCamera)
                    return;
            }
            if (!area)
                area = transform.parent as RectTransform;

            if (rootCanvas.renderMode == RenderMode.WorldSpace)
                onScreenPosition = rootCanvas.transform.InverseTransformPoint(target.position + offsetWorldPosition);
            else
                onScreenPosition = uiCamera.GetAnchoredPosition(target.position + offsetWorldPosition, rootCanvasScaler);
            onScreenPosition += offsetScreenPosition;
            if (overrideInArea != null)
                clampedPosition = ClampInside(overrideInArea.rect, onScreenPosition, Vector2.zero/*rectTransform.rect.size / 2*/);
            else
                clampedPosition = ClampInside(area.rect, onScreenPosition, Vector2.zero/* rectTransform.rect.size / 2*/);

            var newIsClamped = onScreenPosition != clampedPosition;

            if (isFirstFrame || newIsClamped != IsClamped)
            {
                // Debug.Log("triggerInOut: " + (newIsClamped != IsClamped) + " newIsClamped: " + newIsClamped + " IsClamped: " + IsClamped + " onScreenPosition: " + onScreenPosition + " clampedPosition: " + clampedPosition);
                if (newIsClamped)
                    OutArea.Invoke();
                else
                    InArea.Invoke();
            }
            Vector2 direction = Vector2.up;
            if (rootCanvas.renderMode == RenderMode.WorldSpace)
                direction = (Vector2)onScreenPosition - (Vector2)rectTransform.position;
            else
                direction = (Vector2)onScreenPosition - (Vector2)uiCamera.ScreenToWorldPoint(rectTransform.rect.center);

            if (overrideInArea != null)
                direction = Vector2.ClampMagnitude(direction, overrideInArea.rect.size.x / 2);
            else
                direction = Vector2.ClampMagnitude(direction, area.rect.size.x / 2);

            if (overrideRadius > 0)
                direction = Vector2.ClampMagnitude(direction, overrideRadius);

            rectTransform.anchoredPosition = rectTransform.rect.center + ((Vector2)direction);

            if (pointerDirectionTransform)
            {
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                pointerDirectionTransform.localRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }

            if (distanceText != null)
            {
                distanceText.text = string.Format(distanceTextFormat, (target.position - transform.position).magnitude * distanceMultiplier);
            }

            IsClamped = newIsClamped;
            isFirstFrame = false;
        }
    }
}
