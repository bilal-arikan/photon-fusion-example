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
    public abstract class SingletonScriptableObject<T> : SerializedScriptableObject where T : SerializedScriptableObject //SharedInstance<T> where T : SharedInstance<T>
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
                if (!_instance)
                {
                    _instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                }
                /*if (!_instance)
                {
                    _instance = Resources.Load<T>(string.Empty);
                }
                if (!_instance)
                {
                    // !!! neredeyse bütün Resources ları yüklüyor (Sıkıntı)
                    _instance = Resources.LoadAll<T>(string.Empty).FirstOrDefault();
                }*/
                if (ShowErrorOnNull && !_instance)
                {
                    Debug.LogError(typeof(T).Name + ": SingletonScriptableObject Could not find!");
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

        public static bool ShowErrorOnNull = true;
    }
}
