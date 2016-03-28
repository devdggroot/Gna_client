using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class TerrainDestructor : MonoBehaviour
{
    public float thetaScale = 0.01f;  //limit?
    public float radius = 0.4f;

    public int resolution = 2;
    public float sensitive = 0.2f;

    LineRenderer lineRenderer;
    Vector3 lastDestroyedPos;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(0.02f, 0.02f);
    }

    void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float theta = 0f;
        int size = (int)((2f * Mathf.PI) / thetaScale) + 1;
        lineRenderer.SetVertexCount(size);

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        for (int i = 0; i < size; i++)
        {
            theta += (2.0f * Mathf.PI * thetaScale);

            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);

            x += pos.x;
            y += pos.y;

            lineRenderer.SetPosition(i, new Vector3(x, y, -1f));
        }

        //destroy
        if (Input.GetMouseButton(0))
        {
            float dist = Vector3.Distance(lastDestroyedPos, pos);
            if (dist > sensitive || Input.GetMouseButtonDown(0))
            {
                TerrainRoot.instance.Destoryed(pos, radius, resolution);
                lastDestroyedPos = pos;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            PixelCollider.RaycastHit hit = null;
            if (TerrainRoot.instance.Raycast(pos, new Vector3(pos.x, pos.y - 1000f, 0f), ref hit))
            {
                Debug.Log("hit object(" + hit.instance.name + ") position " + hit.coord.x + ", " + hit.coord.y + ", distSq : " + hit.distSq);
                hit = null;
            }
        }
    }
}
