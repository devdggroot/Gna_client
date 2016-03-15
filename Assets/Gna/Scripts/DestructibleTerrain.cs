using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DestructibleTerrain : MonoBehaviour {

    public Texture2D raw;
    SpriteRenderer spriteRenderer;

    Texture2D mask;

    void OnEnable()
    {
        if (mask != null)
            Destroy(mask);

        mask = CreateMask(raw);
    }

    void OnDisable()
    {

    }

	// Use this for initialization
	void Start () {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            if (raw != null) //sprite
            {
                Sprite sprite = Sprite.Create(raw, new Rect(0f, 0f, raw.width, raw.height), new Vector2(0.5f, 0.5f));
                spriteRenderer.sprite = sprite;

                if (mask != null)
                   spriteRenderer.material.SetTexture("_MaskTex", mask); //instance material
            }
        }
	}

    void Destroy()
    {
        raw = null;
        mask = null;

        if (spriteRenderer != null)
        {//Hm....
            spriteRenderer.sprite = null;
            spriteRenderer.material = null; //instance material
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    Texture2D CreateMask( Texture2D raw)
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

            return newTex;
        }

        return null;
    }
}
