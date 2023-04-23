// Original FindMissingScriptsRecursively script by SimTex and Clement
// http://wiki.unity3d.com/index.php?title=FindMissingScripts

#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Arikan
{
    public static class FindMissingScriptsRecursively
    {
        static int go_count = 0, components_count = 0, missing_count = 0;

        /// <summary>
        /// Any gameObject must be selected
        /// </summary>
        [MenuItem("Arikan/Find Missing Scripts (Selected)")]
        private static void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in go)
            {
                FindInGO(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }

        [MenuItem("Arikan/Find Missing Scripts (All Scenes)")]
        private static void FindInAllScene()
        {
            var sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
            var roots = Enumerable.Range(0, sceneCount).SelectMany(i => UnityEngine.SceneManagement.SceneManager.GetSceneAt(i).GetRootGameObjects());
            go_count = 0;
            components_count = 0;
            missing_count = 0;
            foreach (GameObject g in roots)
            {
                FindInGO(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
        }

        private static void FindInGO(GameObject g)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }
            // Now recurse through each child GO (if there are any):
            foreach (Transform childT in g.transform)
            {
                //Debug.Log("Searching " + childT.name  + " " );
                FindInGO(childT.gameObject);
            }
        }
    }
}
#endif