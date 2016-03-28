using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(NormalRenderer))]
public class NormalRendererInspector : Editor
{
    NormalRenderer normalRenderer;

    void OnEnable()
    {
        normalRenderer = target as NormalRenderer;
        normalRenderer.changeResolution = 100;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        normalRenderer.changeResolution = EditorGUILayout.IntSlider("Resolution", normalRenderer.resolution, 100, 500);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            normalRenderer.Clear();
        }

        if (GUILayout.Button("Draw"))
        {
            normalRenderer.Draw();
        }
        EditorGUILayout.EndHorizontal();
    }
}
