//***********************************************************************//
// Copyright (C) 2017 Bilal Arikan. All Rights Reserved.
// Author: Bilal Arikan
// Time  : 04.11.2017   
//***********************************************************************//
using System;
using System.Linq;
using System.Reflection;
using Arikan;

using UnityEngine;

namespace Arikan
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DontSearchOnResourcesAttribute : Attribute { }

    /// <summary>
    /// Singleton pattern.
    /// </summary>
    //[ExecuteInEditMode]
    public abstract class SingletonBehaviour<T> : MonoBehaviour, IArikanBehaviour where T : MonoBehaviour
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
                if (_instance == null && typeof(T).GetCustomAttribute(typeof(DontSearchOnResourcesAttribute), false) == null)
                {
#if Debug
                    Debug.Log("Resources.FindObjectsOfTypeAll " + typeof(T).Name);
#endif
                    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                    // if (_instance == null)
                    // {
                    //     GameObject obj = new GameObject ("_"+typeof(T).Name);
                    //     //obj.hideFlags = HideFlags.HideAndDontSave;
                    //     _instance = obj.AddComponent<T> ();
                    // }
                }
                return _instance;
            }
        }

        public SingletonBehaviour() : base()
        {
#if UNITY_EDITOR
            if (_instance == null)
                _instance = this as T;
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_instance != this)
                return;
            if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out string guid, out long id) && id > 0)
            {
                // Debug.Log("Instance is an Asset", this);
                _instance = null;
                return;
            }
        }
#endif

        /// <summary>
        /// On awake, we initialize our instance. Make sure to call base.Awake() in override if you need awake.
        /// </summary>
        protected virtual void Awake()
        {
#if UNITY_EDITOR
            if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this, out string guid, out long id) && id > 0)
            {
                return;
            }
#endif
            // var isPlaced = _instance.gameObject.scene.isLoaded;
            var isPlaced = !string.IsNullOrEmpty(gameObject.scene.name);
            if (!isPlaced)
            {
                return;
            }

            if (_instance != null && _instance != this)
            {
                // Kendini yok et
                // Destroy(gameObject);
                Debug.LogError(typeof(T).Name + ": There is Another Instance :" + _instance.name + ":" + _instance.gameObject.scene.name + ":" + _instance.gameObject.scene.isLoaded + ":" + _instance.gameObject.scene.IsValid(), _instance);
            }
            else
            {
                _instance = this as T;
            }
        }
        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                // Debug.Log("Singleton Destroyed and Nulled" + GetType().Name);
            }
            // else
            // {
            //     Debug.Log("Singleton Destroyed " + GetType().Name);
            // }
        }
    }
}

