using UnityEngine;
using System.Collections;

public class RigidbodyPixel : CachedTransform {

    const float gravity = -9.8f;

    public float gravityScale = 0.04f;
    public PixelCollider pixelCollider;
    
    public Vector3 gravityAcceleration { get; protected set; }
    public Vector3 velocity { get; protected set; }

    PixelCollider.RaycastHit hit;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        gravityAcceleration = new Vector3(0f, gravity * gravityScale, 0f);
        velocity = Vector3.zero;
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
        Vector3 v = velocity + gravityAcceleration * Time.deltaTime;
        Vector3 p = cachedTransform.position + v;

        //ray to ground
        if (TerrainRoot.instance.Raycast(cachedTransform.position, p, ref hit, pixelCollider))
        {
            p.y = hit.coordToWorld.y;

            cachedTransform.position = p;
            velocity = Vector3.zero;
        }
        else
        {
            cachedTransform.position = p;
            velocity = v;
        }

        //Time.deltaTime == Time.fixedDeltaTime
        //Time.fixedTime : total fiexdTime
    }
}
