﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InControl;

public class Character_Behavior : MonoBehaviour
{
    //Game object specific elements assigned in Awake()
    private Animator myAnim;
    private Script_Trigger myTrigger;
    private Rigidbody2D mybody;
    private ParticleSystem[] particles;
    private BeatObserver beatObserver;
    private SpriteRenderer mySpriteRen;
    private InputDevice player;
    private Script_DashMove myDashMove;
    private Script_Beat_Bar beatBarScript;

    //Things we have to manually add to the game object
    [SerializeField] private GameObject shot;
    [SerializeField] private GameObject GameManage ;
    [SerializeField] private GameObject beatBar;

    //Important fields to be editable from the inspector
    [SerializeField] private float jumpvelocity = 20;
    [SerializeField] private float speed = 10;
    [SerializeField] private float shootdelay = 0.1f;
    [SerializeField] private float shotready = 0.0f;
    [SerializeField] private bool forceOnBeat = true;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [Range(0, 1)] [SerializeField] private int playerNumber = 0;

    //Things we don't care about seeing in the inpsector
    private Vector2 LastAim;
    private bool isgrounded = true;
    private bool jump = false;
    private bool dash = false;
    private bool facingRight = true;
    private bool onBeat = false;


    void Awake()
    {
        //This needs to go first, or we get problems
        myAnim = GetComponent<Animator>();
        Debug.Log("AWAKE myAnim is: " + myAnim);

        //If you don't have a controller plugged in, the game is UNPLAYABLE
        player = InputManager.Devices[playerNumber];
        Debug.Log("AWAKE player is: " + player);

        beatBarScript = beatBar.GetComponent<Script_Beat_Bar>();

        mySpriteRen = GetComponent<SpriteRenderer>();

        //added code to find the trigger object and script
        myTrigger = GameObject.FindGameObjectWithTag("Trigger").GetComponent<Script_Trigger>();

        mybody = GetComponent<Rigidbody2D>();
        particles = GetComponentsInChildren<ParticleSystem>();
        beatObserver = GetComponent<BeatObserver>();

        myDashMove = GetComponent<Script_DashMove>();

        if (myTrigger == null)
            throw new Exception("a Trigger Object was not found");

    }

    // Update is called once per frame
    void Update()
    {

        InputDevice player = InputManager.Devices[playerNumber];

        if (player.Action3.WasPressed || Input.GetKeyDown(KeyCode.J))
        {
            if (forceOnBeat)
            {
                Debug.Log("A Pressed player 1");
                if (beatBarScript.onBeat)
                //if (myTrigger.GetIsActive())
                {
                    Fire(this.gameObject, player);
                    particles[0].Play();
                    myTrigger.BeatHit();
                }
            }
            else
            {
                Fire(this.gameObject, player);
                particles[0].Play();
            }
        }
       
        //jump
        //if ((player.Action2.WasPressed && isgrounded) || (Input.GetKeyDown(KeyCode.K) && isgrounded))
        //{
        //    Debug.Log("B Pressed player 1");
        //    jump = true;
        //}

        //dash
        if (player.Action1.WasPressed || Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("X Pressed by player 1");
            dash = true;
        }
    }

    void FixedUpdate()
    {
        InputDevice player = InputManager.Devices[0];
        InputControl xControl = player.GetControl(InputControlType.LeftStickX);
        InputControl yControl = player.GetControl(InputControlType.LeftStickY);

        myAnim.SetFloat("moveSpeed", Mathf.Abs(xControl.Value));

        //Sets orientation of sprite
        if (xControl.Value > .01f ||  Input.GetKey(KeyCode.D))
            facingRight = true;

        if (xControl.Value < -.01f || Input.GetKey(KeyCode.A))
            facingRight = false;

        if(facingRight)
            transform.localScale = new Vector2(1, transform.localScale.y);
        if(!facingRight)
            transform.localScale = new Vector2(-1, transform.localScale.y);

        FallingPhysics(mybody);

        //Jump Ability        
        if (jump)
        {
            //Now checks if the trigger is active
            if (forceOnBeat)
            {
                //if (myTrigger.GetIsActive())
                if (beatBarScript.onBeat)
                {
                    Jump(mybody);
                    jump = false;
                    myTrigger.BeatHit();                   
                    if (isgrounded)
                        particles[0].Play();
                }
                else
                {
                    jump = false;
                    particles[1].Play();                    
                }
            }           
            else
            {
                Jump(mybody);
                jump = false;
                myTrigger.BeatHit();
                if (isgrounded)
                    particles[0].Play();
            }
        }
        
        //Dash Ability
        if (dash)
        {
            if (forceOnBeat)
            {
                //if (myTrigger.GetIsActive())
                if (beatBarScript.onBeat)
                {
                    Dash(xControl, myDashMove);
                    myTrigger.BeatHit();
                    particles[0].Play();
                }
                else particles[1].Play();                
            }
            else
            {
                Dash(xControl, myDashMove);               
                //Debug.Log("I DASHED");
            }

            dash = false;         
        }

        //Might be important this stays at the end
        Move(xControl.Value, mybody, myDashMove);
    }

