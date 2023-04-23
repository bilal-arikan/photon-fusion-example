using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Arikan
{
    public static partial class EditorUtils
    {
#if UNITY_EDITOR
        public static bool IsCalledInPrefab(Component comp) => IsCalledInPrefab(comp.gameObject);
        public static bool IsCalledInPrefab(GameObject gameObject)
        {
            var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
            bool prefabConnected = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject) == UnityEditor.PrefabInstanceStatus.Connected;
            if (!isValidPrefabStage && prefabConnected)
            {
                //Variables you only want checked when in a Scene
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Adds newly (if not already in the list) found assets.
        /// Returns how many found (not how many added)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="assetsFound">Adds to this list if it is not already there</param>
        /// <returns></returns>
        public static int TryGetUnityObjectsOfTypeFromPath<T>(string path, List<T> assetsFound) where T : UnityEngine.Object
        {
            string[] subFolderPaths = System.IO.Directory.GetDirectories(path);
            foreach (var f in subFolderPaths)
            {
                TryGetUnityObjectsOfTypeFromPath(f, assetsFound);
            }

            string[] filePaths = System.IO.Directory.GetFiles(path);

            Debug.Log(filePaths.Length);
            int countFound = 0;

            if (filePaths != null && filePaths.Length > 0)
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(UnityEngine.Object));
                    if (obj is T asset)
                    {
                        Debug.Log("F");
                        countFound++;
                        if (!assetsFound.Contains(asset))
                        {
                            assetsFound.Add(asset);
                        }
                    }
                }
            }

            return countFound;
        }

        public static bool ExtractMaterials(this ModelImporter importer, string destinationPath)
        {
            var assetsToReload = new HashSet<string>();

            var materials = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath).Where(x => x.GetType() == typeof(Material)).ToArray();

            foreach (var material in materials)
            {
                var newAssetPath = Path.Combine(destinationPath, material.name) + ".mat";
                newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

                var error = AssetDatabase.ExtractAsset(material, newAssetPath);
                if (string.IsNullOrEmpty(error))
                {
                    assetsToReload.Add(importer.assetPath);
                }
            }

            foreach (var path in assetsToReload)
            {
                AssetDatabase.WriteImportSettingsIfDirty(path);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            return assetsToReload.Count > 0;
        }

        public static RuntimePlatform? TryConvertToRuntimePlatform(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.Android:
                    return RuntimePlatform.Android;
                case BuildTarget.PS4:
                    return RuntimePlatform.PS4;
                case BuildTarget.PS5:
                    return RuntimePlatform.PS5;
                case BuildTarget.StandaloneLinux64:
                    return RuntimePlatform.LinuxPlayer;
                case BuildTarget.StandaloneOSX:
                    return RuntimePlatform.OSXPlayer;
                case BuildTarget.StandaloneWindows:
                    return RuntimePlatform.WindowsPlayer;
                case BuildTarget.StandaloneWindows64:
                    return RuntimePlatform.WindowsPlayer;
                case BuildTarget.Switch:
                    return RuntimePlatform.Switch;
                case BuildTarget.WSAPlayer:
                    return RuntimePlatform.WSAPlayerARM;
                case BuildTarget.XboxOne:
                    return RuntimePlatform.XboxOne;
                case BuildTarget.iOS:
                    return RuntimePlatform.IPhonePlayer;
                case BuildTarget.tvOS:
                    return RuntimePlatform.tvOS;
                case BuildTarget.WebGL:
                    return RuntimePlatform.WebGLPlayer;
                case BuildTarget.Lumin:
                    return RuntimePlatform.Lumin;
                case BuildTarget.GameCoreXboxSeries:
                    return RuntimePlatform.GameCoreXboxSeries;
                case BuildTarget.GameCoreXboxOne:
                    return RuntimePlatform.GameCoreXboxOne;
                case BuildTarget.Stadia:
                    return RuntimePlatform.Stadia;
                default:
                    return null;
            }
        }
#endif
    }
#if UNITY_EDITOR
    public static class PrefabUtilityExt
    {
        public static T InstantiatePrefab<T>(T prefab, Vector3 position, Quaternion rotation, Transform transform) where T : UnityEngine.Component
        {
            var obj = PrefabUtility.InstantiatePrefab(prefab) as T;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(transform, true);
            return obj;
        }
    }
#endif
}
