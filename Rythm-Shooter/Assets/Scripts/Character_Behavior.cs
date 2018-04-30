using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InControl;

public class Character_Behavior : MonoBehaviour
{
    public float shootdelay = 0.1f;
    private float shotready = 0.0f;
    public GameObject shot;
    public GameObject GameManage;

    private Script_Trigger myTrigger;
    public Vector2 LastAim;

    public float speed = 10;
    [SerializeField] private float acceleration = 5f;
    public float jumpvelocity = 20;
    public float dashVelocity = 10;


    bool isgrounded = true;
    Vector2 moveVel;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    Rigidbody2D mybody;
    private ParticleSystem[] particles;

    private BeatObserver beatObserver;

    private bool onBeat = false;

    private bool localIsActive = false;

    private Animator myAnim;
    private SpriteRenderer mySpriteRen;

    // Use this for initialization
    void Start()
    {
        myAnim = GetComponent<Animator>();
        mySpriteRen = GetComponent<SpriteRenderer>();

        //added code to find the trigger object and script
        myTrigger = GameObject.FindGameObjectWithTag("Trigger").GetComponent<Script_Trigger>();

        mybody = GetComponent<Rigidbody2D>();
        particles = GetComponentsInChildren<ParticleSystem>();
        beatObserver = GetComponent<BeatObserver>();

        if (myTrigger == null)
            throw new Exception("a Trigger Object was not found");
    }

    // Update is called once per frame
    void Update()
    {


        InputDevice player = InputManager.Devices[0];
        /*
        //temp code to test the trigger functionality
        if (localIsActive && Input.GetKeyDown(KeyCode.F))
        {
            Fire();
            myTrigger.BeatHit(localIsActive);
        }
        */

        //if (Input.GetAxisRaw("TriggersR_1") < -0.1 && Time.time > shotready)
        //    Fire();

        // Uncomment this when ready
/*
        if (Input.GetButtonDown("A_1") && localIsActive)
*/
        if (player.Action1.WasPressed)
        {
            Debug.Log("A Pressed");
            if (myTrigger.GetIsActive())
            {
                Fire();
                particles[0].Play();
                myTrigger.BeatHit(localIsActive);
            }
        }
        

    }

    void FixedUpdate()
    {
        InputDevice player = InputManager.Devices[0];
        InputControl movecontrol = player.GetControl(InputControlType.LeftStickX);
        InputControl aimcontrol = player.GetControl(InputControlType.LeftStickY);
        Move(movecontrol.Value);

        myAnim.SetFloat("moveSpeed", Mathf.Abs(movecontrol.Value));

        //Sets orientation of sprite
        if (movecontrol.Value > .01f)
            transform.localScale = new Vector2(1, transform.localScale.y);

        if (movecontrol.Value < -.01f)
            transform.localScale = new Vector2(-1, transform.localScale.y);

        FallingPhysics();

        //Jump Ability
        
        if (player.Action2.WasPressed)
        {
            Debug.Log("B Pressed");

            //Now checks if the trigger is active
            //if (localIsActive && myTrigger.GetIsActive())
            if (myTrigger.GetIsActive())

            {
                Jump();
                myTrigger.BeatHit(localIsActive);
                if (isgrounded)
                    particles[0].Play();
            }
            else
            {
                if (isgrounded)
                {
                    particles[1].Play();
                }

            }
        }
        
        if (isgrounded)
        {
            localIsActive = true;
        }

        
        //Dash Ability
        if (player.Action3.WasPressed)
        {
            Debug.Log("X pressed");
            if (myTrigger.GetIsActive())
            //if (localIsActive)
            {
                //negative on the y to invert stick for some reason
                Dash(movecontrol.Value, -aimcontrol);
                myTrigger.BeatHit(localIsActive);
                particles[0].Play();
            }
            else
            {
                particles[1].Play();
            }
        }
        
    }

    void UpdateGlobalActive()
    {
        localIsActive = myTrigger.GetIsActive();
    }

    void FallingPhysics()
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

    public void Move(float horizontalInput)

    {
        //Debug.Log(horizontalInput);
        moveVel = mybody.velocity;

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

        Vector2 direction = new Vector2(horizontalInput, 0f);
        var goal = direction * speed;
        Vector2 error = new Vector2(goal.x - moveVel.x, 0f);
        mybody.AddForce(acceleration * error);
    }


    public void Dash(float horzontalInput, float verticalInput)
    {
        Vector2 myVector = new Vector2(horzontalInput, verticalInput);
        myVector.Normalize();
        mybody.velocity += dashVelocity * myVector;
    }

    public void Jump()
    {
        if (isgrounded)
            mybody.velocity += jumpvelocity * Vector2.up * Time.deltaTime;
    }

    void Fire()
    {
        InputDevice player = InputManager.Devices[0];
        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        Vector2 Temp = Aim();
        GameObject shotcreate = Instantiate(shot);
        ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();
        shotinit.Init(this.gameObject, (Vector2)transform.position + (1.5F * (Temp)), Temp);
        shotready = Time.time + shootdelay;
    }

    Vector2 Aim()
    {
        InputDevice player = InputManager.Devices[0];
        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        float Y = aimY.Value;
        float X = aimX.Value;
        float tan = Mathf.Atan2(Y,X);
        if (X < 0 && tan < Mathf.PI*7/12)
        {
            LastAim = new Vector2 (Mathf.Cos(Mathf.PI/2),Mathf.Sin(Mathf.PI/2));
        }
        if (X < 0 && tan > Mathf.PI*7/12)
        {
            LastAim = new Vector2(Mathf.Cos(Mathf.PI *2/3), Mathf.Sin(Mathf.PI *2/3));
        }
        if (X < 0 && tan > Mathf.PI*3/4)
        {
            LastAim = new Vector2(Mathf.Cos(Mathf.PI * 5/6), Mathf.Sin(Mathf.PI * 5/6));
        }
        if (X < 0 && tan > Mathf.PI*11/12)
        {
            LastAim = new Vector2(Mathf.Cos(Mathf.PI), Mathf.Sin(Mathf.PI));
        }
        if (X > 0 && tan > Mathf.PI*5/12)
        {
            LastAim  = new Vector2(Mathf.Cos(Mathf.PI/2), Mathf.Sin(Mathf.PI/2));
        }
        if (X > 0 && tan < Mathf.PI*5/12)
        {
            LastAim = new Vector2(Mathf.Cos(Mathf.PI/3), Mathf.Sin(Mathf.PI/3));
        }
        if (X > 0 && tan < Mathf.PI/4)
        {
            LastAim = new Vector2(Mathf.Cos(Mathf.PI/6), Mathf.Sin(Mathf.PI/6));
        }
        if (X > 0 && tan < Mathf.PI/12)
        {
            LastAim = new Vector2(Mathf.Cos(0), Mathf.Sin(0));
        }
        if (Y > 0.8)
        {
            LastAim = new Vector2(Mathf.Cos(Mathf.PI/2), Mathf.Sin(Mathf.PI/2));
        }
        return LastAim;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<ShotBehavior>() != null)
        {
            GameManage.GetComponent<Script_GameManager>().respawn(this.gameObject);
            Destroy(collision.gameObject);
        }
    }

    // Detect continous collision with the ground
    void OnCollisionStay2D(Collision2D hit)
    {
        if (hit.gameObject.tag == "Ground")
        {
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


}
