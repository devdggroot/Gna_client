using UnityEngine;
using System.Collections;

public class Character : Body
{
    public float moveSpeed = 1f;
    public float limitClimbAngle = 90f;

    public enum LookAt
    {
        Right,
        Left,
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    /*void Update () {
	
	}*/

    public bool Move(Vector3 input)
    {
        if (Mathf.Abs(input.x) > 0f)
        {
            velocity.x = input.x * moveSpeed;
            return true;
        }

        return false;
    }

    Vector3 UpdateLookAt(Vector3 up)
    {
        cachedTransform.rotation = Quaternion.LookRotation(cachedTransform.forward, up);
        return cachedTransform.right;
    }

    Vector3 UpdateLookAt(LookAt lookAt)
    {
        cachedTransform.rotation = Quaternion.LookRotation(lookAt == LookAt.Right ? Vector3.forward : Vector3.back, cachedTransform.up);
        return cachedTransform.right;
    }

    Body.State UpdateMovement(Vector3 deltaMovement, ref Vector3 movement)
    {
        Vector3 direction = cachedTransform.right;

        if (Mathf.Abs(deltaMovement.x) > 0f && state == State.Ground)
        {
            direction = UpdateLookAt(Mathf.Sign(deltaMovement.x) > 0f ? LookAt.Right : LookAt.Left);

            float slopeAngle = Vector3.Angle(cachedTransform.up, Vector3.up);
            if (slopeAngle <= limitClimbAngle || direction.y < 0f/*descend*/)
            {
                movement = direction * Mathf.Abs(deltaMovement.x);

                Vector3 rayStart = cachedTransform.position;
                Vector3 rayEnd = rayStart + direction * radius + movement;

                Debug.DrawRay(rayStart, rayEnd - rayStart, Color.red);
                if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
                {
                    if (hit.distance > radius)
                    {
                        movement = direction * (hit.distance - radius);
                    }
                    else
                    {
                        movement = Vector3.zero;
                    }
                }
            }
            else
            {
                movement = Vector3.zero;
            }
        }

        if (Mathf.Abs(deltaMovement.y) > 0f)
        {
            Vector3 down = cachedTransform.up * -1f;

            Vector3 rayStart = cachedTransform.position + movement;
            Vector3 rayEnd = rayStart + down * (radius + Mathf.Abs(movement.sqrMagnitude > 0f ? deltaMovement.x : deltaMovement.y));

            Debug.DrawRay(rayStart, rayEnd - rayStart, Color.green);
            if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
            {
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle < 90f)
                {
                    movement += down * hit.distance;
                    movement += hit.normal * radius;

                    UpdateLookAt(hit.normal);
                    return Body.State.Ground;
                }
            }

            movement = down * radius;
            movement += Vector3.up * radius;

            direction = UpdateLookAt(Vector3.up);
            down = cachedTransform.up * -1f;

            if (movement.sqrMagnitude > 0f)
            {
                movement += direction * Mathf.Abs(deltaMovement.x);
            }
            movement += down * Mathf.Abs(deltaMovement.y);

            return Body.State.Airborne;
        }

        return state;
    }

    protected override void FixedUpdate()
    {
        acceleration.y = (gna.Physics.gravity * GravityScale); //apply gravity

        Vector3 deltaVelocity = acceleration * Time.deltaTime;
        velocity += deltaVelocity;

        Vector3 deltaMovement = velocity * Time.deltaTime;
        Vector3 movement = Vector3.zero;

        state = UpdateMovement(deltaMovement, ref movement);
        cachedTransform.position = cachedTransform.position + movement;

        velocity.x = 0f;
        if (state == Body.State.Ground)
        {
            velocity = Vector3.zero;
        }
    }
}
