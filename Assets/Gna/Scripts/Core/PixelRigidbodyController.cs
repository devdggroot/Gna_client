using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PixelRigidbody))]
public class PixelRigidbodyController : MonoBehaviour
{
    PixelRigidbody pixelRigidbody;

    float moveSpeed = 1f; //(m/s)
    Vector3 velocity;

    gna.Physics.RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        pixelRigidbody = GetComponent<PixelRigidbody>();
        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        velocity.x = input.x * moveSpeed;

        Vector3 deltaMovement = velocity * Time.deltaTime;
        if (pixelRigidbody != null && deltaMovement.x != 0f)
        {
            //raycast y-axis(detect descend slope)
            Vector3 rayStart = pixelRigidbody.cachedTransform.position - Vector3.up * pixelRigidbody.radius;
            Vector3 rayEnd = rayStart - Vector3.up * 1024f;

            Debug.DrawRay(rayStart, rayEnd - rayStart, Color.blue);
            if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
            {
                Vector3 normal = hit.normal;
                if (normal.sqrMagnitude <= float.Epsilon)
                {
                    normal = Vector3.up;
                    print("pixelRigidbodyController::FixedUpdate hit normal is zero.");
                }

                float angle = Vector3.Angle(normal, Vector3.up);
                if (angle > float.Epsilon && Mathf.Sign(deltaMovement.x) == Mathf.Sign(normal.x))
                {
                    float slopeDistance = Mathf.Abs(deltaMovement.x);
                    if( hit.distance <= Mathf.Tan(angle * Mathf.Deg2Rad) * slopeDistance)
                    {
                        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * slopeDistance;
                        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * slopeDistance;

                        deltaMovement.x = x * Mathf.Sign(deltaMovement.x);
                        deltaMovement.y = -y;

                        //print("detect descned slope.");
                    }
                }
            }


            //raycast x-axis
            rayStart = pixelRigidbody.cachedTransform.position + Vector3.down * pixelRigidbody.radius;
            rayEnd = rayStart + Vector3.right * deltaMovement.x;

            Debug.DrawRay(rayStart, rayEnd - rayStart, Color.blue);
            if (TerrainRoot.instance.Raycast(new gna.Physics.Ray(rayStart, rayEnd), ref hit))
            {
                Vector3 normal = hit.normal;
                if (normal.sqrMagnitude <= float.Epsilon)
                {
                    normal = Vector3.up;
                    print("pixelRigidbodyController::FixedUpdate hit normal is zero.");
                }

                float angle = Vector3.Angle(normal, Vector3.up);
                if( angle <= float.Epsilon) //normal is Vector3.up
                {
                    //
                }
                else
                {
                    float slopeDistance, x, y;

                    float directionX = Mathf.Sign(deltaMovement.x);
                    if(directionX != Mathf.Sign(normal.x)) //climb slope
                    {
                        if (angle <= pixelRigidbody.LimitClimbAngle)
                        {
                            slopeDistance = Mathf.Abs(deltaMovement.x) - hit.distance;

                            x = Mathf.Cos(angle * Mathf.Deg2Rad) * slopeDistance;
                            y = Mathf.Sin(angle * Mathf.Deg2Rad) * slopeDistance;

                            deltaMovement.x = (hit.distance + x) * directionX;
                            deltaMovement.y = y;
                        }
                        else
                        {
                            deltaMovement = Vector3.zero;
                            print("LimitClimbAngle.");
                        }
                    }
                    /*else //descend slope
                    {
                        slopeDistance = Mathf.Abs(deltaMovement.x) - hit.distance;

                        x = Mathf.Cos(angle * Mathf.Deg2Rad) * slopeDistance;
                        y = Mathf.Sin(angle * Mathf.Deg2Rad) * slopeDistance;

                        deltaMovement.x = (hit.distance + x) * directionX;
                        deltaMovement.y = -y;
                    }*/
                }
            }

            velocity = Vector3.zero;
            pixelRigidbody.cachedTransform.position = pixelRigidbody.cachedTransform.position + deltaMovement;
            //print("pixelRigidbodyController::FixedUpdate deltaMovement = (" + deltaMovement.x + ", " + deltaMovement.y + ", " + deltaMovement.z + ").");
        }
    }
}
