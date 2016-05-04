using UnityEngine;
using System.Collections;

public class TerrainRoot : MonoBehaviour
{
    static TerrainRoot _instance;
    public static TerrainRoot instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            Debug.Log("[Terrain] Root is not exist in current scene!");
            return null;
        }
    }

    CollisionTerrain[] terrains;

    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("[Terrain] Root is destroyed because new [Terrain] Root is created!");
            Destroy(_instance);
        }
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {

        terrains = GetComponentsInChildren<CollisionTerrain>();
    }

    void OnDestroy()
    {
        terrains = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    ///<summary>
    ///Destroy root's child terrains.
    ///</summary>
    ///<remarks>
    ///remarks.
    ///</remarks>
    ///<param name="pos">world coordinate.</param>
    public void Destoryed(Vector3 pos, float radius, int resolution)
    {
        for (int i = 0, imax = terrains.Length; i < imax; ++i)
        {
            Vector3 local = terrains[i].cachedTransform.InverseTransformPoint(pos);

            int xPos = (int)(local.x * terrains[i].pixelsPerUnit + terrains[i].pivot.x);
            int yPos = (int)(local.y * terrains[i].pixelsPerUnit + terrains[i].pivot.y);

            terrains[i].Destroyed(xPos, yPos, radius * terrains[i].pixelsPerUnit, resolution);
        }
    }

    ///<summary>
    ///Destroy root's child terrains.
    ///</summary>
    ///<param name="xPos">pixel coordinate.</param>
    ///<param name="yPos">pixel coordinate.</param>
    public void Destroyed(int xPos, int yPos, float radius, int resolution)
    {
        for (int i = 0, imax = terrains.Length; i < imax; ++i)
            terrains[i].Destroyed(xPos, yPos, radius, resolution);
    }

    ///<summary>
    ///Raycast root's child terrains and find shortest hit distance object.
    ///</summary>
    ///<param name="origin">world coordinate.</param>
    ///<param name="direction">...</param>
    ///<param name="length">...</param>
    ///<param name="hit">hit info</param>
    public bool Raycast(Vector3 origin, Vector3 direction, float length, out gna.Physics.RaycastHit hit)
    {
        hit = null;
        for (int i = 0, imax = terrains.Length; i < imax; ++i)
        {
            gna.Physics.RaycastHit temp;
            if (gna.Physics.Raycast(origin, direction, length, terrains[i], out temp))
            {
                if (hit == null || temp.distance < hit.distance)
                    hit = temp;
            }
        }

        return hit != null;
    }
}
