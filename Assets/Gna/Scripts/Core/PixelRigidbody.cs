using UnityEngine;
using System.Collections;

public class PixelRigidbody : CachedTransform
{
    //property
    [HideInInspector] public float COR = 0f; //coefficient of restitution
    [HideInInspector] public float GravityScale = 2;
    [HideInInspector] public float LimitClimbAngle = 90;

    //
    public PixelCollider pixelCollider;

    public Vector3 acceleration { get; protected set; }
    public Vector3 velocity { get; protected set; }

    public float radius { get; protected set; }
    public float minimumVelocity { get; protected set; }

    gna.Physics.RaycastHit hit;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        acceleration = Vector3.zero;
        velocity = Vector3.zero;
        minimumVelocity = gna.Physics.gravity * GravityScale * Time.fixedDeltaTime;

        if(pixelCollider != null)
        {
            pixelCollider.Setup();
            radius = Mathf.Min(pixelCollider.width, pixelCollider.height) * 0.5f / pixelCollider.pixelsPerUnit;
        }
        else
        {
            print("pixelCollider is null(" + gameObject.name + ").");
        }
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
        Vector3 a = acceleration;
        a.y += (gna.Physics.gravity * GravityScale); //apply gravity

        Vector3 deltaVelocity = a * Time.deltaTime;
        velocity = velocity + deltaVelocity;

        Vector3 deltaMovement = velocity * Time.deltaTime;

        Vector3 rayStart = Vector3.zero;
        Vector3 rayEnd = Vector3.zero;

        //raycast y-axis
        //Vector3 end = cachedTransform.position + deltaMovement;
        //end += (deltaMovement.normalized * radius); //

        rayStart = cachedTransform.position;
        rayEnd = cachedTransform.position + deltaMovement + (deltaMovement.normalized * radius);

        Debug.DrawRay(rayStart, rayEnd - rayStart, Color.red);
        if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
        {
            if(COR > 0f)
            {
                Vector3 normal = hit.normal;
                if (hit.normal.sqrMagnitude <= float.Epsilon)
                {
                    normal = Vector3.up;
                }

                Vector3 reflection = velocity - normal * Vector3.Dot(velocity, normal) * 2f;
                velocity = reflection * COR;

                if (velocity.sqrMagnitude <= (minimumVelocity * minimumVelocity))
                {
                    velocity = Vector3.zero;
                }
            }
            else
            {
                velocity = Vector3.zero;
            }

            //
            deltaMovement = deltaMovement.normalized * (hit.distance - radius);
            cachedTransform.position = cachedTransform.position + deltaMovement;
        }
        else
        {
            cachedTransform.position = cachedTransform.position + deltaMovement;
        }



        //print("PixelRigidbody::FixedUpdate");
        //Time.deltaTime == Time.fixedDeltaTime
        //Time.fixedTime : total fiexdTime
    }
}