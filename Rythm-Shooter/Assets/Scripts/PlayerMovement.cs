using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class PlayerMovement : MonoBehaviour {

    public float speed = 10;
    [SerializeField] private float acceleration = 5f;
    public float jumpvelocity = 20;
    public float dashVelocity = 10;


    bool isgrounded = true;
    Vector2 moveVel;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    Rigidbody2D mybody;
    private ParticleSystem particles;

    private BeatObserver beatObserver;

    private bool onBeat = false;

    private Script_Trigger myTrigger;

    void Awake()
    {
        mybody = GetComponent<Rigidbody2D>();
        particles = GetComponentInChildren<ParticleSystem>();
        beatObserver = GetComponent<BeatObserver>();

        myTrigger = GameObject.FindGameObjectWithTag("Trigger").GetComponent<Script_Trigger>();
        if (myTrigger == null)
            Debug.Log("Didn't find a Trigger");
    }

    // Use this for initialization
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isgrounded);

        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            onBeat = true;
            //Debug.Log(onBeat);
        }
        else if ((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat)
        {
            onBeat = true;
            //Debug.Log(onBeat);
        }
        else
        {
            onBeat = false;
            //Debug.Log(onBeat);
        }
    }

    void FixedUpdate()
    {
        Move(Input.GetAxisRaw("L_XAxis_1"));

        //Now checks if the trigger is active
        if (Input.GetButtonDown("Y_1") && myTrigger.GetIsActive())
        {
            Jump();
            myTrigger.BeatHit();
        }

        //Dash Ability
        if (Input.GetButtonDown("B_1") && myTrigger.GetIsActive())
        {
            //negative on the y to invert stick for some reason
            Dash(Input.GetAxisRaw("L_XAxis_1"), -Input.GetAxisRaw("L_YAxis_1"));
            myTrigger.BeatHit();
        }

        //Alt jump for testing
        if (Input.GetKeyDown(KeyCode.D) && myTrigger.GetIsActive())
        {
            Jump();
            myTrigger.BeatHit();
        }

        //Alt dash for testing
        if (Input.GetKeyDown(KeyCode.S) && myTrigger.GetIsActive())
        {
            Dash(1, 1);
            myTrigger.BeatHit();
        }

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

        Vector2 direction = new Vector2(horizontalInput, 0f);  //probably wrong?
        var goal = direction * speed;
        Vector2 error = new Vector2(goal.x - moveVel.x, 0f);
        mybody.AddForce(acceleration * error);
    }


    public void Dash(float horzontalInput, float verticalInput)
    {
        Vector2 myVector = new Vector2(horzontalInput, verticalInput);
        myVector.Normalize();
        mybody.velocity += dashVelocity *  myVector;
    }

    public void Jump()
    {
        if (isgrounded)
            mybody.velocity += jumpvelocity * Vector2.up;
        
        if (onBeat == true)
        {
            Debug.Log("FOUND THAT BEAT SONNN");
            particles.Play();
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
