using System;
using System.Collections;
using UnityEngine;

public static partial class ArikanExtensions
{
    public static T CopyComponent<T>(this T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

    public static void CopyValues<T>(this T obj, T from) where T : ScriptableObject
    {
        var fields = typeof(T).GetFields();
        foreach (var f in fields)
        {
            f.SetValue(obj, f.GetValue(from));
        }
    }

    public static bool TryGetComponentInParent<T>(this GameObject obj, out T component, bool includeInactive = false)
    {
        component = obj.GetComponentInParent<T>(includeInactive);
        return component != null;
    }

    public static bool TryGetComponentInParent<T>(this Component obj, out T component, bool includeInactive = false)
    {
        component = obj.GetComponentInParent<T>();
        return component != null;
    }

    public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component, bool includeInactive = false)
    {
        component = obj.GetComponentInChildren<T>(includeInactive);
        return component != null;
    }

    public static bool TryGetComponentInChildren<T>(this Component obj, out T component, bool includeInactive = false)
    {
        component = obj.GetComponentInChildren<T>(includeInactive);
        return component != null;
    }

    public static void RemoveComponent<T>(this Component obj) where T : Component
    {
        if (obj.TryGetComponent<T>(out var c))
        {
            if (!Application.isPlaying)
                GameObject.DestroyImmediate(c);
            else
                GameObject.Destroy(c);
        }
    }
    public static void RemoveComponent<T>(this GameObject obj) where T : Component
    {
        if (obj.TryGetComponent<T>(out var c))
        {
            if (!Application.isPlaying)
                GameObject.DestroyImmediate(c);
            else
                GameObject.Destroy(c);
        }
    }
}
