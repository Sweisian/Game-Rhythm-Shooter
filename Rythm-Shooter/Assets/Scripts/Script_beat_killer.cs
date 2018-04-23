using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_beat_killer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D c)
    {
        Destroy(c.gameObject);
        //Debug.Log("Tried to kill :" + c);
    }
}
