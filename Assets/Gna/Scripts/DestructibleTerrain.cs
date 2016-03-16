using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DestructibleTerrain : MonoBehaviour
{
    public Texture2D raw;
    SpriteRenderer spriteRenderer;

    Texture2D mask;
    Color[] pixels; //collision info

    void OnEnable()
    {
        
    }

    void OnDisable()
    {

    }

    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (raw != null) //sprite
        {
            Sprite sprite = Sprite.Create(raw, new Rect(0f, 0f, raw.width, raw.height), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;

            mask = CreateMask(raw);
            pixels = mask.GetPixels();

            spriteRenderer.material.SetTexture("_MaskTex", mask); //instance material
        }
    }

    void OnDestroy()
    {
        raw = null;

        pixels = null;
        if (mask != null)
            Destroy(mask);
        mask = null;

        spriteRenderer.sprite = null;
        spriteRenderer.material = null; //instance material
    }

    // Update is called once per frame
    void Update()
    {

    }

    Texture2D CreateMask(Texture2D raw)
    {
        if (raw != null)
        {
            Color[] cols = raw.GetPixels();

            Texture2D newTex = new Texture2D(raw.width, raw.height, TextureFormat.RGBA32, false);
            Color[] newCols = new Color[raw.width * raw.height];

            for (int y = 0, ymax = raw.height; y < ymax; ++y)
            {
                for (int x = 0, xmax = raw.width; x < xmax; ++x)
                {
                    int idx = x + y * xmax;
                    if (cols[idx].a > 0f)
                        newCols[idx] = new Color(0f, 1f, 0f, 0f);

                    else
                        newCols[idx] = Color.clear;
                }
            }

            newTex.SetPixels(newCols);
            newTex.Apply();

            newCols = null;
            return newTex;
        }

        return null;
    }

    public void Destroyed(Vector3 worldPoint, float radius, int resolution)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        Bounds bounds = spriteRenderer.sprite.bounds;
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        int xPos = (int)((localPoint.x + bounds.extents.x) * pixelsPerUnit);
        int yPos = (int)((localPoint.y + bounds.extents.y) * pixelsPerUnit);
        
        radius = radius * pixelsPerUnit;
        float radiusSq = radius * radius;
        
        for (int x = Mathf.Clamp(xPos - (int)radius, 0, mask.width), xmax = Mathf.Clamp(xPos + (int)radius, 0, mask.width); x < xmax; x += resolution)
        {
            for (int y = Mathf.Clamp(yPos - (int)radius, 0, mask.height), ymax = Mathf.Clamp(yPos + (int)radius, 0, mask.height); y < ymax; y += resolution)
            {
                float xDiff = x - xPos;
                float yDiff = y - yPos;
                float diffSq = xDiff * xDiff + yDiff * yDiff;

                if (diffSq < radiusSq)
                {
                    //float a = Mathf.Cos(Mathf.Lerp(0f, Mathf.PI * 0.5f, diffSq / radiusSq));

                    for (int i = 0; i < resolution; ++i)
                    {
                        for (int j = 0; j < resolution; ++j)
                        {
                            int idx = (x + i) + (y + j) * mask.width;
                            Color col = pixels[idx];

                            col.g = 0f;
                            col.a = 1f;//a;

                            pixels[idx] = col;
                        }
                    }
                }
            }
        }

        mask.SetPixels(pixels);
        mask.Apply();
    }
}
