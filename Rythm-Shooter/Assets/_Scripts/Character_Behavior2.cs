using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InControl;

public class Character_Behavior2 : MonoBehaviour
{
    public float shootdelay = 0.1f;
    private float shotready = 0.0f;
    public GameObject shot;
    public GameObject GameManage;

    private Script_Trigger2 myTrigger;
    public Vector2 LastAim;

    bool isgrounded = true;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    Rigidbody2D mybody;
    private ParticleSystem[] particles;

    private BeatObserver beatObserver;

    private bool onBeat = false;

    private Animator myAnim;
    private SpriteRenderer mySpriteRen;

    private bool jump = false;
    private bool dash = false;

    private Character_Behavior charB;
    private GameObject currGameObject;
    private InputDevice player;

    private Script_DashMove myDashMove;

    [SerializeField] public GameObject beatBar;
    private Script_Beat_Bar beatBarScript;

    public Boolean facingRight = true;

    public bool forceOnBeat = true;

    // Use this for initialization
    void Start()
    {
        beatBarScript = beatBar.GetComponent<Script_Beat_Bar>();

        charB = GameObject.FindGameObjectWithTag("PlayerOne").GetComponent<Character_Behavior>();
        if (charB == null)
            Debug.Log("CharacterBehavior not found");
        currGameObject = this.gameObject;
        player = InputManager.Devices[1];
        myDashMove = GetComponent<Script_DashMove>();

        myAnim = GetComponent<Animator>();
        mySpriteRen = GetComponent<SpriteRenderer>();

        //added code to find the trigger object and script
        myTrigger = GameObject.FindGameObjectWithTag("Trigger").GetComponent<Script_Trigger2>();

        mybody = GetComponent<Rigidbody2D>();
        particles = GetComponentsInChildren<ParticleSystem>();
        beatObserver = GetComponent<BeatObserver>();

        if (myTrigger == null)
            throw new Exception("a Trigger Object was not found");
    }

    // Update is called once per frame
    void Update()
    {

        InputDevice player = InputManager.Devices[1];

        if (player.Action1.WasPressed)
        {
            if (forceOnBeat)
            {
                Debug.Log("A Pressed player 2");
                if (beatBarScript.onBeat)
                    //if (myTrigger.GetIsActive())
                {
                    charB.Fire(this.gameObject, player);
                    particles[0].Play();
                    myTrigger.BeatHit();
                }
            }
            else
            {
                charB.Fire(this.gameObject, player);
                particles[0].Play();
            }
        }

        //jump
        if (player.Action2.WasPressed && isgrounded)
        {
            Debug.Log("B Pressed player 2");
            jump = true;
        }

        //dash
        if (player.Action3.WasPressed)
        {
            Debug.Log("X Pressed by player 2");
            dash = true;
        }
        
        /*
        if (player.Action1.WasPressed)
        {
            Debug.Log("A Pressed");
            //if (myTrigger.GetIsActive())
            if (beatBarScript.onBeat)
            {
                charB.Fire(this.gameObject, player);
                particles[0].Play();
                myTrigger.BeatHit();
            }
        }

        //jump
        if (player.Action2.WasPressed && isgrounded)
        {
            //Debug.Log("B Pressed");
            jump = true;
        }
        */

    }

    void FixedUpdate()
    {
        InputDevice player = InputManager.Devices[1];
        InputControl movecontrol = player.GetControl(InputControlType.LeftStickX);
        InputControl aimcontrol = player.GetControl(InputControlType.LeftStickY);
        charB.Move(movecontrol.Value, mybody, myDashMove);

        myAnim.SetFloat("moveSpeed", Mathf.Abs(movecontrol.Value));

        //Sets orientation of sprite
        if (movecontrol.Value > .01f)
            facingRight = true;

        if (movecontrol.Value < -.01f)
            facingRight = false;

        if (facingRight)
            transform.localScale = new Vector2(1, transform.localScale.y);
        if (!facingRight)
            transform.localScale = new Vector2(-1, transform.localScale.y);

        charB.FallingPhysics(mybody);

        //Jump Ability        
        if (jump)
        {
            //Now checks if the trigger is active
            if (forceOnBeat)
            {
                //if (myTrigger.GetIsActive())
                if (beatBarScript.onBeat)
                {
                    charB.Jump(mybody);
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
                charB.Jump(mybody);
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
                    charB.Dash(movecontrol, myDashMove);
                    myTrigger.BeatHit();
                    particles[0].Play();
                }
                else particles[1].Play();
            }
            else
            {
                charB.Dash(movecontrol, myDashMove);
                //Debug.Log("I DASHED");
            }
            dash = false;
        }

    }
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<ShotBehavior>() != null)
        {
            GameManage.GetComponent<Script_GameManager>().respawn(this.gameObject);
            Destroy(collision.gameObject);
        }
    }

    // Detect continous collision with the ground'
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
