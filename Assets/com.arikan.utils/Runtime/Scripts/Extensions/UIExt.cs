using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class UIExt
{
    static object Delay(bool isRealTime, float delay)
    {
        if (isRealTime)
            return new WaitForSecondsRealtime(delay);
        else
            return new WaitForSeconds(delay);
    }

    public static IEnumerator Fade(this Image i, float targetAlpha, float duration = 3, float delay = 0, bool realTime = true) =>
        Fade(i, i.color.WithAlpha(targetAlpha), duration, delay, realTime);
    public static IEnumerator Fade(this Image i, Color target, float duration = 3, float delay = 0, bool realTime = true)
    {
        yield return Delay(realTime, delay);

        float targetTime = (realTime ? Time.unscaledTime : Time.time) + duration;
        Color def = i.color;

        while (realTime ? Time.unscaledTime < targetTime : Time.time < targetTime)
        {
            i.color = Color.Lerp(target, def, (realTime ? targetTime - Time.unscaledTime : targetTime - Time.time) / duration);
            yield return null;
        }
        i.color = target;
    }
    public static IEnumerator Fade(this CanvasGroup i, float targetAlpha, float duration = 3, float delay = 0, bool realTime = true)
    {
        yield return Delay(realTime, delay);

        float targetTime = (realTime ? Time.unscaledTime : Time.time) + duration;
        float def = i.alpha;

        while (realTime ? Time.unscaledTime < targetTime : Time.time < targetTime)
        {
            i.alpha = Mathf.Lerp(targetAlpha, def, (realTime ? targetTime - Time.unscaledTime : targetTime - Time.time) / duration);
            yield return null;
        }
        i.alpha = targetAlpha;
    }

    public static IEnumerator Counter(this Text t, int start, int target, float duration = 3, float stepDuration = 0.1f, float delay = 0, bool realTime = true, Action<int> everyStep = null, Func<int, string> setFormat = null)
    {
        yield return Delay(realTime, delay);

        float targetTime = (realTime ? Time.unscaledTime : Time.time) + duration;
        float startingTime = (realTime ? Time.unscaledTime : Time.time);
        int stepValue = 0;
        int current = start;

        if (setFormat == null)
        {
            setFormat = (v) => v.ToString();
        }

        t.text = setFormat(start);// start.ToString(format);

        if (realTime)
            while (Time.unscaledTime < targetTime)
            {
                stepValue = (int)Mathf.Lerp(start, target, (Time.unscaledTime - startingTime) / (duration));
                t.text = setFormat(stepValue);// stepValue.ToString(format);
                everyStep?.Invoke(stepValue);
                yield return new WaitForSecondsRealtime(stepDuration);
            }
        else
            while (Time.time < targetTime)
            {
                stepValue = (int)Mathf.Lerp(start, target, (Time.unscaledTime - startingTime) / (duration));
                t.text = setFormat(stepValue);// stepValue.ToString(format);
                everyStep?.Invoke(stepValue);
                yield return new WaitForSeconds(stepDuration);
            }
        t.text = setFormat(target);// target.ToString(format);
        everyStep?.Invoke(stepValue);
    }

    /// <summary>
    /// Calculates the anchored position for a given Worldspace transform for a Screenspace-Camera UI
    /// </summary>
    /// <param name="viewingCamera">The worldspace camera</param>
    /// <param name="followTransform">The transform to be followed</param>
    /// <param name="canvasScaler">The canvas scaler</param>
    /// <param name="followElementRect">The rect of the UI element that will follow the transform</param>
    /// <returns></returns>
    public static Vector2 GetAnchoredPosition(this Camera viewingCamera, Vector3 followPosition, CanvasScaler scaler)
    => GetAnchoredPosition(viewingCamera, followPosition, scaler.referenceResolution, new Rect(0, 0, scaler.referenceResolution.x, scaler.referenceResolution.y));
    public static Vector2 GetAnchoredPosition(this Camera viewingCamera, Transform followTransform, Vector2 canvasScaler, Rect followElementRect)
        => GetAnchoredPosition(viewingCamera, followTransform.position, canvasScaler, followElementRect);
    public static Vector2 GetAnchoredPosition(this Camera viewingCamera, Vector3 followPosition, Vector2 canvasScaler, Rect followElementRect)
    {
        // We need to calculate the object's relative position to the camera make sure the
        // follow element's position doesn't end up getting "inverted" by WorldToViewportPoint when far away
        var relativePosition = viewingCamera.transform.InverseTransformPoint(followPosition);
        relativePosition.z = Mathf.Max(relativePosition.z, 1f);
        var viewportPos = viewingCamera.WorldToViewportPoint(viewingCamera.transform.TransformPoint(relativePosition));

        return new Vector2(viewportPos.x * canvasScaler.x - followElementRect.size.x / 2f,
                           viewportPos.y * canvasScaler.y - followElementRect.size.y / 2f);
    }

    public static Tuple<Toggle, int> GetIsOnToggle(this ToggleGroup grp)
    {
        int i = 0;
        foreach (var toggle in grp.ActiveToggles())
        {
            if (toggle.isOn)
            {
                return new Tuple<Toggle, int>(toggle, i);
            }
            i++;
        }
        return new Tuple<Toggle, int>(null, -1);
    }
}
