using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelCollider : CachedTransform
{
    public class RaycastHit
    {
        public PixelCollider instance { get; private set; }
        public Vector2 prev { get; private set; }
        public Vector2 coord { get; private set; }
        public float sqrDist { get; private set; }

        public Vector3 ray { get; set; }
        public Vector3 pixelCoord { get; set; }

        public RaycastHit(PixelCollider instance, Vector2 prev, Vector2 coord, float sqrDist)
        {
            this.instance = instance;
            this.prev = prev;
            this.coord = coord;
            this.sqrDist = sqrDist;
        }

        public Vector3 coordToWorld
        {
            get
            {
                Vector3 pos = Vector3.zero;

                pos.x = (coord.x - instance.pivot.x) / instance.pixelsPerUnit;
                pos.y = (coord.y - instance.pivot.y) / instance.pixelsPerUnit;

                return pos;
            }
        }
    }

    private class SortByVectorAngle : System.Collections.Generic.IComparer<Vector3>
    {
        private Vector3 to { get; set; }
        public SortByVectorAngle( Vector3 to) { this.to = to; }

        int System.Collections.Generic.IComparer<Vector3>.Compare(Vector3 x, Vector3 y)
        {
            float angleX = Vector3.Angle(x, to);
            float angleY = Vector3.Angle(y, to);

            return (int)(angleX - angleY);
        }
    }

    protected SpriteRenderer spriteRenderer;

    public int width { get; private set; }
    public int height { get; private set; }

    public Vector2 pivot { get; private set; }
    public float pixelsPerUnit { get; private set; }

    protected Color[] pixels;

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

    public bool Raycast(int startX, int startY, int endX, int endY, ref RaycastHit hit)
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
                if (idx < pixels.Length && pixels[idx].a > 0f)
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

    public bool Raycast(int startX, int startY, int endX, int endY, ref RaycastHit hit, PixelCollider collider, Vector3 ray)
    {
        if (collider == null)
            return Raycast(startX, startY, endX, endY, ref hit);

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

        int volume = Mathf.Min(collider.width, collider.height);

        System.Collections.Generic.List<Vector3> order = new System.Collections.Generic.List<Vector3>(4);
        order.Add(collider.cachedTransform.right.normalized);
        order.Add(collider.cachedTransform.up.normalized);
        order.Add((collider.cachedTransform.right * -1f).normalized);
        order.Add((collider.cachedTransform.up * -1f).normalized);

        order.Sort(new SortByVectorAngle(ray));

        int sensitivity = 5;
        Vector3 cross = Vector3.zero;
        for (int curpixel = 0, increment = 0; curpixel <= numpixels; curpixel += increment)
        {
            for (int i = 0; i <= sensitivity; ++i)
            {
                //center -> edge
                float radius = volume * Mathf.Lerp(0f, 0.5f, Mathf.InverseLerp(0, sensitivity, i));
                for(int axis = 0, axisMax = order.Count/*4*/; axis < axisMax; ++axis)
                {
                    Vector3 edge = order[axis] * radius;

                    cross.x = -edge.y;
                    cross.y = edge.x;

                    cross = cross.normalized;

                    //center + cross * volume * (0.5f ~ -0.5f)
                    for (int j = 0; j <= sensitivity; ++j)
                    {
                        float dist = radius * Mathf.Lerp(0.5f, -0.5f, Mathf.InverseLerp(0, sensitivity, j));
                        Vector3 coord = edge + (cross * dist);

                        coord.x += x;
                        coord.y += y;

                        if (coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height)
                        {
                            int idx = (int)coord.x + (int)coord.y * width;
                            if (idx < pixels.Length && pixels[idx].a > 0f)
                            {
                                float xDiff = (int)x / pixelsPerUnit - startX / pixelsPerUnit;
                                float yDiff = (int)y / pixelsPerUnit - startY / pixelsPerUnit;

                                float sqrDist = xDiff * xDiff + yDiff * yDiff;

                                //overlap
                                //Vector3.Dot(ray, coord);

                                Vector3 dir = coord;
                                dir.x -= x;
                                dir.y -= y;

                                float overlap = volume * 0.5f - Vector3.Dot(ray, dir);
                                if (overlap > 0f)
                                    Debug.Log("overlap : " + overlap);
                                Vector3 inverse = ray * -1f;
                                inverse = inverse * overlap;
                                //

                                hit = new RaycastHit(this, new Vector2(prevX, prevY), new Vector2(x + inverse.x, y + inverse.y), sqrDist);

                                //
                                hit.ray = ray;
                                hit.pixelCoord = coord;

                                return true;
                            }
                        }
                    }
                }
            }

            prevX = x;
            prevY = y;

            increment = Mathf.Min(curpixel + volume, numpixels) - curpixel;
            if (increment <= 0)
                increment = volume;//or break;

            num += (numadd * increment); // Increase the numerator by the top of the fraction

            if (num >= den)
            {  // Check if numerator >= denominator
                //num -= den; // Calculate the new numerator value
                int div = den > 0 ? num / den : 0;
                num = num - (div * den);

                x += (xinc1 * div); // Change the x as appropriate
                y += (yinc1 * div); // Change the y as appropriate
            }

            x += (xinc2 * increment); // Change the x as appropriate
            y += (yinc2 * increment); // Change the y as appropriate
        }

        hit = null;
        return false; // nothing was found
    }
}
