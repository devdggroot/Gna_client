using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(DestructibleTerrain))]
public class DestructibleTerrainInspector : Editor
{
    DestructibleTerrain destructibleTerrain;

    void OnEnable()
    {
        destructibleTerrain = target as DestructibleTerrain;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Reset Mask");
        if ( GUILayout.Button("Reset"))
        {
            destructibleTerrain.Reset();
        }
        EditorGUILayout.EndHorizontal();
    }
}
