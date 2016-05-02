using UnityEngine;
using System.Collections;

public class Projectile : Body
{
    //property
    public float damage = 1f;
    public float explosion = 1f;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    /*void Update () {
	
	}*/

    public void Fire(Vector3 deltaVelocity)
    {
        velocity += deltaVelocity;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (state == State.Ground)
        {
            TerrainRoot.instance.Destoryed(hit.point, explosion, 1);
            DestroyImmediate(gameObject);
        }
        else if (state == State.Airborne)
        {
            cachedTransform.right = velocity.normalized;
        }
    }
}
