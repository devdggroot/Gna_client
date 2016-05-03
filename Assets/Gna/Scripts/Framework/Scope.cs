using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class Scope : MonoBehaviour
{
    Character character;
    Transform root;

    LineRenderer min;
    LineRenderer max;
    LineRenderer current;

    void OnDestroy()
    {
        if(root != null)
        {
            gna.Factory.DestroyAllChild(root.transform);
            DestroyImmediate(root.gameObject);
        }

        character = null;
    }

    // Use this for initialization
    void Start()
    {
        character = GetComponent<Character>();

        root = gna.Factory.Create("Scope", character.centeredTransform);

        min = gna.Factory.Create("min", root).gameObject.AddComponent<LineRenderer>();
        min.SetWidth(0.04f, 0.04f);

        max = gna.Factory.Create("max", root).gameObject.AddComponent<LineRenderer>();
        max.SetWidth(0.04f, 0.04f);

        current = gna.Factory.Create("current", root).gameObject.AddComponent<LineRenderer>();
        current.SetWidth(0.02f, 0.02f);
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null && character.cachedTransform != null)
        {
            Vector3 lookAt = Quaternion.LookRotation(character.cachedTransform.forward, character.up) * Vector3.right;//character.cachedTransform.right;

            {
                min.SetVertexCount(2);
                min.SetPosition(0, character.centeredTransform.position);

                Vector3 direction = Quaternion.AngleAxis(character.minScopeAngle, character.cachedTransform.forward) * lookAt;
                min.SetPosition(1, character.centeredTransform.position + direction);
            }

            {
                max.SetVertexCount(2);
                max.SetPosition(0, character.centeredTransform.position);

                Vector3 direction = Quaternion.AngleAxis(character.maxScopeAngle, character.cachedTransform.forward) * lookAt;
                max.SetPosition(1, character.centeredTransform.position + direction);
            }

            {
                current.SetVertexCount(2);
                current.SetPosition(0, character.centeredTransform.position);

                Vector3 direction = Quaternion.AngleAxis(character.angle, character.cachedTransform.forward) * lookAt;
                current.SetPosition(1, character.centeredTransform.position + direction * 2f);
            }
        }
    }
}
