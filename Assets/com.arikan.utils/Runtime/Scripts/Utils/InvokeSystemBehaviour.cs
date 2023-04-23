using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class InvokeSystemBehaviour : MonoBehaviour
{
}

public static class InvokeSystem
{
    static InvokeSystemBehaviour _container;
    static InvokeSystemBehaviour Container
    {
        get
        {
            if (_container == null)
            {
                var go = new GameObject("InvokeSystemBehaviour");
                _container = go.AddComponent<InvokeSystemBehaviour>();
                GameObject.DontDestroyOnLoad(go);
            }
            return _container;
        }
    }

    /// <summary>
    /// MainThread de çağırabilmek için (Parametresiz)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="delay"></param>
    public static Coroutine Invoke(IEnumerator a)
        => StartThrowingCoroutine(a, Debug.LogException, null, Container);
    public static Coroutine Invoke(Action a, float delay = 0)
        => StartThrowingCoroutine(InvokeCo(a, delay), Debug.LogException, null, Container);
    public static Coroutine Invoke(this MonoBehaviour bhv, Action a, float delay = 0)
        => StartThrowingCoroutine(InvokeCo(a, delay), Debug.LogException, null, bhv);
    static IEnumerator InvokeCo(Action a, float delay)
    {
        yield return Delay(delay);
        a();
        yield break;
    }

    public static Coroutine InvokeRepeating(Action a, float step = 1, float duration = -1, float delay = 0, bool realTime = true)
        => StartThrowingCoroutine(InvokeRepeatingCo(a, step, duration, delay, realTime), Debug.LogException, null, Container);
    public static Coroutine InvokeRepeating(this MonoBehaviour bhv, Action a, float step = 1, float duration = -1, float delay = 0, bool realTime = true)
        => StartThrowingCoroutine(InvokeRepeatingCo(a, step, duration, delay, realTime), Debug.LogException, null, bhv);
    static IEnumerator InvokeRepeatingCo(Action a, float step, float duration, float delay, bool realTime)
    {
        yield return Delay(delay, realTime);

        float targetTime = duration <= 0 ? float.MaxValue : (realTime ? Time.unscaledTime : Time.time) + duration;
        float startingTime = (realTime ? Time.unscaledTime : Time.time);

        if (realTime)
        {
            object wait = new WaitForSecondsRealtime(step);
            while (Time.unscaledTime < targetTime)
            {
                a?.Invoke();
                yield return new WaitForSecondsRealtime(step); ;
            }
        }
        else
        {
            object wait = new WaitForSeconds(step);
            while (Time.time < targetTime)
            {
                a?.Invoke();
                yield return new WaitForSeconds(step);
            }
        }
        a?.Invoke();
        yield break;
    }

    public static void InvokeUntil(Action a, Func<bool> wait)
        => StartThrowingCoroutine(InvokeUntilCo(a, wait), Debug.LogException, null, Container);
    public static void InvokeUntil(this MonoBehaviour bhv, Action a, Func<bool> wait)
        => StartThrowingCoroutine(InvokeUntilCo(a, wait), Debug.LogException, null, bhv);
    static IEnumerator InvokeUntilCo(Action a, Func<bool> wait)
    {
        yield return new WaitUntil(wait);
        a.Invoke();
        yield break;
    }


    /// <summary>
    /// Checks for playing coroutines and stops them.
    /// </summary>
    /// <param name="mono">Mono.</param>
    /// <param name="buttonCoroutines">Button coroutines.</param>
    public static Coroutine StopAndStartCoroutine(this MonoBehaviour mono, IEnumerator toStart, List<Coroutine> routines) => StopAndStartCoroutine(mono, toStart, routines.ToArray());
    public static Coroutine StopAndStartCoroutine(this MonoBehaviour mono, IEnumerator toStart, params Coroutine[] routines)
    {
        for (int i = 0; i < routines.Length; i++)
            if (routines[i] != null)
                mono.StopCoroutine(routines[i]);
        return StartThrowingCoroutine(toStart, Debug.LogException, null, mono);
    }

