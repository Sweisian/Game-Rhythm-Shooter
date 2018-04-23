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
        if (Input.GetKeyDown(myKey) && isActive)
        {
            beat.GetComponent<SpriteRenderer>().color = Color.green;
            isActive = false;
        }
    }

    public void BeatHit()
    {
        beat.GetComponent<SpriteRenderer>().color = Color.green;
        isActive = false;
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        isActive = true;
        beat = c.gameObject;
    }

    void OnTriggerExit2D(Collider2D c)
    {
        isActive = false;
    }
}
