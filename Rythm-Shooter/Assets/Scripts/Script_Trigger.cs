using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Trigger : MonoBehaviour
{
    private bool isActive = false;

    private GameObject beat;

    public KeyCode myKey;

    //Uneeded code
    void Update()
    {
        //Temp code for testing
        //if (Input.GetKeyDown(myKey) && isActive)
        //{
        //    beat.GetComponent<SpriteRenderer>().color = Color.green;
        //    isActive = false;
        //}
    }
    public void BeatHit(bool localIsActive)
    //public void BeatHit()
    {
        beat.GetComponent<SpriteRenderer>().color = Color.green;
        localIsActive = false;
    }

    public void BeatHit()
    {
        beat.GetComponent<SpriteRenderer>().color = Color.green;
    }


    public bool GetIsActive()
    {
        return isActive;
    }
    

    void OnTriggerEnter2D(Collider2D c)
    {
        isActive = true;
        beat = c.gameObject;

        //Debug.Log("Beat entered trigger");
    }

    void OnTriggerExit2D(Collider2D c)
    {
        isActive = false;

        //Debug.Log("Beat exited trigger");
    }
}
