using UnityEngine;
using System.Collections;

public class Character : Body
{
    //property
    public float moveSpeed = 1f;
    public float limitClimbAngle = 90f;

    public float minScopeAngle = -10f;
    public float maxScopeAngle = 40f;

    [HideInInspector]
    public Vector3 up;

    [HideInInspector]
    public float movement;
    [HideInInspector]
    public float angle = 0f;

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

    public void Aim(Vector3 input)
    {
        if (Mathf.Abs(input.y) > 0f)
        {
            angle = Mathf.Clamp(angle + input.y, minScopeAngle, maxScopeAngle);
        }
    }

    public void Move(Vector3 input)
    {
        if (Mathf.Abs(input.x) > 0f)
        {
            movement = input.x * moveSpeed;
        }
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (state == State.Ground)
        {
            up = hit.normal;

            if (Mathf.Abs(movement) > 0f)
            {
                Vector3 direction = UpdateLookAt(Mathf.Sign(movement) > 0f ? LookAt.Right : LookAt.Left);

                float slopeAngle = Vector3.Angle(up, Vector3.up);
                if (slopeAngle <= limitClimbAngle || Vector3.Angle(direction, up) <= 90f/*descend*/)
                {
                    Vector3 deltaMovement = direction * Mathf.Abs(movement) * Time.deltaTime;
                    deltaMovement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * deltaMovement.x;

                    Vector3 rayStart = cachedTransform.position;
                    Vector3 rayEnd = cachedTransform.position + deltaMovement;

                    Debug.DrawRay(rayStart, rayEnd - rayStart, Color.green);
                    if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
                    {
                        if (hit.distance > radius)
                        {
                            deltaMovement = direction * (hit.distance - radius);
                        }
                        else
                        {
                            deltaMovement = Vector3.zero;
                        }
                    }

                    rayStart = cachedTransform.position + deltaMovement;
                    rayEnd = rayStart + Vector3.down * (radius + radius);

                    Debug.DrawRay(rayStart, rayEnd - rayStart, Color.green);
                    if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
                    {
                        deltaMovement += Vector3.down * (hit.distance - radius);
                    }

                    cachedTransform.position = cachedTransform.position + deltaMovement;
                }
            }
        }
        else if(state == State.Airborne)
        {
            up = Vector3.up;

            if (Mathf.Abs(movement) > 0f)
            {
                UpdateLookAt(Mathf.Sign(movement) > 0f ? LookAt.Right : LookAt.Left);
            }
        }

        movement = 0f;
    }
}
