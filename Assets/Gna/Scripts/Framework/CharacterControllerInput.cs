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
        if (character != null)
        {
#if UNITY_EDITOR
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            character.Move(input);
            character.Aim(input);

            if (character.state == Body.State.Ground)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    isPressed = true;
                }

                if (Input.GetButtonUp("Jump") && isPressed)
                {
                    isPressed = false;

                    float ratio = Mathf.InverseLerp(0f, 1f, accumulatedTime);
                    character.Fire(Mathf.Lerp(1f, 1000f, ratio));

                    accumulatedTime = 0f;
                }

                if (isPressed)
                {
                    accumulatedTime += Time.deltaTime;
                    if (accumulatedTime > 1f)
                    {
                        isPressed = false;

                        float ratio = Mathf.InverseLerp(0f, 1f, accumulatedTime);
                        character.Fire(Mathf.Lerp(10f, 1000f, ratio));

                        accumulatedTime = 0f;
                    }
                }
            }
#endif
        }
    }
}
