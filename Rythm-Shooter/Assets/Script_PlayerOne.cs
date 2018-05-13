using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using InControl;

public class Script_PlayerOne : Character_Behavior
{
    void Awake()
    {
        beatBarScript = beatBar.GetComponent<Script_Beat_Bar>();
        //cpurrGameObject = this.gameObject;
        player = InputManager.Devices[playerNumber];

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
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetPlayerNumber()
    {

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
}
