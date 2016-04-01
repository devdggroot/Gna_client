using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelCollider : CachedTransform
{
    protected SpriteRenderer spriteRenderer;

    public int width { get; private set; }
    public int height { get; private set; }

    public Vector2 pivot { get; private set; }
    public float pixelsPerUnit { get; private set; }

    public Color[] pixels { get; protected set; }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer.sprite != null)
        {
            width = spriteRenderer.sprite.texture.width;
            height = spriteRenderer.sprite.texture.height;

            pivot = spriteRenderer.sprite.pivot;
            pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

            pixels = spriteRenderer.sprite.texture.GetPixels();
        }
    }

    protected override void OnDestroy()
    {
        pixels = null;
        spriteRenderer = null;

        Debug.Log("PixelCollider::OnDestroy");
        base.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 NormalAt(int x, int y)
    {
        Vector2 avg = Vector2.zero;
        for (int w = -3; w <= 3; ++w)
        {
            if ((x + w) >= 0f && (x + w) < width)
            {
                for (int h = -3; h <= 3; ++h)
                {
                    if ((y + h) >= 0f && (y + h) < height)
                    {
                        int idx = (x + w) + (y + h) * width;
                        if (idx < pixels.Length && pixels[idx].a > 0f)
                        {
                            avg.x -= w;
                            avg.y -= h;
                        }
                    }
                }
            }
        }

        return avg.normalized;
    }
}
