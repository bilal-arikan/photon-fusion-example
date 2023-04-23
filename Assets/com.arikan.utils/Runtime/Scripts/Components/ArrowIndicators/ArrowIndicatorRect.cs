using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Arikan
{
    public class ArrowIndicatorRect : ArrowIndicator
    {
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
                clampedPosition = ClampInside(overrideInArea.rect, onScreenPosition, rectTransform.rect.size / 2);
            else
                clampedPosition = ClampInside(area.rect, onScreenPosition, rectTransform.rect.size / 2);

            rectTransform.anchoredPosition = clampedPosition;

            var newIsClamped = onScreenPosition != clampedPosition;
            if (overrideInArea != null)
            {
                newIsClamped = newIsClamped && onScreenPosition != ClampInside(overrideInArea.rect, onScreenPosition, rectTransform.rect.size / 2);
            }

            if (isFirstFrame || newIsClamped != IsClamped)
            {
                // Debug.Log("triggerInOut: " + (newIsClamped != IsClamped) + " newIsClamped: " + newIsClamped + " IsClamped: " + IsClamped + " onScreenPosition: " + onScreenPosition + " clampedPosition: " + clampedPosition);
                if (newIsClamped)
                    OutArea.Invoke();
                else
                    InArea.Invoke();
            }

            if (pointerDirectionTransform)
            {
                if (newIsClamped)
                {
                    Vector2 direction = Vector2.up;
                    if (rootCanvas.renderMode == RenderMode.WorldSpace)
                        direction = (Vector2)onScreenPosition - (Vector2)rectTransform.position;
                    else
                        direction = (Vector2)onScreenPosition - (Vector2)uiCamera.ScreenToWorldPoint(rectTransform.rect.center);

                    var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    pointerDirectionTransform.localRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                }
                else
                {
                    pointerDirectionTransform.localRotation = defRot;
                }
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
