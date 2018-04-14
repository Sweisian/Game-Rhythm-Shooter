using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Behavior : MonoBehaviour {

    public float shootdelay = 0.1f;
    private float shotready = 0.0f;
    public GameObject shot;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Shoot") < -0.1 && Time.time > shotready)
            Fire();
	}



    void Fire ()
    {
        GameObject shotcreate = Instantiate(shot);
        ShotBehavior shotinit = shotcreate.GetComponent<ShotBehavior>();
        shotinit.Init(this.gameObject, transform.position + (0.2F * transform.right), transform.right);
        shotready = Time.time + shootdelay;
    }
}
