using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Trigger : MonoBehaviour
{

    public KeyCode myKey;

    void OnTriggerStay2D(Collider2D c)
    {
        if (Input.GetKeyDown(myKey))
        {
            Destroy(c.gameObject);
        }
    }
}
