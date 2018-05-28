using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InControl;
using EZCameraShake;

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

    [SerializeField] private GameObject GameManage;
    [SerializeField] private GameObject beatBar;

    //move dis
    private ParticleSystem myDashParticles;

    //Important fields to be editable from the inspector
    [SerializeField] private float jumpvelocity = 20;

    [SerializeField] private float speed = 10;
    [SerializeField] private float shootdelay = 0.1f;
    [SerializeField] private float shotready = 0.0f;
    [SerializeField] private bool forceOnBeat = true;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float stunLength = .5f;
    [Range(0, 1)] [SerializeField] private int playerNumber = 0;

    //Things we don't care about seeing in the inpsector
    private Vector2 LastAim;

    private bool isgrounded = true;
    private bool jump = false;
    private bool dash = false;
    private bool facingRight = true;
    private bool onBeat = false;


    //Ghost state testing
    [SerializeField] private bool isGhost = false;

    [SerializeField] private bool isGhostEnabled = false;
    private bool stuckAsHuman = false;
    Color32 humanColor; // = new Color(0.5279903f, 0.990566f, 0.5664769f, 1f);
    Color32 ghostColor = new Color(0.1884567f, 0.3301887f, 0.1992668f, 1f);


    //[SerializeField] AudioClip coinEffect;
    private AudioManager audioManager;


    //Public variables
    public bool isStunned = false;

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
        humanColor = mySpriteRen.color;

        //added code to find the trigger object and script
        myTrigger = GameObject.FindGameObjectWithTag("Trigger").GetComponent<Script_Trigger>();

        mybody = GetComponent<Rigidbody2D>();
        particles = GetComponentsInChildren<ParticleSystem>();
        beatObserver = GetComponent<BeatObserver>();

        myDashMove = GetComponent<Script_DashMove>();

        //finds child dash particles with name "Dash Particles"
        myDashParticles = transform.Find("Dash Particles").gameObject.GetComponent<ParticleSystem>();
        if (myDashParticles == null)
            Debug.Log("No Dash Particles child attached");

        if (myTrigger == null)
            throw new Exception("a Trigger Object was not found");

        audioManager = GameObject.FindObjectOfType<AudioManager>();


    }

    // Update is called once per frame
    void Update()
    {
        //audiosource.PlayOneShot(coinEffect);
        

        //checks to see if the player is stunned
        if (!isStunned)
        {
            InputDevice player = InputManager.Devices[playerNumber];
            if (isGhostEnabled)
            {
                if (isGhost)
                {
                    //dash
                    if (player.Action1.WasPressed || Input.GetKeyDown(KeyCode.L))
                    {
                        //Debug.Log("X Pressed by player 1");
                        dash = true;
                    }
                }
                else if (!isGhost && !stuckAsHuman)
                {
                    //shoot
                    if (player.Action3.WasPressed || Input.GetKeyDown(KeyCode.J))
                    {
                        Fire(this.gameObject, player);
                        particles[0].Play();
                    }
                }
            }
            else
            {
                //dash
                if (player.Action1.WasPressed || Input.GetKeyDown(KeyCode.L))
                {
                    //Debug.Log("X Pressed by player 1");
                    dash = true;
                }
                //shoot
                if (player.Action3.WasPressed || Input.GetKeyDown(KeyCode.J))
                {
                    Fire(this.gameObject, player);
                    particles[0].Play();
                }
            }

            //Handles enableing particle emmision on dash
            if (myDashMove.direction != 0)
            {
                var emission = myDashParticles.emission;
                emission.enabled = true;
            }
            else if (myDashMove.direction == 0)
            {
                var emission = myDashParticles.emission;
                emission.enabled = false;
            }
        }

        //Handles enableing particle emmision on dash
        if (myDashMove.direction != 0)
        {
            var emission = myDashParticles.emission;
            emission.enabled = true;
            //Debug.Log("Currently Dashing");
        }
        else if (myDashMove.direction == 0)
        {
            //Debug.Log("not dashing");
            var emission = myDashParticles.emission;
            emission.enabled = false;
        }

        //makes sure the player is the right color
        //if (myDashMove.direction != 0)
        //{
        if (isGhost)
        {
            mySpriteRen.color = ghostColor;
        }
        else if (!isGhost)
        {
            mySpriteRen.color = humanColor;
        }
        //}


    }

    void FixedUpdate()
    {
        InputDevice player = InputManager.Devices[playerNumber];
        InputControl xControl = player.GetControl(InputControlType.LeftStickX);
        InputControl yControl = player.GetControl(InputControlType.LeftStickY);

        myAnim.SetFloat("moveSpeed", Mathf.Abs(xControl.Value));

        //Sets orientation of sprite
        if (xControl.Value > .01f || Input.GetKey(KeyCode.D))
            facingRight = true;

        if (xControl.Value < -.01f || Input.GetKey(KeyCode.A))
            facingRight = false;

        if (facingRight)
            transform.localScale = new Vector2(1, transform.localScale.y);
        if (!facingRight)
            transform.localScale = new Vector2(-1, transform.localScale.y);

        FallingPhysics(mybody);

        //Dash Ability
        if (dash)
        {
            audioManager.PlaySound("dash");

            Dash(xControl, myDashMove);
            //Debug.Log("I DASHED");
            dash = false;
        }

        //Might be important this stays at the end
        Move(xControl.Value, mybody, myDashMove);
    }

    public void Dash(InputControl xControl, Script_DashMove myDashMove)
    {
        //camera shake function
        Debug.Log("Called Dash");
        CameraShaker.Instance.ShakeOnce(5f, 3f, 0f, .5f);

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
        else if (xControl.Value == 0)
        {
            if (facingRight)
                myDashMove.direction = 2;
            if (!facingRight)
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
        if (Input.GetKey(KeyCode.A))
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
        audioManager.PlaySound("fire");
        //StartCoroutine(ChillAsGhost(currGameObject, player));
        BecomeGhost(currGameObject);

        Vector2 Temp = new Vector2();
        InputControl aimX = player.GetControl(InputControlType.LeftStickX);
        InputControl aimY = player.GetControl(InputControlType.LeftStickY);
        Aim(currGameObject, player);
        if (currGameObject.GetComponent<Character_Behavior2>() != null)
            Temp = currGameObject.GetComponent<Character_Behavior2>().LastAim;
        else
        {
            Temp = LastAim;
        }



        //old code for regular bullets
        //GameObject shotcreate = Instantiate(shot);
        //ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();
        //shotinit.Init(currGameObject, (Vector2)currGameObject.transform.position + (1.5F * (Temp)), Temp);

        //new code for boomerang bullets

        GameObject shotcreate = Instantiate(shot); //This might possibly spawn the boomerang inside
        Script_Boomerang_Bullet myBoomBulletScript = shotcreate.GetComponent<Script_Boomerang_Bullet>();
        shotready = Time.time + shootdelay;

        //initilizes shot parameters
        myBoomBulletScript.shotInit(facingRight, gameObject);
    }

    public void BecomeGhost(GameObject curr)
    {
        if (!stuckAsHuman)
        {
            this.isGhost = true;
            Color32 ghostColor = new Color(0.1884567f, 0.3301887f, 0.1992668f, 1f);
            mySpriteRen.color = ghostColor;
            Debug.Log("ITS SPOOKY TIME");
        }
        else
        {
            Debug.Log("You must be a human for a time, ye mortal...");
        }

    }

    public void BecomeHuman(GameObject curr)
    {
        mySpriteRen.color = humanColor;
        this.isGhost = false;
        Debug.Log("JUST A HUMAN...");
        StartCoroutine(RemainAHuman(curr));
    }


    IEnumerator RemainAHuman(GameObject curr)
    {
        Debug.Log("stay mortal for a time");
        stuckAsHuman = true;
        particles[4].Play();
        yield return new WaitForSeconds(2f);
        stuckAsHuman = false;
    }


    void Aim(GameObject curr, InputDevice player)
    {
        //InputDevice player = InputManager.Devices[playerNumber];
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

    void OnCollisionEnter2D(Collision2D other)
    {       
        //checks to see if we are dashing and hit another player
        if (myDashMove.direction != 0 && other.gameObject.tag == "PlayerOne" || other.gameObject.tag == "PlayerTwo")
        {
            audioManager.PlaySound("playerCollision");

            Debug.Log("I dashed into: " + other);
            StartCoroutine(stunned(other));
        }
    }

    IEnumerator stunned(Collision2D other)
    {
        Color32 c = other.gameObject.GetComponent<SpriteRenderer>().color;
        isStunned = true;
        other.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
        yield return new WaitForSeconds(stunLength);
        other.gameObject.GetComponent<SpriteRenderer>().color = c;
        isStunned = false;
    }

    IEnumerator dashFlash()
    {
        //Debug.Log("called dashFlash");
        //Debug.Log(myDashMove.direction);

        Color32 c = mySpriteRen.color;
        while (myDashMove.direction != 0)
        {
            //Debug.Log("inside dashFlash");
            mySpriteRen.color = humanColor;
            yield return new WaitForSeconds(0.03f);
            mySpriteRen.color = c;
            yield return new WaitForSeconds(0.03f);
        }
        mySpriteRen.color = c;
    }
}
