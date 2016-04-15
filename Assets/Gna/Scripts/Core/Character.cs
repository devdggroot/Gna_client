using UnityEngine;
using System.Collections;

public class Character : PixelRigidbody
{
    // Use this for initialization
    /*void Start () {
	
	}*/

    // Update is called once per frame
    /*void Update () {
	
	}*/

    float skin = 0.1f;
    float moveSpeed = 2f;

    public bool test = false;

    protected override void FixedUpdate()
    {
        radius = 1f;
        acceleration.y = (gna.Physics.gravity * GravityScale); //apply gravity

        Vector3 deltaVelocity = acceleration * Time.deltaTime;
        velocity += deltaVelocity;

        //input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Mathf.Abs(input.x) > 0f)
        {
            velocity.x = input.x * moveSpeed;//Mathf.SmoothDamp(velocity.x, input.x * moveSpeed, ref velocityXSmoothing, 0.1f);
        }
        //

        Vector3 deltaMovement = velocity * Time.deltaTime;
        Vector3 movement = Vector3.zero;

        if (Mathf.Abs(deltaMovement.x) > 0f)
        {
            Vector3 direction = cachedTransform.right * Mathf.Sign(deltaMovement.x);
            movement = direction * Mathf.Abs(deltaMovement.x);

            Vector3 rayStart = cachedTransform.position /*+ direction * radius * 0.5f*/;
            Vector3 rayEnd = rayStart + (direction * skin) + movement;

            Debug.DrawRay(rayStart, rayEnd - rayStart, Color.blue);
            if (/*direction.y >= 0f &&*/ TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
            {
                if( hit.distance > skin)
                {
                    movement = direction * (hit.distance - skin);
                }
                else
                {
                    movement = Vector3.zero;
                }
            }
        }

        if (Mathf.Abs(deltaMovement.y) > 0f)
        {
            Vector3 direction = cachedTransform.up * -1f;

            Vector3 rayStart = cachedTransform.position + movement + direction * radius;
            Vector3 rayEnd = rayStart + (skin + Mathf.Abs(deltaMovement.y)) * direction;

            //
            if (Mathf.Abs(movement.x) > 0f)
            {
                rayEnd += (skin/*minimum instant movement*/ * direction);
            }
                

            Debug.DrawRay(rayStart, rayEnd - rayStart, Color.blue);
            if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
            {
                movement += direction * (radius + hit.distance);
                movement += hit.normal * (radius + skin);

                velocity.y = 0f;
                cachedTransform.up = hit.normal;
            }

            else
            {
                movement += direction * (radius + skin + Mathf.Abs(deltaMovement.y));
                movement += Vector3.up * (radius + skin);

                cachedTransform.up = Vector3.up;
            }
        }

        velocity.x = 0f;
        cachedTransform.position = cachedTransform.position + movement;
    }
}
