#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Arikan.Editor
{
    public static class EditorUtils
    {
        [MenuItem("Arikan/GetFileGuid")]
        public static void GetFileGuid()
        {
            Debug.Log(string.Join("\n", UnityEditor.Selection.assetGUIDs));
            Debug.Log(string.Join("\n", UnityEditor.Selection.instanceIDs));
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(UnityEditor.Selection.activeObject, out string guid, out long id);
            Debug.Log(guid + "\n" + id);
        }

#if UNITY_EDITOR
#if ODIN_INSPECTOR
        [UnityEditor.MenuItem("Arikan/Thumbnail to PNG")]
        public static void SaveThumbnailToFile()
        {
            var tex = Sirenix.Utilities.Editor.GUIHelper.GetAssetThumbnail(Selection.activeObject, typeof(GameObject), true);

            byte[] bytes;
            bytes = tex.EncodeToPNG();

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets/" + Selection.activeObject.name + ".png";
            }
            else
            {
                path += ".png";
            }
            System.IO.File.WriteAllBytes(path, bytes);
            Texture2D.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
            Debug.Log("Saved to " + path, Selection.activeObject);
        }
#endif
#endif

        /// <summary>
        /// Get a list of all the build constraint symbols defined in the Android build target group.
        /// </summary>
        /// <returns>List of defined build constraint symbols.</returns>
        private static List<string> GetAndroidDefinesList()
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(';').ToList();
        }

        /// <summary>
        /// Get a list of all the build constraint symbols defined in the iOS build target group.
        /// </summary>
        /// <returns>List of defined build constraint symbols.</returns>
        private static List<string> GetiOSDefinesList()
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(';').ToList();
        }

        public static void CreateTag(string tag)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // First check if it is not already present
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tag)) { found = true; break; }
            }

            // if not found, add it
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tag;
            }
            tagManager.ApplyModifiedProperties();
        }

        // TODO
        // public static void CreateLayerMask(string layer)
        // {
        //     // Open tag manager
        //     SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        //     // For Unity 5 we need this too
        //     SerializedProperty layersProp = tagManager.FindProperty("layers");

        //     // --- Unity 5 ---
        //     SerializedProperty sp = layersProp.GetArrayElementAtIndex(10);
        //     if (sp != null) sp.stringValue = layer;
        //     // and to save the changes
        //     tagManager.ApplyModifiedProperties();
        // }
    }
}
#endif
