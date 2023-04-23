using UnityEngine;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Arikan
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourcePathAttribute : Attribute
    {
        public string Path;

        public ResourcePathAttribute(string path)
        {
            Path = path;
        }
    }

    public static class SingletonUtils
    {
        public const string ConfigsFolder = "Assets/Resources/Arikan/";
        public const string ConfigsResourcesPath = "Arikan/";
        public const string ConfigsMenuItem = "Arikan/Arikan Configs/";

#if UNITY_EDITOR
        public static T GetOrCreateConfigFile<T>(string path) where T : ScriptableObject
            => GetOrCreateConfigFile<T>(path, typeof(T).Name);
        public static T GetOrCreateConfigFile<T>(string path, string objName) where T : ScriptableObject
        {
            T config = null;
            var assets = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).FullName);
            if (assets.Length == 0)
            {
                if (Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }

                config = ScriptableObject.CreateInstance<T>();

                var validateMethod = typeof(T).GetMethod("OnValidate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                if (validateMethod != null)
                {
                    validateMethod.Invoke(config, null);
                }

                UnityEditor.AssetDatabase.CreateAsset(config, path + objName + ".asset");
                UnityEditor.AssetDatabase.SaveAssets();
                Debug.Log(objName + " Generated in " + path, config);
            }
            else
            {
                var foundPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
                config = UnityEditor.AssetDatabase.LoadAssetAtPath(foundPath, typeof(T)) as T;
                Debug.Log(objName + " Found in " + foundPath, config);
            }

            var _instanceField = typeof(T).GetField("_instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (_instanceField != null)
            {
                _instanceField.SetValue(null, config);
            }

            UnityEditor.Selection.activeObject = config;
            return config;
        }
#endif
    }
}
