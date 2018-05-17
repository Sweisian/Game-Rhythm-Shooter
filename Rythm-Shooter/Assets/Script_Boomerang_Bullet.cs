using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Boomerang_Bullet : MonoBehaviour
{

     public GameObject player;
     public Transform target;
    public Rigidbody2D rb;

    [SerializeField] private float speed = 33f;
    [SerializeField] private float rotateSpeed = 200f;

    

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //gets the target based on the player, which should be assigned when this is instantiated
        if(target == null && player != null)
            target = player.transform;

        if (target != null)
        {
            Vector2 direction = (Vector2)target.position - rb.position;

            direction.Normalize();

            float rotateAmount = Vector3.Cross(direction, transform.up).z;

            rb.angularVelocity = -rotateAmount * rotateSpeed;

            rb.velocity = transform.up * speed;
        }

    }


    void OnCollisionEnter2D(Collision2D c)
    {
       
    }


    

}
