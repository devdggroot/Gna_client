using UnityEngine;
using System.Collections;

namespace gna
{
    public static class Physics
    {
        public const float gravity = -9.8f;
        public const float velocityEpsilon = 0.00001f;


        public class Ray
        {
            public Vector3 dir { get; private set; }

            public Vector3 start { get; private set; }
            public Vector3 end { get; private set; }

            public Ray(Vector3 start, Vector3 end)
            {
                dir = end - start;
                dir.Normalize();

                this.start = start;
                this.end = end;
            }
        }

        public class RaycastHit
        {
            public Ray ray { get; private set; }
            public PixelCollider pixelCollider { get; private set; }

            public Vector3 point { get; private set; }
            public Vector3 normal { get; private set; }

            public float sqrDist { get; private set; }
            public float distance { get; private set; }

            public RaycastHit( Ray ray, PixelCollider pixelCollider, Vector3 point, Vector3 normal)
            {
                this.ray = ray;
                this.pixelCollider = pixelCollider;

                this.point = point;
                this.normal = normal;

                this.distance = Vector3.Distance(ray.start, point);
            }
        }

        public static bool Raycast( Ray ray, PixelCollider pixelCollider, ref RaycastHit hit)
        {
            Vector3 start = pixelCollider.cachedTransform.InverseTransformPoint(ray.start);
            Vector3 end = pixelCollider.cachedTransform.InverseTransformPoint(ray.end);

            int startX = Mathf.RoundToInt(start.x * pixelCollider.pixelsPerUnit + pixelCollider.pivot.x);
            int startY = Mathf.RoundToInt(start.y * pixelCollider.pixelsPerUnit + pixelCollider.pivot.y);

            int endX = Mathf.RoundToInt(end.x * pixelCollider.pixelsPerUnit + pixelCollider.pivot.x);
            int endY = Mathf.RoundToInt(end.y * pixelCollider.pixelsPerUnit + pixelCollider.pivot.y);

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

            //int prevX = startX;
            //int prevY = startY;

            for (int curpixel = 0; curpixel <= numpixels; curpixel++)
            {
                if (x >= 0 && x < pixelCollider.width && y >= 0 && y < pixelCollider.height)
                {
                    int idx = x + y * pixelCollider.width;
                    if (idx < pixelCollider.pixels.Length && pixelCollider.pixels[idx].a > 0f)
                    {//hit
                        Vector3 point = Vector3.zero;
                        point.x = (x - pixelCollider.pivot.x) / pixelCollider.pixelsPerUnit;
                        point.y = (y - pixelCollider.pivot.y) / pixelCollider.pixelsPerUnit;
                        point = pixelCollider.cachedTransform.TransformPoint(point);

                        //float xDiff = (x - startX) / pixelCollider.pixelsPerUnit;
                        //float yDiff = (y - startY) / pixelCollider.pixelsPerUnit;
                        //float sqrDist = (xDiff * xDiff + yDiff * yDiff);

                        hit = new RaycastHit( ray, pixelCollider, point, pixelCollider.NormalAt(x, y));
                        return true;
                    }
                }

                //prevX = x;
                //prevY = y;

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
    }
}
