using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class NormalRenderer : MonoBehaviour {

    public int resolution { get; private set; }
    public int changeResolution
    {
        set
        {
            if (resolution != value)
                resolution = value;
        }
    }

    public PixelCollider forNormal;

    // Use this for initialization
    void Start () {

        
    }

    void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public void Draw()
    {
        List<Vector2> edges = FindEdge(resolution);
        DrawNormal(edges);
    }

    List<Vector2> FindEdge( int resolution)
    {
        List<Vector2> edges = new List<Vector2>();

        if(forNormal != null)
        {
            int width = forNormal.width;
            int height = forNormal.height;

            for( int y = 0; y < height; y += resolution)
            {
                for( int x = 0; x < width; x += resolution)
                {
                    //0.left
                    PixelCollider.RaycastHit hit = null;
                    if( forNormal.Raycast(x, (int)((y + resolution) * 0.5f), x + resolution, (int)((y + resolution) * 0.5f), ref hit))
                    {
                        if( hit.sqrDist > 0f)
                        {
                            edges.Add( hit.coord);
                            continue;
                        }
                    }

                    //1.top
                    if (forNormal.Raycast((int)((x + resolution) * 0.5f), y + resolution, (int)((x + resolution) * 0.5f), y, ref hit))
                    {
                        if (hit.sqrDist > 0f)
                        {
                            edges.Add(hit.coord);
                            continue;
                        }
                    }

                    //2.right
                    if (forNormal.Raycast(x + resolution, (int)((y + resolution) * 0.5f), x, (int)((y + resolution) * 0.5f), ref hit))
                    {
                        if (hit.sqrDist > 0f)
                        {
                            edges.Add(hit.coord);
                            continue;
                        }
                    }

                    //3.bottom
                    if (forNormal.Raycast((int)((x + resolution) * 0.5f), y, (int)((x + resolution) * 0.5f), y + resolution, ref hit))
                    {
                        if (hit.sqrDist > 0f)
                        {
                            edges.Add(hit.coord);
                            continue;
                        }
                    }
                }
            }
        }

        return edges;
    }

    void DrawNormal(List<Vector2> edges)
    {
        if (forNormal != null)
        {
            Clear();

            for (int i = 0, imax = edges.Count; i < imax; ++i)
            {
                GameObject go = new GameObject("normal");

                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;

                LineRenderer lineRenderer = go.AddComponent<LineRenderer>();
                lineRenderer.SetWidth(0.02f, 0.02f);
                lineRenderer.SetVertexCount(2);

                Vector3 edge = new Vector3((edges[i].x - forNormal.pivot.x) / forNormal.pixelsPerUnit, (edges[i].y - forNormal.pivot.y) / forNormal.pixelsPerUnit, -1f);
                edge = forNormal.cachedTransform.TransformPoint(edge);

                lineRenderer.SetPosition(0, edge);

                Vector2 normal = forNormal.NormalAt((int)edges[i].x, (int)edges[i].y);
                edge.x += normal.x;
                edge.y += normal.y;

                lineRenderer.SetPosition(1, edge);
            }
        }
    }
}
