using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class FireScope : MonoBehaviour
{
    Character character;
    GameObject root;

    LineRenderer min;
    LineRenderer max;
    LineRenderer current;

    void OnDestroy()
    {
        if(root != null)
        {
            gna.Factory.DestroyAllChild(root.transform);
            DestroyImmediate(root);
        }

        character = null;
    }

    // Use this for initialization
    void Start()
    {
        character = GetComponent<Character>();

        root = gna.Factory.CreateGameObject("[Scope] Root", transform);

        min = gna.Factory.CreateGameObject("min", root.transform).AddComponent<LineRenderer>();
        min.SetWidth(0.04f, 0.04f);

        max = gna.Factory.CreateGameObject("max", root.transform).AddComponent<LineRenderer>();
        max.SetWidth(0.04f, 0.04f);

        current = gna.Factory.CreateGameObject("current", root.transform).AddComponent<LineRenderer>();
        current.SetWidth(0.02f, 0.02f);
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null && character.cachedTransform != null)
        {
            Vector3 lookAt = character.cachedTransform.right;

            {
                min.SetVertexCount(2);
                min.SetPosition(0, character.cachedTransform.position + Vector3.back);

                Vector3 direction = Quaternion.AngleAxis(character.minimumFireAngle, character.cachedTransform.forward) * lookAt;
                min.SetPosition(1, character.cachedTransform.position + direction + Vector3.back);
            }

            {
                max.SetVertexCount(2);
                max.SetPosition(0, character.cachedTransform.position + Vector3.back);

                Vector3 direction = Quaternion.AngleAxis(character.maximumFireAngle, character.cachedTransform.forward) * lookAt;
                max.SetPosition(1, character.cachedTransform.position + direction + Vector3.back);
            }

            {
                current.SetVertexCount(2);
                current.SetPosition(0, character.cachedTransform.position + Vector3.back);

                Vector3 direction = Quaternion.AngleAxis(character.currentFireAngle, character.cachedTransform.forward) * lookAt;
                current.SetPosition(1, character.cachedTransform.position + direction * 2f + Vector3.back);
            }
        }
    }
}
