using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(Body), true)]
public class RigidbodyInspector : Editor
{
    Body body;

    void OnEnable()
    {
        body = target as Body;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        body.COR = EditorGUILayout.Slider("COR", body.COR, 0f, 1f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        body.GravityScale = EditorGUILayout.Slider("Gravity Scale", body.GravityScale, 0f, 10f);
        EditorGUILayout.EndHorizontal();

        /*EditorGUILayout.BeginHorizontal();
        body.LimitClimbAngle = EditorGUILayout.Slider("Limit ClimbAngle", body.LimitClimbAngle, 0f, 90f);
        EditorGUILayout.EndHorizontal();*/
    }
}
