using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Target_Behavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.position = new Vector2 (Random.Range(-7f, 7f),Random.Range(1,4.5f));

    }
}
