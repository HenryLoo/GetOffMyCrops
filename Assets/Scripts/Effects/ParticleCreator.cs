using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCreator : Behaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void Emit( string particleType, Vector3 pos )
    {
        Instantiate( Resources.Load( particleType ), pos, 
            Quaternion.Euler( 90, 0, 0 ) );
    }
}
