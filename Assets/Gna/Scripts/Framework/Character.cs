using UnityEngine;
using System.Collections;

public class Character : Body
{
    //property
    public float moveSpeed = 1f;
    public float limitClimbAngle = 90f;

    public float minScopeAngle = -10f;
    public float maxScopeAngle = 40f;

    //variable
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

    public void Fire(float force)
    {
        GameObject go = Resources.Load("Prefabs/Projectile") as GameObject;
        if (go != null)
        {
            go = GameObject.Instantiate(go) as GameObject;
            gna.Factory.AddChild(go.transform, ProjectileRoot.instance.transform);

            Projectile projectile = go.GetComponent<Projectile>();
            projectile.cachedTransform.position = cachedTransform.position;

            Vector3 lookAt = Quaternion.LookRotation(cachedTransform.forward, up) * Vector3.right;
            Vector3 direction = Quaternion.AngleAxis(angle, cachedTransform.forward) * lookAt;

            Vector3 acceleration = direction * (force / projectile.mass);
            Vector3 deltaVelocity = acceleration * Time.fixedDeltaTime;

            projectile.Fire(deltaVelocity);
        }
    }

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
            Vector3 normal = hit.normal;
            if (normal.y < 0f)
            {
                normal.y *= -1f;
            }
            else if (normal.y <= float.Epsilon)
            {
                normal = Vector3.up;
            }/*exception*/

            up = normal;

            if (Mathf.Abs(movement) > 0f)
            {
                Vector3 lookAt = UpdateLookAt(Mathf.Sign(movement) > 0f ? LookAt.Right : LookAt.Left);

                float slopeAngle = Vector3.Angle(up, Vector3.up);
                if (slopeAngle <= limitClimbAngle || Vector3.Angle(lookAt, up) <= 90f/*descend*/)
                {
                    Vector3 deltaMovement = lookAt * Mathf.Abs(movement) * Time.deltaTime;
                    deltaMovement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * deltaMovement.x;

                    Vector3 rayStart = cachedTransform.position;
                    Vector3 rayEnd = cachedTransform.position + deltaMovement;

                    Debug.DrawRay(rayStart, rayEnd - rayStart, Color.green);
                    if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
                    {
                        float distance = (float)System.Math.Round(hit.distance, 3, System.MidpointRounding.AwayFromZero);
                        if (distance > radius)
                        {
                            deltaMovement = lookAt * (distance - radius);
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
                        float distance = (float)System.Math.Round(hit.distance, 3, System.MidpointRounding.AwayFromZero);
                        deltaMovement += Vector3.down * (distance - radius);

                        //check 
                        rayStart = cachedTransform.position + deltaMovement;
                        rayEnd = rayStart + Vector3.down * radius;

                        Debug.DrawRay(rayStart, rayEnd - rayStart, Color.blue);
                        if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
                        {
                            distance = (float)System.Math.Round(hit.distance, 3, System.MidpointRounding.AwayFromZero);
                            if (distance < radius)
                            {
                                deltaMovement = Vector3.zero;
                            }
                        }
                    }

                    cachedTransform.position = cachedTransform.position + deltaMovement;
                }
            }
        }
        else if (state == State.Airborne)
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
