using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehavior : MonoBehaviour {


    public GameObject Creator;
    public float shotspeed = 0.5f;
    public Vector2 angle;

    public void Init(GameObject creator, Vector2 initialPosition, Vector2 Right)
    {
        Creator = creator;
        transform.position = initialPosition;
        angle = Right;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += (Vector3)(angle * shotspeed); 
	}
}
