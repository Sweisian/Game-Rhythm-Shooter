﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InControl;

public class Character_Behavior : MonoBehaviour
{
    public float shootdelay = 0.1f;
    protected float shotready = 0.0f;
    public GameObject shot;
    public GameObject GameManage;

    
    public Vector2 LastAim;

    public float speed = 10;
    [SerializeField] public float jumpvelocity = 20;


    protected bool isgrounded = true;
    protected bool jump = false;
    protected bool dash = false;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    protected Script_Trigger myTrigger;
    protected Rigidbody2D mybody;
    protected ParticleSystem[] particles;
    protected Script_DashMove myDashMove;
    protected BeatObserver beatObserver;
    protected Animator myAnim;
    protected SpriteRenderer mySpriteRen;
    protected InputDevice player;
    protected Script_Beat_Bar beatBarScript;

    protected bool onBeat = false;

    public bool forceOnBeat = true;

    public Boolean facingRight = true;

    [SerializeField] public GameObject beatBar;
    

    [Range(0, 1)]
    public int playerNumber;


    //SHOULD BE COVERED IN SCRIPT_PLAYERONE
    //void Awake()
    //{
    //    beatBarScript = beatBar.GetComponent<Script_Beat_Bar>();
    //    //cpurrGameObject = this.gameObject;
    //    player = InputManager.Devices[playerNumber];

    //    myAnim = GetComponent<Animator>();
    //    mySpriteRen = GetComponent<SpriteRenderer>();

    //    //added code to find the trigger object and script
    //    myTrigger = GameObject.FindGameObjectWithTag("Trigger").GetComponent<Script_Trigger>();

    //    mybody = GetComponent<Rigidbody2D>();
    //    particles = GetComponentsInChildren<ParticleSystem>();
    //    beatObserver = GetComponent<BeatObserver>();

    //    myDashMove = GetComponent<Script_DashMove>();

    //    if (myTrigger == null)
    //        throw new Exception("a Trigger Object was not found");
    //}

    // Update is called once per frame
    void Update()
    {

        InputDevice player = InputManager.Devices[playerNumber];

        if (player.Action1.WasPressed || Input.GetKeyDown(KeyCode.J))
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
        if ((player.Action2.WasPressed && isgrounded) || (Input.GetKeyDown(KeyCode.K) && isgrounded))
        {
            Debug.Log("B Pressed player 1");
            jump = true;
        }

        //dash
        if (player.Action3.WasPressed || Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("X Pressed by player 1");
            dash = true;
        }
       
    }

    void FixedUpdate()
    {
        InputDevice player = InputManager.Devices[0];

        InputControl movecontrol = player.GetControl(InputControlType.LeftStickX);
        InputControl aimcontrol = player.GetControl(InputControlType.LeftStickY);

        myAnim.SetFloat("moveSpeed", Mathf.Abs(movecontrol.Value));

        //Sets orientation of sprite
        if (movecontrol.Value > .01f ||  Input.GetKey(KeyCode.D))
            facingRight = true;

        if (movecontrol.Value < -.01f || Input.GetKey(KeyCode.A))
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
                    Dash(movecontrol, myDashMove);
                    myTrigger.BeatHit();
                    particles[0].Play();
                }
                else particles[1].Play();                
            }
            else
            {
                Dash(movecontrol, myDashMove);               
                //Debug.Log("I DASHED");
            }

            dash = false;         
        }

        //Might be important this stays at the end
        Move(movecontrol.Value, mybody, myDashMove);
    }

    public void Dash(InputControl movecontrol, Script_DashMove myDashMove)
    {
        if (movecontrol.Value < 0)
        {
            myDashMove.direction = 1;
        }
        else if (movecontrol.Value > 0)
        {
            myDashMove.direction = 2;
        }
        else if(movecontrol.Value == 0)
        {
            if(facingRight)
                myDashMove.direction = 2;
            if(!facingRight)
                myDashMove.direction = 1;
        }
        Debug.Log("I Dashed");
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
    }

    


}
