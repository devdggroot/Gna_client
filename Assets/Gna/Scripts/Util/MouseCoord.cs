using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class MouseCoord : MonoBehaviour {

    GUIText gui;

	// Use this for initialization
	void Start () {

        gui = GetComponent<GUIText>();
    }
	
	// Update is called once per frame
	void Update () {
	
        Vector3 pos = Input.mousePosition;
        gui.text = "mouse position: " + pos.x + ", " + pos.y;
	}
}
