using UnityEngine;
using System.Collections;

namespace gna
{
    public static class Factory
    {
        public static GameObject CreateGameObject(string name, Transform parent = null)
        {
            GameObject go = new GameObject(name);

            Transform transform = go.transform;
            transform.parent = parent;

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;

            return go;
        }

        public static void DestroyAllChild(Transform root)
        {
            while (root != null && root.childCount > 0)
                GameObject.DestroyImmediate(root.GetChild(0).gameObject);
        }

        public static Transform AddChild(Transform child, Transform parent)
        {
            if (child != null)
            {
                child.parent = parent;

                child.localPosition = Vector3.zero;
                child.localScale = Vector3.one;
                child.localRotation = Quaternion.identity;
            }

            return child;
        }
    }
}

