using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class CharacterControllerInput : MonoBehaviour
{
    Character character;

    void OnDestroy()
    {
        character = null;
    }

    // Use this for initialization
    void Start()
    {
        character = GetComponent<Character>();
    }

    // Update is called once per frame
    /*void Update()
    {
        if (character != null)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            character.Move(input);
            character.Aim(input);
        }
    }*/

    void FixedUpdate()
    {
        if (character != null)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            character.Move(input);
            character.Aim(input);
        }
    }
}
