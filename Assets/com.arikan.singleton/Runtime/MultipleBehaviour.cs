//***********************************************************************//
// Author: Bilal Arikan
// Time  : 27.11.2018   
//***********************************************************************//
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Sahnede Birden fazla ayni component bulunacaksa bununla kullanilabilir
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class MultipleBehaviour<T> : SerializedMonoBehaviour, IArikanBehaviour where T : SerializedMonoBehaviour
{
    [ShowInInspector]
    [ReadOnly]
    [PropertySpace(0, 12)]
    public static HashSet<T> Instances = new HashSet<T>();

    protected virtual void OnEnable()
    {
        // var isPlaced = _instance.gameObject.scene.isLoaded;
        var isPlaced = !string.IsNullOrEmpty(gameObject.scene.name);
        if (!isPlaced)
        {
            return;
        }
        Instances.Add(this as T);
    }

#if !UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (Application.isPlaying)
        {
            Instances.Add(this as T);
            Instances.RemoveWhere(a => a == null);
        }
    }
#else
    protected virtual void OnValidate() { }
#endif

    protected virtual void OnDisable()
    {
        Instances.Remove(this as T);
    }
}
