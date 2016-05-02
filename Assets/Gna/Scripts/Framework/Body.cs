using UnityEngine;
using System.Collections;

public class Body : CachedTransform
{
    //property
    [HideInInspector]
    public float COR = 0f; //coefficient of restitution
    [HideInInspector]
    public float GravityScale = 1f;

    public float mass = 1f;
    public float radius = 1f;

    //variable
    [HideInInspector]
    public Vector3 acceleration;
    [HideInInspector]
    public Vector3 velocity;

    public enum State
    {
        Ground,
        Airborne,
    }
    public State state { get; protected set; }

    protected gna.Physics.RaycastHit hit;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        acceleration.y = (gna.Physics.gravity * GravityScale); //apply gravity

        Vector3 deltaVelocity = acceleration * Time.deltaTime;
        velocity += deltaVelocity;

        Vector3 deltaMovement = velocity * Time.deltaTime;

        Vector3 rayStart = cachedTransform.position;
        Vector3 rayEnd = cachedTransform.position + (deltaMovement.normalized * radius) + deltaMovement;

        Debug.DrawRay(rayStart, rayEnd - rayStart, Color.red);
        if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
        {
            Vector3 normal = hit.normal;
            if (hit.normal.sqrMagnitude <= float.Epsilon)
            {
                normal = Vector3.up;
            }

            if (COR > 0f)
            {
                Vector3 reflection = velocity - normal * Vector3.Dot(velocity, normal) * 2f;
                velocity = reflection * COR;

                if (velocity.sqrMagnitude <= gna.Physics.velocityEpsilon)
                {
                    velocity = Vector3.zero;
                    state = State.Ground;
                }
            }
            else
            {
                velocity = Vector3.zero;
                state = State.Ground;
            }

            //
            deltaMovement = deltaMovement.normalized * (hit.distance - radius);
            cachedTransform.position = cachedTransform.position + deltaMovement;
        }
        else
        {
            state = State.Airborne;
            cachedTransform.position = cachedTransform.position + deltaMovement;
        }

        //print("Body::FixedUpdate");
        //Time.deltaTime == Time.fixedDeltaTime
        //Time.fixedTime : total fiexdTime
    }
}