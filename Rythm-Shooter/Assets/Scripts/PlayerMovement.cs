using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float threshold = -20;
    public float speed = 10;
    public float jumpvelocity = 10;
    public LayerMask playerMask;
    public bool canmoveinair = true;
    Transform myTransform, tagGround;
    Rigidbody2D mybody;
    bool isgrounded = true;
    private Vector2 pos;
    Vector2 moveVel;
    public float scalingFactor = 0.36f;
    public float minSpeed = 9f;
    public float maxSpeed = 5;

    // Use this for initialization
    void Start()
    {
        mybody = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();
        pos = myTransform.position;
        tagGround = GameObject.FindGameObjectWithTag("Ground").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(Input.GetAxisRaw("L_XAxis_1"));
        //Respawn();
        //CheckDeath();
        if (Input.GetButtonDown("A_1"))
        {
            Jump();
        }
    }


    public void Move(float horzontalInput)
    {
        if (!canmoveinair && !isgrounded)
            return;

        moveVel = mybody.velocity;

        if (Input.GetAxisRaw("L_XAxis_1") == 0)
        {
            // when movement keys are not currently pressed
            if (isgrounded && Mathf.Abs(mybody.velocity.x) < 1)
            {
                // if circle is grounded and almost not moving
                // this prevents the circle from drifting when the player isn't moving
                moveVel.x = 0f;
            }
            if (!isgrounded)
            {
                // if in the air
                if (Mathf.Abs(moveVel.x) <= minSpeed)
                {
                    return; //don't slow down anymore
                }
            }
            if (moveVel.x > 0) //moving to the right
                moveVel.x -= speed * scalingFactor;
            else if (moveVel.x < 0) //moving to the left
                moveVel.x += speed * scalingFactor;
            mybody.velocity = moveVel; //set the circle's velocity
        }
        // else movement keys are pressed	
        else if (Mathf.Abs(moveVel.x) < maxSpeed)
        {
            moveVel.x += horzontalInput * speed;
            mybody.velocity = moveVel;
        }
        else
        {
            // don't increase speed if maxSpeed has been reached
            moveVel.x = horzontalInput * maxSpeed;
            mybody.velocity = moveVel;
        }
    }


    public void Jump()
    {
        if (isgrounded)
            mybody.velocity += jumpvelocity * Vector2.up;
    }

    // Detect continous collision with the ground
    void OnCollisionStay2D(Collision2D hit)
    {
        if (hit.gameObject.tag == "Ground" || hit.gameObject.tag == "boxToPush")
        {
            isgrounded = true;
        }
    }

    // Detect collision exit with ground
    void OnCollisionExit2D(Collision2D exit)
    {
        if (exit.gameObject.tag == "Ground" || exit.gameObject.tag == "boxToPush")
        {
            isgrounded = false;
        }
    }
}
