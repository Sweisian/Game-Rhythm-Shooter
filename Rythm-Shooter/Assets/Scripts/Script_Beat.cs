using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Beat : MonoBehaviour
{
    public float moveSpeed;

	// Use this for initialization
	void Start ()
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveSpeed , 0);
	}
	
}
