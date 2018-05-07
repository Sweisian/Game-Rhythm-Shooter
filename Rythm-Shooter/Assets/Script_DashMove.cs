using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DashMove : MonoBehaviour {

    private Rigidbody2D rb;
    public float dashSpeed;
    public float dashTime;
    public float startDashTime;


    [HideInInspector] public int direction;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        dashTime = startDashTime;
	}
	
	// To call a dash, simply change the direction field.
	void FixedUpdate () {
		if(direction == 0)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction = 1;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction = 2;
            }
        }
        else
        {
            if (dashTime <= 0)
            {
                direction = 0;
                dashTime = startDashTime;
                rb.velocity = Vector2.zero;
            }
            else
            {
                dashTime -= Time.deltaTime;

                if(direction == 1)
                {
                    rb.velocity = Vector2.left * dashSpeed;
                    Debug.Log(rb);
                    Debug.Log(rb.velocity);
                }
                else if (direction == 2)
                {
                    rb.velocity = Vector2.right * dashSpeed;
                    Debug.Log(rb);
                    Debug.Log(rb.velocity);
                }
            }
        }
	}
}