    public void Dash(InputControl xControl, Script_DashMove myDashMove)
    {
        

        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        float Y = aimY.Value;
        float X = aimX.Value;

        float YAbsVal = Mathf.Abs(Y);
        float XAbsVal = Mathf.Abs(X);

        if (X < 0 && XAbsVal > YAbsVal)
        {
            myDashMove.direction = 1;
        }
        else if (X > 0 && XAbsVal > YAbsVal)
        {
            myDashMove.direction = 2;
        }
        else if (Y > 0 && YAbsVal > XAbsVal)
        {
            myDashMove.direction = 3;
        }
        else if (Y < 0 && YAbsVal > XAbsVal)
        {
            myDashMove.direction = 4;
        }
        else if(xControl.Value == 0)
        {
            if(facingRight)
                myDashMove.direction = 2;
            if(!facingRight)
                myDashMove.direction = 1;
        }
        //Debug.Log("I Dashed");

        //Needs to go at end because this depends on the myDashMove.direction value
        StartCoroutine("dashFlash");
    }

    public void FallingPhysics(Rigidbody2D mybody)
    {
        // player is falling
        if (mybody.velocity.y < 0)
        {
            mybody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            // minus 1 because unity is already applying 1 multiple of the gravity (don't want to add gravity part twice)
        }
        // player is moving up in the jump and a is released
        else if (mybody.velocity.y > 0) //&& !Input.GetButton("A_1"))
        {
            mybody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void Move(float horizontalInput, Rigidbody2D mybody, Script_DashMove myDashMove)
    {
        //Code for keyboard controls
        if(Input.GetKey(KeyCode.A))
            horizontalInput = -1f;

        if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;

        // separates movement into discrete speeds (crawl, walk, run)
        if (horizontalInput > 0 && horizontalInput <= 0.3f)
        {
            horizontalInput = 0.3f;
        }
        else if (horizontalInput > 0.3f && horizontalInput <= 0.9f)
        {
            horizontalInput = 0.5f;
        }
        else if (horizontalInput > 0.8f && horizontalInput <= 1)
        {
            horizontalInput = 1;
        }

        if (horizontalInput < 0 && horizontalInput >= -0.3f)
        {
            horizontalInput = -0.3f;
        }
        else if (horizontalInput < -0.3f && horizontalInput >= -0.9f)
        {
            horizontalInput = -0.5f;
        }
        else if (horizontalInput < -0.8f && horizontalInput > -1)
        {
            horizontalInput = -1;
        }

        //this is necessary so this code doesn't overwrite the dash velocity
        //Simply checks if we are dashing or not
        if (myDashMove.direction == 0)
        {
            mybody.velocity = new Vector2(horizontalInput * speed, mybody.velocity.y);
        }
    }

    public void Jump(Rigidbody2D mybody)
    {
        //Debug.Log("Jump velocity is: ");
        //Debug.Log(jumpvelocity);
        //Debug.Log("mybody.velocity before jump is: ");
        //Debug.Log(mybody.velocity);
        
        mybody.velocity += jumpvelocity * Vector2.up; //* Time.deltaTime;
    }

    public void Fire(GameObject currGameObject, InputDevice player)
    {
        Vector2 Temp = new Vector2();
        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        Aim(currGameObject,player);
        if (currGameObject.GetComponent<Character_Behavior2>() != null)
            Temp = currGameObject.GetComponent<Character_Behavior2>().LastAim;
        else
        {
            Temp = LastAim;
        }
        GameObject shotcreate = Instantiate(shot);
        ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();

        shotinit.Init(currGameObject, (Vector2)currGameObject.transform.position + (1.5F * (Temp)), Temp);
        shotready = Time.time + shootdelay;
    }

    void Aim(GameObject curr, InputDevice player)
    {
        //InputDevice player = InputManager.Devices[0];
        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        float Y = aimY.Value;
        float X = aimX.Value;
        float tan = Mathf.Atan2(Y, X);
        Vector2 temp = new Vector2();

        //New way of shooting
        if (facingRight)
        {
            temp = new Vector2(1, 0);
        }
        if (!facingRight)
        {
            temp = new Vector2(-1, 0);
        }

        LastAim = temp;
        

        /* This is the old way of shooting. It still is buggy

        if (Y == 0 && X == 0 && facingRight)
        {
            temp = new Vector2(1, 0);
        }
        if (Y == 0 && X == 0 && !facingRight)
        {
            temp = new Vector2(-1, 0);
        }

       
        if (X < 0 && tan < Mathf.PI * 7 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI / 2), Mathf.Sin(Mathf.PI / 2));
        }
        if (X < 0 && tan > Mathf.PI * 7 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 2 / 3), Mathf.Sin(Mathf.PI * 2 / 3));
        }
        if (X < 0 && tan > Mathf.PI * 3 / 4)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 5 / 6), Mathf.Sin(Mathf.PI * 5 / 6));
        }
        if (X < 0 && tan > Mathf.PI * 11 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI), Mathf.Sin(Mathf.PI));
        }
        if (X < 0 && tan < Mathf.PI * 13 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 7 / 6), Mathf.Sin(Mathf.PI * 7 / 6));
        }
        if (X < 0 && tan < Mathf.PI * 5 / 4)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 4 / 3), Mathf.Sin(Mathf.PI * 4 / 3));
        }
        if (X < 0 && tan < Mathf.PI * 17 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 3 / 2), Mathf.Sin(Mathf.PI * 3 / 2));
        }

        //This should be left shooting
        if (X < 0 && tan < Mathf.PI / 12)
        {
            temp = new Vector2(-1, 0);
        }

        if (X > 0 && tan > Mathf.PI * 5 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI / 2), Mathf.Sin(Mathf.PI / 2));
        }
        if (X > 0 && tan < Mathf.PI * 5 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI / 3), Mathf.Sin(Mathf.PI / 3));
        }
        if (X > 0 && tan < Mathf.PI / 4)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI / 6), Mathf.Sin(Mathf.PI / 6));
        }
        if (X > 0 && tan < Mathf.PI / 12)
        {
            temp = new Vector2(Mathf.Cos(0), Mathf.Sin(0));
        }
        if (X > 0 && tan > Mathf.PI * 23 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 11 / 6), Mathf.Sin(Mathf.PI * 11 / 6));
        }
        if (X > 0 && tan > Mathf.PI * 7 / 4)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 5 / 3), Mathf.Sin(Mathf.PI * 5 / 3));
        }
        if (X > 0 && tan > Mathf.PI * 19 / 12)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 3 / 2), Mathf.Sin(Mathf.PI * 3 / 2));
        }
        if (Y > 0.8)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI / 2), Mathf.Sin(Mathf.PI / 2));
        }
        if (Y < -0.8)
        {
            temp = new Vector2(Mathf.Cos(Mathf.PI * 3 / 2), Mathf.Sin(Mathf.PI * 3 / 2));
        }

        if (curr.GetComponent<Character_Behavior2>() != null) { 
            curr.GetComponent<Character_Behavior2>().LastAim = temp; }
        else
        {
            LastAim = temp;
        }

       */
    }

    // Detect continous collision with the ground
    void OnCollisionStay2D(Collision2D hit)
    {
        if (hit.gameObject.tag == "Ground")
        {
            //Debug.Log("myAnim is: " + myAnim);
            isgrounded = true;
            myAnim.SetBool("isJump", false);
        }
    }

    // Detect collision exit with ground
    void OnCollisionExit2D(Collision2D exit)
    {
        if (exit.gameObject.tag == "Ground")
        {
            isgrounded = false;
            myAnim.SetBool("isJump", true);
        }
    }

    IEnumerator dashFlash()
    {
        //Debug.Log("called dashFlash");
        //Debug.Log(myDashMove.direction);
        Color32 c = mySpriteRen.color;
        while (myDashMove.direction != 0)
        {
            //Debug.Log("inside dashFlash");
            mySpriteRen.color = Color.black;
            yield return new WaitForSeconds(0.03f);
            mySpriteRen.color = c;
            yield return new WaitForSeconds(0.03f);
        }
        mySpriteRen.color = c;
    }


}
