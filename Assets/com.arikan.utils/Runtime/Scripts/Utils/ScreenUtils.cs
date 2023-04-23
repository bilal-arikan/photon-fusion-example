using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Arikan
{
    public static class ScreenUtils
    {
        public static float Pix2Dpi(float pix)
        {
            return pix / (Screen.dpi / 160);
        }

        // Checks if an input is on a UI object
        public static bool IsPointerOverUIObject(Vector2 screenPosition)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        public static bool IsPointerOverUIObjectNew()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// Clamps the position on the screen for a Screenspace-Camera UI
        /// </summary>
        /// <param name="onScreenPosition">The current on-screen position for an UI element</param>
        /// <param name="followElementRect">The rect that follows the worldspace object</param>
        /// <param name="mainCanvasRectTransform">The rect transform of this UI's main canvas</param>
        /// <returns></returns>
        public static Vector2 GetClampedOnScreenPosition(this Vector2 onScreenPosition, Rect followElementRect, RectTransform mainCanvasRectTransform)
        {
            return new Vector2(Mathf.Clamp(onScreenPosition.x, 0f, mainCanvasRectTransform.sizeDelta.x - followElementRect.size.x),
                               Mathf.Clamp(onScreenPosition.y, 0f, mainCanvasRectTransform.sizeDelta.y - followElementRect.size.y));
        }
    }
}
