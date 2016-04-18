using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class CharacterControllerInput : MonoBehaviour {

    Character charater;

	// Use this for initialization
	void Start () {

        charater = GetComponent<Character>();
    }
	
	// Update is called once per frame
	void Update () {

        if(charater != null)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            charater.Move(input);
        }
    }
}
