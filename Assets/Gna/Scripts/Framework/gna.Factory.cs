using UnityEngine;
using System.Collections;

namespace gna
{
    public static class Factory
    {
        public static Transform Create(string name, Transform parent = null)
        {
            GameObject go = new GameObject(name);

            Transform transform = go.transform;
            transform.parent = parent;

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;

            return go.transform;
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

        public static Transform Search(string name, Transform root)
        {
            if (root != null)
            {
                if (root.name == name)
                    return root;

                for ( int i = 0, imax = root.childCount; i < imax; ++i)
                {
                    Transform transform = Search(name, root.GetChild(i));
                    if (transform != null)
                        return transform;
                }
            }
            return null;
        }
    }
}

