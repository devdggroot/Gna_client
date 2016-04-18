using UnityEngine;
using System.Collections;

public class DestructibleTerrain : CollisionTerrain
{
    protected Texture2D mask { get; private set; }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {

    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        AssignAlphaMask();
    }

    protected override void OnDestroy()
    {
        Destroy(spriteRenderer.material); //instance material
        pixels = null;

        if (mask != null)
            Destroy(mask);

        mask = null;

        Debug.Log("DestructibleTerrain::OnDestroy");
        base.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool AssignAlphaMask()
    {
        if (spriteRenderer != null)
        {
            Texture2D raw = spriteRenderer.sprite.texture;
            if( raw != null)
            {
                Color[] cols = raw.GetPixels();

                mask = new Texture2D(raw.width, raw.height, TextureFormat.Alpha8, false);
                pixels = new Color[raw.width * raw.height];

                for (int y = 0, ymax = raw.height; y < ymax; ++y)
                {
                    for (int x = 0, xmax = raw.width; x < xmax; ++x)
                    {
                        int idx = x + y * xmax;
                        if (cols[idx].a > 0f)
                            pixels[idx] = new Color(0f, 0f, 0f, 1f);

                        else
                            pixels[idx] = Color.clear;
                    }
                }

                mask.SetPixels(pixels);
                mask.Apply();

                spriteRenderer.material.SetTexture("_AlphaTex", mask); //instance material
                return true;
            }
        }

        return false;
    }

    public void Reset()
    {
        if (mask != null)
            Destroy(mask);

        AssignAlphaMask();
    }

    public override void Destroyed(int xPos, int yPos, float radius, int resolution)
    {
        float radiusSq = radius * radius;

        for (int y = Mathf.Clamp(yPos - (int)radius, 0, mask.height), ymax = Mathf.Clamp(yPos + (int)radius, 0, mask.height); y < ymax; y += resolution)
        {
            for (int x = Mathf.Clamp(xPos - (int)radius, 0, mask.width), xmax = Mathf.Clamp(xPos + (int)radius, 0, mask.width); x < xmax; x += resolution)
            {
                float xDiff = x - xPos;
                float yDiff = y - yPos;
                float diffSq = xDiff * xDiff + yDiff * yDiff;

                if (diffSq <= radiusSq)
                {
                    float alpha = 0f;//Mathf.Sin(Mathf.Lerp(0f, Mathf.PI * 0.5f, diffSq / radiusSq) * 0.04f);

                    for (int i = 0; i < resolution; ++i)
                    {
                        for (int j = 0; j < resolution; ++j)
                        {
                            int idx = (x + j) + (y + i) * mask.width;
                            if( idx < pixels.Length)
                            {
                                if (alpha < pixels[idx].a)
                                    pixels[idx].a = alpha;
                            }
                        }
                    }
                }
            }
        }

        mask.SetPixels(pixels);
        mask.Apply();
    }
}
