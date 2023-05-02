//***********************************************************************//
// Copyright (C) 2017 Bilal Arikan. All Rights Reserved.
// Author: Bilal Arikan
// Time  : 04.11.2017   
//***********************************************************************//
using System;
using System.Linq;
using Arikan;

using UnityEngine;

namespace Arikan
{
    /// <summary>
    /// Singleton pattern.
    /// </summary>
    //[ExecuteInEditMode]
    public abstract class GeneratedSingletonBehaviour<T> : MonoBehaviour, IArikanBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        /// <summary>
        /// Singleton design pattern
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                {
                    if (_instance == null)
                        _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // var isPlaced = _instance.gameObject.scene.isLoaded;
            var isPlaced = !string.IsNullOrEmpty(gameObject.scene.name);
            if (!isPlaced)
            {
                return;
            }
            if (_instance == null)
            {
                _instance = this as T;
            }
        }
    }

}
