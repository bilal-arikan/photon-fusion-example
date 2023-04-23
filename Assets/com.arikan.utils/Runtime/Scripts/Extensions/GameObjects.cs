// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameObjects.cs" company="Nick Prühs">
//   Copyright (c) Nick Prühs. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameObjects
{
    /// <summary>
    ///   Instantiates a new game object and parents it to this one.
    ///   Resets position, rotation and scale and inherits the layer.
    /// </summary>
    /// <param name="parent">Game object to add the child to.</param>
    /// <returns>New child.</returns>
    public static GameObject AddChild(this GameObject parent)
    {
        return parent.AddChild("NewGameObject");
    }

    /// <summary>
    ///   Instantiates a new game object and parents it to this one.
    ///   Resets position, rotation and scale and inherits the layer.
    /// </summary>
    /// <param name="parent">Game object to add the child to.</param>
    /// <param name="name">Name of the child to add.</param>
    /// <returns>New child.</returns>
    public static GameObject AddChild(this GameObject parent, string name)
    {
        var go = AddChild(parent, (GameObject)null);
        go.name = name;
        return go;
    }

    /// <summary>
    ///   Instantiates a prefab and parents it to this one.
    ///   Resets position, rotation and scale and inherits the layer.
    /// </summary>
    /// <param name="parent">Game object to add the child to.</param>
    /// <param name="prefab">Prefab to instantiate.</param>
    /// <returns>New prefab instance.</returns>
    public static GameObject AddChild(this GameObject parent, GameObject prefab)
    {
        var go = prefab != null ? Object.Instantiate(prefab) : new GameObject();
        if (go == null || parent == null)
        {
            return go;
        }

        var transform = go.transform;
        transform.SetParent(parent.transform);
        transform.Reset();
        go.layer = parent.layer;
        return go;
    }

    /// <summary>
    ///   Destroys all children of a object.
    /// </summary>
    /// <param name="gameObject">Game object to destroy all children of.</param>
    public static void DestroyChildren(this GameObject gameObject)
    {
        foreach (var child in gameObject.GetChildren())
        {
            // Hide immediately.
            // child.SetActive(false);
            if (Application.isEditor && !Application.isPlaying)
            {
                Object.DestroyImmediate(child);
            }
            else
            {
                Object.Destroy(child);
            }
        }
    }

    /// <summary>
    /// Moves an object from point A to point B in a given time
    /// </summary>
    /// <param name="movingObject">Moving object.</param>
    /// <param name="pointA">Point a.</param>
    /// <param name="pointB">Point b.</param>
    /// <param name="time">Time.</param>
    public static IEnumerator MoveFromTo(this GameObject movingObject, Vector3 pointA, Vector3 pointB, float time, float approximationDistance)
    {
        float t = 0f;

        float distance = Vector3.Distance(movingObject.transform.position, pointB);

        while (distance >= approximationDistance)
        {
            distance = Vector3.Distance(movingObject.transform.position, pointB);
            t += Time.deltaTime / time;
            movingObject.transform.position = Vector3.Lerp(pointA, pointB, t);
            yield return 0;
        }
        yield break;
    }

    /// <summary>
    ///   Gets the component of type <typeparamref name="T" /> if the game object has one attached,
    ///   and adds and returns a new one if it doesn't.
    /// </summary>
    /// <typeparam name="T">Type of the component to get or add.</typeparam>
    /// <param name="gameObject">Game object to get the component of.</param>
    /// <returns>
    ///   Component of type <typeparamref name="T" /> attached to the game object.
    /// </returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var c = gameObject.GetComponent<T>();
        if (c)
            return c;
        else
            return gameObject.AddComponent<T>();
    }

    /// <summary>
    ///   Returns the full path of a game object, i.e. the names of all
    ///   ancestors and the game object itself.
    /// </summary>
    /// <param name="gameObject">Game object to get the path of.</param>
    /// <returns>Full path of the game object.</returns>
    public static string GetPath(this GameObject gameObject)
    {
        return
            gameObject.GetAncestorsAndSelf()
                .Reverse()
                .Aggregate(string.Empty, (path, go) => path + "/" + go.name)
                .Substring(1);
    }

    /// <summary>
    ///   Sets the layer of the game object.
    /// </summary>
    /// <param name="gameObject">Game object to set the layer of.</param>
    /// <param name="layerName">Name of the new layer.</param>
    public static void SetLayer(this GameObject gameObject, string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    ///   Sets the layers of all queried game objects.
    /// </summary>
    /// <param name="gameObjects">Game objects to set the layers of.</param>
    /// <param name="layerName">Name of the new layer.</param>
    /// <returns>Query for further execution.</returns>
    public static List<GameObject> SetLayers(this List<GameObject> gameObjects, string layerName)
    {
        var layer = LayerMask.NameToLayer(layerName);
        foreach (var o in gameObjects)
        {
            o.layer = layer;
        }
        return gameObjects;
    }

    /// <summary>
    ///   Sets the tags of all queried game objects.
    /// </summary>
    /// <param name="gameObjects">Game objects to set the tags of.</param>
    /// <param name="tag">Name of the new tag.</param>
    /// <returns>Query for further execution.</returns>
    public static List<GameObject> SetTags(this List<GameObject> gameObjects, string tag)
    {
        foreach (var gameObject in gameObjects)
        {
            gameObject.tag = tag;
        }
        return gameObjects;
    }

    public static void SetChildActiveOnly(this GameObject parent, int index)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(i == index);
        }
    }

    /// <summary>
    /// Same with Destroy(gameobject);
    /// </summary>
    /// <param name="gameObject"></param>
    public static void Destroy(this GameObject gameObject)
    {
#if !UNITY_EDITOR
            MonoBehaviour.Destroy(gameObject);
#else
        if (Application.isPlaying)
            MonoBehaviour.Destroy(gameObject);
        else
            MonoBehaviour.DestroyImmediate(gameObject);
#endif
    }

    public static T GetComponentInNeighbours<T>(this UnityEngine.Component c, bool includeInactive = true) where T : Component
    {
        if (!c.transform.parent)
        {
            Debug.LogError(c + " No Parent Object !", c);
            return null;
        }

        foreach (Transform n in c.transform.parent)
        {
            var t = n.GetComponent<T>();
            if (t)
                return t;
        }
        return null;
    }
    public static T[] GetComponentsInNeighbours<T>(this UnityEngine.Component c, bool includeInactive = true) where T : Component
    {
        if (!c.transform.parent)
        {
            Debug.LogError(c + " No Parent Object !", c);
            return new T[0];
        }

        List<T> objs = new List<T>();
        foreach (Transform n in c.transform.parent)
        {
            var t = n.GetComponent<T>();
            if (t)
                objs.Add(t);
        }

        return objs.ToArray();
    }

    /// <summary>
    /// UIManager da ki UIView arama fonksiyonunu Generic ve Extension hali
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    public static T[] FindObjectsInScene<T>(this UnityEngine.Object c, Scene s)// where T : Component
    {
        List<T> objs = new List<T>();
        foreach (var o in s.GetRootGameObjects())
            objs.AddRange(o.GetComponentsInChildren<T>(true));
        return objs.ToArray();
    }

    /// <summary>
    /// UIManager da ki UIView arama fonksiyonunu Generic ve Extension hali
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <returns></returns>
    public static T[] FindObjects<T>(this UnityEngine.Object c, bool includeInactive = true)// where T : Component
    {
        List<T> objs = new List<T>();
        var roots = new List<GameObject>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
            roots.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
        foreach (var o in roots)
        {
            objs.AddRange(o.GetComponentsInChildren<T>(includeInactive));
        }
        return objs.ToArray();
    }
}
