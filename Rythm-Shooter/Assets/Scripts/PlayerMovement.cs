using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float threshold = -20;
    public float speed = 10;
    [SerializeField] private float acceleration = 5f;
    public float jumpvelocity = 20;
    public LayerMask playerMask;
    public bool canmoveinair = true;
    Transform myTransform, tagGround;
    
    bool isgrounded = true;
    private Vector2 pos;
    Vector2 moveVel;
    public float scalingFactor = 0.36f;
    public float minSpeed = 9f;
    public float maxSpeed = 5;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    Rigidbody2D mybody;
    private ParticleSystem particles;
    

    void Awake()
    {
        mybody = GetComponent<Rigidbody2D>();
        //child = GetComponentInChildren()
        particles = GetComponentInChildren<ParticleSystem>();
    }

    // Use this for initialization
    void Start()
    {
        //mybody = GetComponent<Rigidbody2D>();
        myTransform = GetComponent<Transform>();
        pos = myTransform.position;
        tagGround = GameObject.FindGameObjectWithTag("Ground").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isgrounded);
        if (Input.GetButtonDown("LB_1"))
        {
            Jump();
        }

        // try moving to fixed update


        //Respawn();
        //CheckDeath();

        //particles.Play();
    }

    void FixedUpdate()
    {
        Move(Input.GetAxisRaw("L_XAxis_1"));

        
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


    public void Move(float horzontalInput)
    {
        
        moveVel = mybody.velocity;
        
        Vector2 direction = new Vector2(horzontalInput, 0f);  //probably wrong?
        var goal = direction * speed;
        Vector2 error = new Vector2(goal.x - moveVel.x, 0f);
        mybody.AddForce(acceleration * error);
        
        /*
        // janky old code
        moveVel = mybody.velocity;
        Debug.Log(horzontalInput);
        if (Input.GetAxisRaw("L_XAxis_1") == 0)
        {
            // when movement keys are not currently pressed
            moveVel.x = 0f;            
        }

        // else movement keys are pressed	
        else if (Mathf.Abs(moveVel.x) < maxSpeed)
        {
            moveVel.x += horzontalInput * speed;          
        }

        else
        {
            // don't increase speed if maxSpeed has been reached
            moveVel.x = horzontalInput * maxSpeed;           
        }
        mybody.velocity = moveVel;
        */
    }


    public void Jump()
    {
        if (isgrounded)
            mybody.velocity += jumpvelocity * Vector2.up;
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
