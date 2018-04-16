using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Trigger : MonoBehaviour
{
    private bool isActive = false;

    private GameObject beat;

    public KeyCode myKey;

    void Update()
    {
        if (Input.GetKeyDown(myKey) && isActive)
        {
            beat.GetComponent<SpriteRenderer>().color = Color.green;
        }
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
