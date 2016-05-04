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
            public Vector3 origin { get; private set; }
            public Vector3 direction { get; private set; }
            public float length { get; private set;}
            
            public Vector3 destination { get; private set; }

            public Ray(Vector3 start, Vector3 end)
            {
                direction = (end - start).normalized;

                origin = start;
                destination = end;

                length = Vector3.Distance(origin, destination);
            }

            public Ray(Vector3 origin, Vector3 direction, float length)
            {
                destination = origin + direction * length;

                this.origin = origin;
                this.direction = direction;
                this.length = length;
            }
        }

        public class RaycastHit
        {
            public Ray ray;
            public PixelCollider collider;

            public Vector3 point { get; private set; }
            public Vector3 normal { get; private set; }

            public float distance { get; private set; }

            public RaycastHit(PixelCollider collider, Vector3 point, Vector3 normal, float distance)
            {
                this.collider = collider;

                this.point = point;
                this.normal = normal;

                this.distance = distance;
            }
        }

        public static bool Raycast(int startX, int startY, int endX, int endY, PixelCollider collider, out RaycastHit hit)
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

            //int prevX = startX;
            //int prevY = startY;

            for (int curpixel = 0; curpixel <= numpixels; curpixel++)
            {
                if (x >= 0 && x < collider.width && y >= 0 && y < collider.height)
                {
                    int idx = x + y * collider.width;
                    if (idx < collider.pixels.Length && collider.pixels[idx].a > 0f)
                    {
                        Vector3 point = new Vector3((x - collider.pivot.x) / collider.pixelsPerUnit, (y - collider.pivot.y) / collider.pixelsPerUnit);
                        point = collider.cachedTransform.TransformPoint(point);

                        Vector3 origin = new Vector3((startX - collider.pivot.x) / collider.pixelsPerUnit, (startY - collider.pivot.y) / collider.pixelsPerUnit);
                        origin = collider.cachedTransform.TransformPoint(origin);

                        hit = new RaycastHit(collider, point, collider.NormalAt(x, y), Vector3.Distance(origin, point));
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

        public static bool Raycast(Ray ray, PixelCollider collider, out RaycastHit hit)
        {
            Vector3 start = collider.cachedTransform.InverseTransformPoint(ray.origin);
            Vector3 end = collider.cachedTransform.InverseTransformPoint(ray.destination);

            int startX = Mathf.RoundToInt(start.x * collider.pixelsPerUnit + collider.pivot.x);
            int startY = Mathf.RoundToInt(start.y * collider.pixelsPerUnit + collider.pivot.y);

            int endX = Mathf.RoundToInt(end.x * collider.pixelsPerUnit + collider.pivot.x);
            int endY = Mathf.RoundToInt(end.y * collider.pixelsPerUnit + collider.pivot.y);

            if (Raycast(startX, startY, endX, endY, collider, out hit))
            {
                hit.ray = ray;
            }
            
            return hit != null;
        }

        public static bool Raycast(Vector3 origin, Vector3 direction, float length, PixelCollider collider, out RaycastHit hit)
        {
            return Raycast(new Ray(origin, direction, length), collider, out hit);
        }
    }
}
