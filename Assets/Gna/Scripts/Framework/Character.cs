using UnityEngine;
using System.Collections;

public class Character : Body
{
    public float skin = 0.1f;
    public float moveSpeed = 2f;
    public float limitClimbAngle = 90f;

    public enum View
    {
        Right,
        Left,
    }
    public View view { get; protected set; }

    // Use this for initialization
    protected override void Start () {

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

    View UpdateMovement(Vector3 deltaMovement, ref Vector3 movement)
    {
        Vector3 direction = cachedTransform.right * Mathf.Sign(deltaMovement.x);

        float slopeAngle = Vector3.Angle(cachedTransform.up, Vector3.up);
        if (slopeAngle > limitClimbAngle && direction.y >= 0f/*climb*/)
        {
            deltaMovement.x = 0f;
        }

        if (slopeAngle >= 90f && direction.y < 0f/*descend*/)
        {
            cachedTransform.up = Vector3.up;
            direction = cachedTransform.right * Mathf.Sign(deltaMovement.x);
        }

        movement = direction * Mathf.Abs(deltaMovement.x);

        Vector3 rayStart = cachedTransform.position /*+ direction * radius * 0.5f*/;
        Vector3 rayEnd = rayStart + (direction * skin) + movement;

        Debug.DrawRay(rayStart, rayEnd - rayStart, Color.blue);
        if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
        {
            if (hit.distance > skin)
            {
                movement = direction * (hit.distance - skin);
            }
            else
            {
                movement = Vector3.zero;
            }
        }

        return Mathf.Sign(deltaMovement.x) > 0f ? View.Right : View.Left;
    }

    Body.State UpdateBodyState(Vector3 deltaMovement, ref Vector3 movement)
    {
        Vector3 direction = cachedTransform.up * Mathf.Sign(deltaMovement.y);

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

            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle >= 90f)
            {
                cachedTransform.up = Vector3.up;
                return Body.State.Airborne;
            }
            else
            {
                cachedTransform.up = hit.normal;
                return Body.State.Ground;
            }
        }

        else
        {
            if( movement.y >= 0f/*climb*/)
            {
                movement += direction * (radius + skin);
                movement += Vector3.up * (radius + skin);
            }

            cachedTransform.up = Vector3.up;
            movement += cachedTransform.up * Mathf.Sign(deltaMovement.y) * Mathf.Abs(deltaMovement.y);

            return Body.State.Airborne;
        }
    }

    protected override void FixedUpdate()
    {
        acceleration.y = (gna.Physics.gravity * GravityScale); //apply gravity

        Vector3 deltaVelocity = acceleration * Time.deltaTime;
        velocity += deltaVelocity;

        Vector3 deltaMovement = velocity * Time.deltaTime;

        Vector3 movement = Vector3.zero;
        if (Mathf.Abs(deltaMovement.x) > 0f && state == State.Ground)
        {
            view = UpdateMovement(deltaMovement, ref movement);
        }

        if (Mathf.Abs(deltaMovement.y) > 0f)
        {
            state = UpdateBodyState(deltaMovement, ref movement);
        }
            
        if( state == Body.State.Ground)
        {
            velocity.y = 0f;
        }
        velocity.x = 0f;
        cachedTransform.position = cachedTransform.position + movement;
    }
}
