using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Target : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position = new Vector2(Random.Range(-5, 5), Random.Range(1, 4));
    }
}
