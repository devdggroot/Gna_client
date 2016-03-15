using UnityEngine;
using System.Collections;

public class DrawGizmos : MonoBehaviour {
#if UNITY_EDITOR
    private Texture2D blackBackground;

    // Use this for initialization
    void Start () {

        blackBackground = new Texture2D(1, 1);
        blackBackground.SetPixel(0, 0, new Color(0.0f, 0.0f, 0.0f, 0.8f));
        blackBackground.Apply();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
   
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.BeginGUI();
        GUI.color = Color.white;
        string text = " " + gameObject.name + " ";
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(gameObject.transform.position);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.skin.box.normal.background = blackBackground;
        Rect rect = new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y);
        GUI.Box(rect, GUIContent.none);
        GUI.Label(rect, text);
        UnityEditor.Handles.EndGUI();
    }
#endif
}
