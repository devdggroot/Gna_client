using UnityEngine;
using System.Collections;

public class TerrainObject : MonoBehaviour {

    public Transform cachedTransform { get; private set; }

    // Use this for initialization
    protected virtual void Start()
    {
        cachedTransform = transform;
    }

    protected virtual void OnDestroy()
    {
        cachedTransform = null;
    }

    // Update is called once per frame
    /*void Update () {
	
	}*/

    public virtual void Destroyed(int xPos, int yPos, float radius, int resolution) { }
}
