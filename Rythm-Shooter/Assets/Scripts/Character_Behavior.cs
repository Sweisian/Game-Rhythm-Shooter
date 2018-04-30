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

    // Use this for initialization
    void Start()
    {
        //added code to find the trigger object and script
        myTrigger = GameObject.FindGameObjectWithTag("Trigger").GetComponent<Script_Trigger>();

        if(myTrigger == null)
            throw new Exception("a Trigger Object was not found");
    }

    // Update is called once per frame
    void Update()
    {

        InputDevice player = InputManager.ActiveDevice;

        //temp code to test the trigger functionality
        if (myTrigger.GetIsActive() && Input.GetKeyDown(KeyCode.F))
        {
            Fire();
            myTrigger.BeatHit();
        }

        //if (Input.GetAxisRaw("TriggersR_1") < -0.1 && Time.time > shotready)
        //    Fire();

        // Uncomment this when ready
        if (player.Action1 && myTrigger.GetIsActive())
        {
            Fire();
            myTrigger.BeatHit();
        }
    }

    void Fire()
    {
        InputDevice player = InputManager.ActiveDevice;
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
        InputDevice player = InputManager.ActiveDevice;
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


}
