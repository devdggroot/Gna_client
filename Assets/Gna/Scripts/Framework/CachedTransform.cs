using UnityEngine;
using System.Collections;

public class CachedTransform : MonoBehaviour
{
    Transform _cachedTransform;
    public Transform cachedTransform //{ get; private set; }
    {
        get
        {
            if (_cachedTransform == null)
                _cachedTransform = transform;

            return _cachedTransform;
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {
        _cachedTransform = transform;
    }

    protected virtual void OnDestroy()
    {
        _cachedTransform = null;
    }

    // Update is called once per frame
    /*void Update()
    {

    }*/
}
