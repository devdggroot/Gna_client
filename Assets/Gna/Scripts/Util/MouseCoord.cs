using UnityEngine;
using System.Collections;

using System.Text;

[RequireComponent(typeof(GUIText))]
public class MouseCoord : MonoBehaviour {

    GUIText gui;
    StringBuilder contents;

    public PixelCollider terrain; 

	// Use this for initialization
	void Start () {

        gui = GetComponent<GUIText>();
        contents = new StringBuilder();
    }
	
    void OnDestroy()
    {
        contents = null;
        gui = null;
    }

	// Update is called once per frame
	void Update () {

        contents.Remove(0, contents.Length);

        Vector3 pos = Input.mousePosition;
        contents.AppendLine("mouse position: " + pos.x + ", " + pos.y);

        if (terrain != null)
        {
            pos = Camera.main.ScreenToWorldPoint(pos);

            Vector3 local = terrain.cachedTransform.InverseTransformPoint(pos);

            Bounds bounds = terrain.bounds;
            float pixelsPerUnit = terrain.pixelsPerUnit;

            int xPos = (int)((local.x + bounds.extents.x) * pixelsPerUnit);
            int yPos = (int)((local.y + bounds.extents.y) * pixelsPerUnit);

            contents.AppendLine(terrain.name + "(linked terrain) pixel coord : " + xPos + ", " + yPos);
        }

        gui.text = contents.ToString();
    }
}
