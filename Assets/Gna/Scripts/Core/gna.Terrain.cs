using UnityEngine;
using System.Collections;

namespace gna
{
    public class Terrain : PixelCollider
    {
        // Use this for initialization
        /*void Start () {

        }*/

        // Update is called once per frame
        /*void Update () {

        }*/

        public virtual void Destroyed(int xPos, int yPos, float radius, int resolution) { }
        //
        //
    }
}
