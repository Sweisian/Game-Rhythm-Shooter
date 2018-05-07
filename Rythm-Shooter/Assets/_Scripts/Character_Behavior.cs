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
    [SerializeField] public float acceleration = 5f;   //lookup how to do have friend scripts in c#
    [SerializeField] public float jumpvelocity = 10;
    public float dashVelocity = 10;


    bool isgrounded = true;
    private bool jump = false;
    private bool dash = false;
    Vector2 moveVel;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    Rigidbody2D mybody;
    private ParticleSystem[] particles;

    private BeatObserver beatObserver;

    private bool onBeat = false;

    public bool forceOnBeat = true;

    private Animator myAnim;
    private SpriteRenderer mySpriteRen;

    //NEW//
    private GameObject currGameObject;
    private InputDevice player;

    private Script_DashMove myDashMove;

    public Boolean facingRight = true;

    void Awake()
    {
        currGameObject = this.gameObject;
        player = InputManager.Devices[0];

        myAnim = GetComponent<Animator>();
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

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        InputDevice player = InputManager.Devices[0];

        if (player.Action1.WasPressed)
        {
            if (forceOnBeat)
            {
                Debug.Log("A Pressed player 1");
                if (myTrigger.GetIsActive())
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
        if (player.Action2.WasPressed && isgrounded)
        {
            Debug.Log("B Pressed player 1");
            jump = true;
        }

        //dash
        if (player.Action3.WasPressed)
        {
            dash = true;
        }
       
    }

    void FixedUpdate()
    {
        InputDevice player = InputManager.Devices[0];
        InputControl movecontrol = player.GetControl(InputControlType.LeftStickX);
        InputControl aimcontrol = player.GetControl(InputControlType.LeftStickY);
        Move(movecontrol.Value, mybody);

        myAnim.SetFloat("moveSpeed", Mathf.Abs(movecontrol.Value));

        //Sets orientation of sprite
        if (movecontrol.Value > .01f)
            facingRight = true;

        if (movecontrol.Value < -.01f)
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
                if (myTrigger.GetIsActive())
                {
                    Jump(mybody);
                    myTrigger.BeatHit();
                    jump = false;
                    if (isgrounded)
                        particles[0].Play();
                }
                else
                {
                    particles[1].Play();
                    jump = false;
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
                if (myTrigger.GetIsActive())
                {
                    if (movecontrol.Value < 0)
                    {
                        myDashMove.direction = 1;
                    }
                    else if (movecontrol.Value > 0)
                    {
                        myDashMove.direction = 2;
                    }

                    myTrigger.BeatHit();
                    particles[0].Play();
                }
            }
            else
            {
                if (movecontrol.Value < 0)
                {
                    myDashMove.direction = 1;
                }
                else if (movecontrol.Value > 0)
                {
                    myDashMove.direction = 2;
                }
                particles[1].Play();
                Debug.Log("I DASHED");
            }

            dash = false;         
        }      
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

    public void Move(float horizontalInput, Rigidbody2D mybody)
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
        // use local acceleration and speed variables (don't need to assign in CharB2)
        /*
        Vector2 direction = new Vector2(horizontalInput, 0f);
        var goal = direction * speed;
        Vector2 error = new Vector2(goal.x - moveVel.x, 0f);
        mybody.AddForce(acceleration * error);
        */
        mybody.velocity = new Vector2(horizontalInput*speed, mybody.velocity.y);
    }


    public void Dash(float horzontalInput, float verticalInput)
    {
        Vector2 myVector = new Vector2(horzontalInput, verticalInput);
        myVector.Normalize();
        mybody.velocity += dashVelocity * myVector;
        dash = false;
    }

    public void Jump(Rigidbody2D mybody)
    {
        Debug.Log("Jump velocity is: ");
        Debug.Log(jumpvelocity);
        Debug.Log("mybody.velocity before jump is: ");
        Debug.Log(mybody.velocity);
        
        mybody.velocity += jumpvelocity * Vector2.up; //* Time.deltaTime;
        //Vector2 jumpForce = new Vector2(0f, jumpvelocity);
        //mybody.AddForce(jumpForce); 
    }

    public void Fire(GameObject currGameObject, InputDevice player)
    {
        
        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        Vector2 Temp = Aim(player);
        GameObject shotcreate = Instantiate(shot);
        ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();

        shotinit.Init(currGameObject, (Vector2)currGameObject.transform.position + (1.5F * (Temp)), Temp);
        shotready = Time.time + shootdelay;
    }

    Vector2 Aim(InputDevice player)
    {
        //InputDevice player = InputManager.Devices[0];
        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        float Y = aimY.Value;
        float X = aimX.Value;
        float tan = Mathf.Atan2(Y,X);

        if(Y == 0 && X == 0 && facingRight)
        {
            LastAim = new Vector2(1, 0);
        }
        if (Y == 0 && X == 0 && !facingRight)
        {
            LastAim = new Vector2(-1, 0);
        }


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
