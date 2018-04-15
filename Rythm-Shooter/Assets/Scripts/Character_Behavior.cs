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
        if (Input.GetAxis("TriggersR_1") < -0.1 && Time.time > shotready)
            Fire();
    }



    void Fire()
    {
        GameObject shotcreate = Instantiate(shot);
        ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();
        shotinit.Init(this.gameObject, transform.position + (0.2F * transform.right), transform.right);
        shotready = Time.time + shootdelay;
    }

    Vector2 Aim()
    {
        Vector2 Temp;
        Temp = new Vector2(0, 0);
        float Y = Input.GetAxis("R_YAxis_1");
        float X = Input.GetAxis("R_XAxis_1");
        float slope = Y / X;
        if (X < 0 && slope < -373.2f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 90f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 90f);
        }
        if (X < 0 && slope > -373.2f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 120f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 120f);
        }
        if (X < 0 && slope > -100f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 150f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 150f);
        }
        if (X < 0 && slope > -26.5f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 180f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 180f);
        }
        if (X > 0 && slope > 373.2f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 90f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 90f);
        }
        if (X > 0 && slope < 373.2f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 60f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 60f);
        }
        if (X > 0 && slope < 100f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 30f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 30f);
        }
        if (X > 0 && slope < 26.5f)
        {
            Temp.x = Mathf.Cos(Mathf.Deg2Rad * 0f);
            Temp.y = Mathf.Sin(Mathf.Deg2Rad * 0f);
        }

        return Temp;
    }


}
