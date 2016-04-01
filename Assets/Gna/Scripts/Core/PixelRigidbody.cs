using UnityEngine;
using System.Collections;

public class PixelRigidbody : CachedTransform
{
    public const float gravity = -9.8f;
    public const float gravityScale = 0.04f;

    //property
    [HideInInspector] public float COR; //coefficient of restitution

    //
    public PixelCollider pixelCollider;

    public Vector3 acceleration { get; protected set; }
    public Vector3 velocity { get; protected set; }

    public float radius { get; protected set; }

    gna.Physics.RaycastHit hit;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        acceleration = new Vector3(0f, gravity * gravityScale, 0f);
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
                if (normal.sqrMagnitude <= float.Epsilon)
                {
                    normal = -v.normalized;
                    Debug.Log(hit.normal);
                }

                Vector3 reflection = v - normal * Vector3.Dot(v, normal) * 2;

                cachedTransform.position = hit.point + (reflection.normalized * radius);
                velocity = reflection * COR;
            }

            else
            {
                cachedTransform.position = hit.point + (-v.normalized * radius);
                velocity = Vector3.zero;
            }
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