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

    void Fire()
    {
        Vector2 Temp = Aim();
        GameObject shotcreate = Instantiate(shot);
        ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();
        shotinit.Init(this.gameObject, (Vector2)transform.position + (0.2F * (Temp)), Temp);
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
        if (collision.gameObject.tag == "shot")
        {
            this.GetComponentInParent<Script_GameManager>().respawn(this.gameObject);
            Destroy(collision.gameObject);
        }
    }


}
