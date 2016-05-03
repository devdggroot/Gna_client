using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(Body), true)]
public class BodyEditor : Editor
{
    Body body;

    void OnEnable()
    {
        body = target as Body;
    }

    void DrawInspector()
    {
        EditorGUILayout.BeginVertical();

        body.COR = EditorGUILayout.Slider("COR", body.COR, 0f, 1f);
        body.GravityScale = EditorGUILayout.Slider("Gravity Scale", body.GravityScale, 0f, 10f);
        body.mass = EditorGUILayout.Slider("Mass", body.mass, 1f, 10f);
        body.radius = EditorGUILayout.Slider("Radius", body.radius, 0.1f, 10f);

        EditorGUILayout.Separator();

        GUI.enabled = false;
        EditorGUILayout.Vector3Field("Acceleration", body.acceleration);
        EditorGUILayout.Vector3Field("Velocity", body.velocity);
        EditorGUILayout.EnumPopup("Body State", body.state);
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }

    public override void OnInspectorGUI()
    {
        if (body == null)
            body = target as Body;

        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        DrawInspector();
    }
}
