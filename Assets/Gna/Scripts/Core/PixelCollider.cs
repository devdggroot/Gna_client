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
        
        Setup();
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

    public void Setup()
    {
        if(spriteRenderer == null)
        {
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
    }

    public Vector2 NormalAt(int x, int y)
    {
        Vector2 avg = Vector2.zero;

        for (int w = -3; w <= 3; ++w)
        {
            for (int h = -3; h <= 3; ++h)
            {
                //width, height 예외처리.

                int idx = (x + w) + (y + h) * width;
                if (idx < pixels.Length && pixels[idx].a > 0f)
                {
                    avg.x -= w;
                    avg.y -= h;
                }


                //
                /*for ( int i = 0; i < resolution; ++i)
                {
                    int _x = x + resolution * w + i;
                    if (_x < 0 || _x >= width) continue;

                    for ( int j = 0; j < resolution; ++j)
                    {
                        int _y = y + resolution * h + j;
                        if (_y < 0 || _y >= height) continue;

                        int idx = (_x + w) + (_y + h) * width;
                        if (idx < pixels.Length && pixels[idx].a > 0f)
                        {
                            avg.x -= w;
                            avg.y -= h;

                            //
                            i = resolution;
                            j = resolution;
                            //break;
                        }
                    }
                }*/
                //
                
            }
        }

        avg = avg.normalized;
        if (Mathf.Abs(avg.x) <= float.Epsilon && Mathf.Abs(avg.y) <= float.Epsilon)
        {
            avg = Vector3.up;
        }

        return avg;
    }
}
