﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehavior : MonoBehaviour {


    public GameObject Creator;
    public float shotspeed = 0.5f;
    public Vector2 angle;

    public Script_GameManager GameManage;

    public void Init(GameObject creator, Vector2 initialPosition, Vector2 Right)
    {
        Creator = creator;
        transform.position = initialPosition;
        angle = Right;

        GameManage = GameObject.Find("GameManager").GetComponent<Script_GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += (Vector3)(angle * shotspeed); 
	}
    
    void OnCollisionEnter2D(Collision2D c)
    {
        Debug.Log("Shot entered: " + c.gameObject.name);

            if (c.gameObject.tag == "PlayerOne" || c.gameObject.tag == "PlayerTwo")
                {
                    //only respawn the other player if they are not dashing. Jank ass "I FRAMES"
                    if(c.gameObject.GetComponent<Script_DashMove>().direction == 0)
                        GameManage.respawn(c.gameObject);
                }

        Destroy(gameObject);
    }
}
