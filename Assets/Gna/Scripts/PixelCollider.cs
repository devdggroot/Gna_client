using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelCollider : MonoBehaviour {

    public class RaycastHit
    {
        public PixelCollider instance { get; private set; }
        public Vector2 prev { get; private set; }
        public Vector2 coord { get; private set; }
        public float distSq { get; private set; }
        public RaycastHit(PixelCollider instance, Vector2 prev, Vector2 coord, float distSq)
        {
            this.instance = instance;
            this.prev = prev;
            this.coord = coord;
            this.distSq = distSq;
        }
    }

    public Transform cachedTransform { get; private set; }
    protected SpriteRenderer spriteRenderer;
    
    public int width { get; private set; }
    public int height { get; private set; }
    public Bounds bounds { get; private set; }
    public float pixelsPerUnit { get; private set; }

    protected Color[] pixels;

    // Use this for initialization
    protected virtual void Start () {

        cachedTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        width = spriteRenderer.sprite.texture.width;
        height = spriteRenderer.sprite.texture.height;

        bounds = spriteRenderer.sprite.bounds;
        pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        pixels = spriteRenderer.sprite.texture.GetPixels();
    }

    protected virtual void OnDestroy()
    {
        pixels = null;

        spriteRenderer = null;
        cachedTransform = null;
        
        Debug.Log("PixelCollider::OnDestroy");
    }

    // Update is called once per frame
    void Update () {

	}

    public bool Raycast( int startX, int startY, int endX, int endY, ref RaycastHit hit)
    {
        int deltaX = Mathf.Abs(endX - startX);
        int deltaY = Mathf.Abs(endY - startY);

        int x = startX;
        int y = startY;

        int xinc1, xinc2, yinc1, yinc2;
        if (endX >= startX)
        { // The x-values are increasing     
            xinc1 = 1;
            xinc2 = 1;
        }
        else { // The x-values are decreasing
            xinc1 = -1;
            xinc2 = -1;
        }

        if (endY >= startY)
        { // The y-values are increasing
            yinc1 = 1;
            yinc2 = 1;
        }
        else { // The y-values are decreasing
            yinc1 = -1;
            yinc2 = -1;
        }

        int den, num, numadd, numpixels;
        if (deltaX >= deltaY)
        { // There is at least one x-value for every y-value
            xinc1 = 0; // Don't change the x when numerator >= denominator
            yinc2 = 0; // Don't change the y for every iteration
            den = deltaX;
            num = deltaX / 2;
            numadd = deltaY;
            numpixels = deltaX; // There are more x-values than y-values
        }
        else { // There is at least one y-value for every x-value
            xinc2 = 0; // Don't change the x for every iteration
            yinc1 = 0; // Don't change the y when numerator >= denominator
            den = deltaY;
            num = deltaY / 2;
            numadd = deltaX;
            numpixels = deltaY; // There are more y-values than x-values
        }

        int prevX = startX;
        int prevY = startY;

        for (int curpixel = 0; curpixel <= numpixels; curpixel++)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                int idx = x + y * width;
                if (pixels[idx].a > 0f)
                {
                    float xDiff = x / pixelsPerUnit - startX / pixelsPerUnit;
                    float yDiff = y / pixelsPerUnit - startY / pixelsPerUnit;

                    hit = new RaycastHit(this, new Vector2(prevX, prevY), new Vector2(x, y), xDiff * xDiff + yDiff * yDiff);
                    return true;
                }
            }

            prevX = x;
            prevY = y;

            num += numadd; // Increase the numerator by the top of the fraction

            if (num >= den)
            {  // Check if numerator >= denominator
                num -= den; // Calculate the new numerator value
                x += xinc1; // Change the x as appropriate
                y += yinc1; // Change the y as appropriate
            }

            x += xinc2; // Change the x as appropriate
            y += yinc2; // Change the y as appropriate
        }

        hit = null;
        return false; // nothing was found
    }

    public virtual void Destroyed(int xPos, int yPos, float radius, int resolution) { }
}
