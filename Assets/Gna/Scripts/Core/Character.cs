using UnityEngine;
using System.Collections;

public class Character : PixelRigidbody
{
    // Use this for initialization
    /*void Start () {
	
	}*/

    // Update is called once per frame
    /*void Update () {
	
	}*/

    public void AddVelocity(Vector3 add)
    {
        velocity = velocity + add;
        //print("Actor::AddVelocity");
    }
}