    public static void StopCoroutineNullCheck(this MonoBehaviour mono, params Coroutine[] routines)
    {
        for (int i = 0; i < routines.Length; i++)
            if (routines[i] != null)
                mono.StopCoroutine(routines[i]);
    }
    public static IEnumerator Delay(float seconds = 0, bool realTime = true)
    {
        if (seconds <= 0)
            yield break;
        else if (realTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
    }
    public static IEnumerator Delay(this MonoBehaviour mono, float seconds = 0, bool realTime = true)
    {
        if (seconds <= 0)
            yield break;
        else if (realTime)
            yield return new WaitForSecondsRealtime(seconds);
        else
            yield return new WaitForSeconds(seconds);
    }

    /// <summary>
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="finished"></param>
    /// <param name="left"></param>
    /// <param name="realTime"></param>
    public static Coroutine InvokeTimer(this MonoBehaviour Instance, float seconds, Action finished, Action<float> left = null, bool realTime = false)
    {
        if (realTime)
            return StartThrowingCoroutine(TimerUnscaledCo(seconds, finished, left), Debug.LogException, null, Instance);
        else
            return StartThrowingCoroutine(TimerCo(seconds, finished, left), Debug.LogException, null, Instance);
    }
    static IEnumerator TimerCo(float seconds, Action finished, Action<float> left = null)
    {
        float target = Time.time + seconds;
        if (left != null)
        {
            var rule = new WaitForEndOfFrame();
            while (Time.time < target)
            {
                left.Invoke(target - Time.time);
                yield return rule;
            }
        }
        else
            yield return new WaitForSeconds(seconds);
        finished.Invoke();
        yield break;
    }
    static IEnumerator TimerUnscaledCo(float seconds, Action finished, Action<float> left = null)
    {
        float target = Time.unscaledTime + seconds;
        if (left != null)
        {
            var rule = new WaitForEndOfFrame();
            while (Time.unscaledTime < target)
            {
                left.Invoke(target - Time.unscaledTime);
                yield return rule;
            }
        }
        else
            yield return new WaitForSecondsRealtime(seconds);
        finished.Invoke();
        yield break;
    }

    private class ExceptionContainer
    {
        public Exception Exception = null;
    }

    public static Coroutine StartThrowingCoroutine(
        IEnumerator enumerator,
        Action<Exception> onException,
        Action onSuccess,
        MonoBehaviour mono = null,
        float coroutineAbortTime = 40f
    )
    {
        var exceptionHandler = onException ?? Debug.LogError;
        return Container.StartCoroutine(RunThrowingIterator(enumerator, exceptionHandler, onSuccess, mono ?? Container, new ExceptionContainer(), coroutineAbortTime));
    }
    private static IEnumerator RunThrowingIterator(
            IEnumerator enumerator,
            Action<Exception> onException,
            Action onSuccess,
            MonoBehaviour mono,
            ExceptionContainer excContainer,
            float coroutineAbortTime,
            int floor = 0
        )
    {
        if (floor > 1000)
        {
            Debug.LogError("Coroutine floor more than 1000 iterations:" + floor);
        }
        float coroutineStartTime = Time.unscaledTime;

        while (excContainer.Exception == null)
        {
            object current;
            try
            {
                if (enumerator.MoveNext() == false)
                {
                    break;
                }
                if (coroutineAbortTime > 0 && Time.unscaledTime - coroutineStartTime > coroutineAbortTime)
                {
                    throw new Exception($"Coroutine Duration > {coroutineAbortTime} : {mono}");
                }
                current = enumerator.Current;
            }
            catch (Exception ex)
            {
                excContainer.Exception = ex;
                break;
            }
            // Debug.Log(floor + "_" + current?.GetType() + "_" + (excContainer.Exception == null), mono);
            if (current is IEnumerator)
            {
                // Debug.Log("1_" + floor + "_" + current?.GetType() + "_" + (excContainer.Exception == null), mono);
                yield return RunThrowingIterator(
                    (IEnumerator)current,
                    (e) => { },
                    () => { },
                    mono,
                    excContainer,
                    coroutineAbortTime,
                    floor + 1
                );
            }
            else
            {
                yield return current;
            }
        }
        if (excContainer.Exception != null)
        {
            // Debug.LogError(floor + "_" + excContainer.Exception);
            onException?.Invoke(excContainer.Exception);
        }
        else
        {
            // Debug.Log("2_" + floor + "_" + onSuccess + "_" + mono);
            onSuccess?.Invoke();
        }
        yield break;
    }
}
