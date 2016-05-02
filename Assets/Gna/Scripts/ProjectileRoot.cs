using UnityEngine;
using System.Collections;

public class ProjectileRoot : MonoBehaviour
{
    static ProjectileRoot _instance;
    public static ProjectileRoot instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            Debug.Log("[Projectile] Root is not exist in current scene!");
            return null;
        }
    }

    void Awake()
    {
        if (_instance != null)
        {
            Debug.Log("[Projectile] Root is destroyed because new [Projectile] Root is created!");
            Destroy(_instance);
        }
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
