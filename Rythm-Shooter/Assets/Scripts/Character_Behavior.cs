using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Behavior : MonoBehaviour
{

    public float shootdelay = 0.1f;
    private float shotready = 0.0f;
    public GameObject shot;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("TriggersR_1") < -0.1 && Time.time > shotready)
            Fire();
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
        float Y = Input.GetAxisRaw("R_YAxis_1");
        float X = Input.GetAxisRaw("R_XAxis_1");
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


}
