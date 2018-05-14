using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_DashMove : MonoBehaviour {

    private Rigidbody2D rb;
    public float dashSpeed;
    [HideInInspector] public float dashTime;
    public float startDashTime;

    [HideInInspector] public int direction;

    private bool afterDash = false;
    [Range(-1,0)] [SerializeField] private float gravityModifier = 0;
    [SerializeField] private float afterDashFloatTime = .25f;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        dashTime = startDashTime;
	}
	
	// To call a dash, simply change the direction field.
	void FixedUpdate () {
		if(direction == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                direction = 1;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                direction = 2;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                direction = 3;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                direction = 4;
            }
        }
        else
        {
            if (dashTime <= 0)
            {
                //starts the timer for the "after dash" period
                StartCoroutine("afterDashTimer");

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
                }
                else if (direction == 2)
                {
                    rb.velocity = Vector2.right * dashSpeed;
                }
                else if (direction == 3)
                {
                    rb.velocity = Vector2.up * dashSpeed;

                }
                else if (direction == 4)
                {
                    rb.velocity = Vector2.down * dashSpeed;
                }
            }
        }

        //this part modifys the gravity on the player after the dash is completed
        //This is designed to make the player "float" after dashing to give them a chance to dash again
        if (afterDash)
        {
            rb.AddForce(gravityModifier * Physics.gravity * rb.mass);
        }
    }

    IEnumerator afterDashTimer()
    {
        afterDash = true;
        yield return new WaitForSeconds(afterDashFloatTime);
        afterDash = false;
    }
}
