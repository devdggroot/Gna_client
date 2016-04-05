using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(PixelRigidbody), true)]
public class RigidbodyInspector : Editor
{
    PixelRigidbody rigidbody;

    void OnEnable()
    {
        rigidbody = target as PixelRigidbody;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        rigidbody.COR = EditorGUILayout.Slider("COR", rigidbody.COR, 0f, 1f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        rigidbody.GravityScale = EditorGUILayout.Slider("Gravity Scale", rigidbody.GravityScale, 0f, 10f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        rigidbody.LimitClimbAngle = EditorGUILayout.Slider("Limit ClimbAngle", rigidbody.LimitClimbAngle, 0f, 90f);
        EditorGUILayout.EndHorizontal();
    }
}
