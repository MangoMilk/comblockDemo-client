using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Unity中GameObject的一些工具
/// </summary>
public static class GameObjectUtils
{
    public static void SetLayerRecursively(Transform root, int layer = 0)
    {
        if (root != null)
        {
            root.gameObject.layer = layer;

            foreach (Transform child in root)
            {
                SetLayerRecursively(child, layer);
            }
        }
    }
    public static T GetComponentInChildrenByName<T>(string gameObjectName, Transform parent) where T : Component
    {
        GameObject go = GetGameObjectInChildren(gameObjectName, parent);
        T comp = null;
        if (go != null)
        {
            comp = go.GetComponent<T>();
        }
        return comp;
    }
    /// <summary>
    /// Find a GameObject called gameobjectname in mine Transform or others
    /// </summary>
    /// <param name="gameObjectName"></param>
    /// <param name="t">，</param>
    /// <returns> </returns>
    public static GameObject GetGameObjectInChildren(string gameObjectName, Transform t)
    {
        if (t == null)
            throw new ArgumentNullException("t");

        foreach (Transform child in t)
        {
            if (child.name == gameObjectName)
            {
                return child.gameObject;
            }
            GameObject go = GetGameObjectInChildren(gameObjectName, child);

            if (go != null)
            {
                return go;
            }
        }
        return null;
    }
}
