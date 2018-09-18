using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sow : MonoBehaviour {

    public GameObject player;

    public GameObject seed;

    private Vector3 currentPosition;

    void LateUpdate()
    {
       

    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown("r"))
        {
            currentPosition = new Vector3(player.transform.position.x, 0.1f, player.transform.position.z);

            Instantiate(seed, currentPosition, new Quaternion(45,0,0,0));

        }
    }
}
