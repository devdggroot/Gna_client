using UnityEngine;
using System.Collections;

using System.Text;

[RequireComponent(typeof(GUIText))]
public class MouseCoord : MonoBehaviour {

    GUIText gui;
    StringBuilder contents;

    public PixelCollider forPixelCoord; 

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

        if (forPixelCoord != null)
        {
            pos = Camera.main.ScreenToWorldPoint(pos);

            Vector3 local = forPixelCoord.cachedTransform.InverseTransformPoint(pos);

            int xPos = (int)(local.x * forPixelCoord.pixelsPerUnit + forPixelCoord.pivot.x);
            int yPos = (int)(local.y * forPixelCoord.pixelsPerUnit + forPixelCoord.pivot.y);

            contents.AppendLine(forPixelCoord.name + "(linked terrain) pixel coord : " + xPos + ", " + yPos);
        }

        gui.text = contents.ToString();
    }
}
