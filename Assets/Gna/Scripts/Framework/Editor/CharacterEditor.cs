using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(Character), true)]
public class CharacterEditor : BodyEditor
{
    Character character;

    void OnEnable()
    {
        character = target as Character;
    }

    void DrawInspector()
    {
        EditorGUILayout.BeginVertical();

        character.moveSpeed = EditorGUILayout.Slider("Move Speed", character.moveSpeed, 1f, 10f);
        character.limitClimbAngle = EditorGUILayout.Slider("Limit ClimbAngle", character.limitClimbAngle, 60f, 90f);
        character.minScopeAngle = EditorGUILayout.Slider("Min ScopeAngle", character.minScopeAngle, -90, 0f);
        character.maxScopeAngle = EditorGUILayout.Slider("Max ScopeAngle", character.maxScopeAngle, 0, 90f);

        EditorGUILayout.Separator();

        GUI.enabled = false;
        EditorGUILayout.FloatField("Current ClimbAngle", Vector3.Angle(character.up, Vector3.up));
        EditorGUILayout.FloatField("Movement", character.movement);
        EditorGUILayout.FloatField("Current ScopeAngle", character.angle);
        GUI.enabled = true;

        EditorGUILayout.EndVertical();
    }

    public override void OnInspectorGUI()
    {
        if (character == null)
            character = target as Character;

        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        DrawInspector();
    }
}
