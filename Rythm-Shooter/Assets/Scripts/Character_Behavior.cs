using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_Behavior : MonoBehaviour
{
    public float shootdelay = 0.1f;
    private float shotready = 0.0f;
    public GameObject shot;
    public GameObject GameManage;

    private Script_Trigger myTrigger;

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

    // Use this for initialization
    void Start()
    {
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
        //temp code to test the trigger functionality
        if (myTrigger.GetIsActive() && Input.GetKeyDown(KeyCode.F))
        {
            Fire();
            myTrigger.BeatHit();
        }

        //if (Input.GetAxisRaw("TriggersR_1") < -0.1 && Time.time > shotready)
        //    Fire();

        // Uncomment this when ready
        if (Input.GetButtonDown("A_1") && myTrigger.GetIsActive())
        {
            Fire();
            myTrigger.BeatHit();
        }       
    }

    void FixedUpdate()
    {
        FallingPhysics();

        Move(Input.GetAxisRaw("L_XAxis_1"));

        //Jump Ability
        if (Input.GetButtonDown("Y_1"))
        {
            //Debug.Log("Y Pressed");

            //Now checks if the trigger is active
            if (myTrigger.GetIsActive())
            {
                Jump();
                myTrigger.BeatHit();
                if (isgrounded)
                    particles[0].Play();
            }
            else
            {
                if (isgrounded) particles[1].Play();
            }
        }

        //Dash Ability
        if (Input.GetButtonDown("B_1"))
        {
            if (myTrigger.GetIsActive())
            {
                //negative on the y to invert stick for some reason
                Dash(Input.GetAxisRaw("L_XAxis_1"), -Input.GetAxisRaw("L_YAxis_1"));
                myTrigger.BeatHit();
                particles[0].Play();
            }
            else
            {
                particles[1].Play();
            }
        }
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
            mybody.velocity += jumpvelocity * Vector2.up;
    }

    void Fire()
    {
        Vector2 Temp = Aim();
        GameObject shotcreate = Instantiate(shot);
        ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();
        shotinit.Init(this.gameObject, (Vector2)transform.position + (1.5F * (Temp)), Temp);
        shotready = Time.time + shootdelay;
    }

    Vector2 Aim()
    {
        Vector2 Temp;
        Temp = new Vector2(1, 0);
        float Y = Input.GetAxisRaw("L_YAxis_1");
        float X = Input.GetAxisRaw("L_XAxis_1");
        float tan = Mathf.Atan2(Y,X);
        if (X < 0 && tan < Mathf.PI*7/12)
        {
            Temp = new Vector2 (Mathf.Cos(Mathf.PI/2),Mathf.Sin(Mathf.PI/2));
        }
        if (X < 0 && tan > Mathf.PI*7/12)
        {
            Temp = new Vector2(Mathf.Cos(Mathf.PI *2/3), Mathf.Sin(Mathf.PI *2/3));
        }
        if (X < 0 && tan > Mathf.PI*3/4)
        {
            Temp = new Vector2(Mathf.Cos(Mathf.PI * 5/6), Mathf.Sin(Mathf.PI * 5/6));
        }
        if (X < 0 && tan > Mathf.PI*11/12)
        {
            Temp = new Vector2(Mathf.Cos(Mathf.PI), Mathf.Sin(Mathf.PI));
        }
        if (X > 0 && tan > Mathf.PI*5/12)
        {
            Temp = new Vector2(Mathf.Cos(Mathf.PI/2), Mathf.Sin(Mathf.PI/2));
        }
        if (X > 0 && tan < Mathf.PI*5/12)
        {
            Temp = new Vector2(Mathf.Cos(Mathf.PI/3), Mathf.Sin(Mathf.PI/3));
        }
        if (X > 0 && tan < Mathf.PI/4)
        {
            Temp = new Vector2(Mathf.Cos(Mathf.PI/6), Mathf.Sin(Mathf.PI/6));
        }
        if (X > 0 && tan < Mathf.PI/12)
        {
            Temp = new Vector2(Mathf.Cos(0), Mathf.Sin(0));
        }
        if (Y > 0.8)
        {
            Temp = new Vector2(Mathf.Cos(Mathf.PI/2), Mathf.Sin(Mathf.PI/2));
        }
        print(Temp);
        return Temp;
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
        }
    }

    // Detect collision exit with ground
    void OnCollisionExit2D(Collision2D exit)
    {
        if (exit.gameObject.tag == "Ground")
        {
            isgrounded = false;
        }
    }


}
