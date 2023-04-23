//***********************************************************************//
// Author: Bilal Arikan
// Time  : 04.11.2017   
//***********************************************************************//
using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Arikan
{
    public abstract class GeneratedSingletonScriptableObject<T> : SerializedScriptableObject where T : SerializedScriptableObject //SharedInstance<T> where T : SharedInstance<T>
    {
        [ShowInInspector]
        [ReadOnly]
        [PropertySpace(0, 12)]
        protected static T _instance = null;
        public static T Instance
        {
            get
            {
                /*if (Application.isEditor)
                    return null;*/
                if (!_instance)
                {
                    _instance = Resources_Load();
                }
#if UNITY_EDITOR
                if (!_instance)
                {
                    _instance = GenerateConfigFile();
                }
#endif
                if (!_instance)
                {
                    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                }
                if (ShowErrorOnNull && !_instance)
                {
                    Debug.LogError(typeof(T).Name + ": GeneratedSingletonScriptableObject Could not generate!");
                }
                return _instance;
            }
        }

        protected static T Resources_Load()
        {
            var path = ((ResourcePathAttribute[])typeof(T).GetCustomAttributes(typeof(ResourcePathAttribute), true)).FirstOrDefault()?.Path;
            if (!string.IsNullOrEmpty(path))
            {
                return Resources.Load<T>(path);
            }
            else
            {
                return Resources.Load<T>(typeof(T).Name);
                // var loaded = Resources.Load<T>(typeof(T).Name);
                // if (loaded == null)
                // {
                //     loaded = Resources.Load<T>(SingletonUtils.ConfigsResourcesPath + typeof(T).Name);
                // }
                // return loaded;
            }
        }

        protected static T GenerateConfigFile()
        {
#if UNITY_EDITOR
            var path = ((ResourcePathAttribute[])typeof(T).GetCustomAttributes(typeof(ResourcePathAttribute), true)).FirstOrDefault()?.Path;
            if (!string.IsNullOrEmpty(path))
            {
                var subPaths = path.Split('/');
                var resPath = "Assets/Resources/" + string.Join("/", subPaths.Take(subPaths.Length - 1)) + "/";
                var objName = subPaths.Last();
                return SingletonUtils.GetOrCreateConfigFile<T>(resPath, objName);
            }
            else
            {
                return SingletonUtils.GetOrCreateConfigFile<T>(SingletonUtils.ConfigsFolder);
            }
#else
            Debug.LogError("GenerateConfigFile is not supported in runtime mode!");
            return null;
#endif
        }

        public static bool ShowErrorOnNull = true;
    }
}
