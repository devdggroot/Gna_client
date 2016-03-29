using UnityEngine;
using System.Collections;

public class RigidbodyPixel : CachedTransform {

    public PixelCollider pixelCollider;

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
}
