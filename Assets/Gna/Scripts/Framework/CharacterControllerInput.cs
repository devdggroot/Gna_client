using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Character))]
public class CharacterControllerInput : MonoBehaviour
{
    Character character;

    float accumulatedTime;
    bool isPressed;

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
    void Update()
    {
        if (character != null && character.state == Body.State.Ground)
        {
#if UNITY_EDITOR
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            character.Move(input);
            character.Aim(input);

            if (Input.GetButtonDown("Jump"))
            {
                isPressed = true;
            }

            if (Input.GetButtonUp("Jump"))
            {
                isPressed = false;

                float ratio = Mathf.InverseLerp(0f, 2f, accumulatedTime);
                character.Fire(Mathf.Lerp(10f, 500f, ratio)); //0f~500f

                accumulatedTime = 0f;
            }

            if (isPressed)
            {
                accumulatedTime += Time.deltaTime;
                if (accumulatedTime > 2f)
                    accumulatedTime = 2f;
            }
#endif
        }
    }
}
