using UnityEngine;
using System.Collections;

public class Character : Body
{
    //property
    [HideInInspector]
    public float moveSpeed = 1f;
    [HideInInspector]
    public float limitClimbAngle = 60f;

    [HideInInspector]
    public float minScopeAngle = -10f;
    [HideInInspector]
    public float maxScopeAngle = 45f;

    //variable
    [HideInInspector]
    public Vector3 up;
    [HideInInspector]
    public float movement;
    [HideInInspector]
    public float angle = 0f;

    public float centerOffset = 1f;
    public float sensitivity = 10f;

    public enum LookAt
    {
        Right,
        Left,
    }

    Transform _centeredTransform;
    public Transform centeredTransform
    {
        get
        {
            if (_centeredTransform == null)
                _centeredTransform = gna.Factory.Search("Center", cachedTransform);

            return _centeredTransform;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        _centeredTransform = gna.Factory.Search("Center", cachedTransform);
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
            projectile.cachedTransform.position = _centeredTransform.position;

            Vector3 direction = Quaternion.AngleAxis(angle, cachedTransform.forward) * _centeredTransform.right;

            Vector3 acceleration = direction * (force / projectile.mass);
            Vector3 deltaVelocity = acceleration * Time.fixedDeltaTime;

            projectile.Fire(deltaVelocity);
        }
    }

    public void Aim(Vector3 input)
    {
        if (Mathf.Abs(input.y) > 0f)
        {
            angle = Mathf.Clamp(angle + (sensitivity > 0f ? input.y / sensitivity : input.y), minScopeAngle, maxScopeAngle);
        }
    }

    public void Move(Vector3 input)
    {
        if (Mathf.Abs(input.x) > 0f)
        {
            movement = input.x * moveSpeed;
        }
    }

    Vector3 UpdateCenter(Vector3 normal)
    {
        _centeredTransform.rotation = Quaternion.LookRotation(cachedTransform.forward, normal);
        _centeredTransform.position = cachedTransform.position + _centeredTransform.up * centerOffset;

        return _centeredTransform.right;
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
            }/*exception(if one line pixel, normal is incorrect.)*/

            up = normal;
            UpdateCenter(up);

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
            UpdateCenter(up);

            if (Mathf.Abs(movement) > 0f)
            {
                UpdateLookAt(Mathf.Sign(movement) > 0f ? LookAt.Right : LookAt.Left);
            }
        }

        movement = 0f;
    }
}
