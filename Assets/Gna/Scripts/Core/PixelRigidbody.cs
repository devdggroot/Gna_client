using UnityEngine;
using System.Collections;

public class PixelRigidbody : CachedTransform
{
    //property
    [HideInInspector] public float COR = 0f; //coefficient of restitution
    [HideInInspector] public float GravityScale = 0.02f;

    //
    public PixelCollider pixelCollider;

    public Vector3 acceleration { get; protected set; }
    public Vector3 velocity { get; protected set; }

    public float radius { get; protected set; }
    public float MinimumVelocity { get; protected set; }

    gna.Physics.RaycastHit hit;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        acceleration = new Vector3(0f, gna.Physics.gravity * GravityScale, 0f);
        velocity = Vector3.zero;

        pixelCollider.Setup();
        radius = Mathf.Min(pixelCollider.width, pixelCollider.height) * 0.5f / pixelCollider.pixelsPerUnit;

        MinimumVelocity = gna.Physics.gravity * GravityScale * Time.fixedDeltaTime;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector3 v = velocity + acceleration * Time.deltaTime;

        Vector3 end = cachedTransform.position + v; //position in world
        if (pixelCollider != null)
        {
            radius = Mathf.Min(pixelCollider.width, pixelCollider.height) * 0.5f / pixelCollider.pixelsPerUnit;
            end += (v.normalized * radius);
        }

        //ray to ground
        if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(cachedTransform.position, end), ref hit))
        {
            if(COR > 0f)
            {
                Vector3 normal = hit.normal;
                if (hit.normal.sqrMagnitude <= float.Epsilon)
                {
                    normal = Vector3.up;
                }

                Vector3 reflection = v - normal * Vector3.Dot(v, normal) * 2;
                velocity = reflection * COR;

                if (velocity.sqrMagnitude <= (MinimumVelocity * MinimumVelocity))
                {
                    velocity = Vector3.zero;
                }
            }
            else
            {
                velocity = Vector3.zero;
            }
            cachedTransform.position = hit.point + (-v.normalized * radius);
        }
        else
        {
            cachedTransform.position = cachedTransform.position + v;
            velocity = v;
        }

        //Time.deltaTime == Time.fixedDeltaTime
        //Time.fixedTime : total fiexdTime
    }
}