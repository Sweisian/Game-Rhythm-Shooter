using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Boomerang_Bullet : MonoBehaviour
{

     public GameObject player;
     public Transform target;
    public Rigidbody2D rb;
    public bool trackPlayer = true;

    [SerializeField] private float speed = 33f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float initialSpeed = 80f;
    [SerializeField] private float flyStraightTime = 1f;
    [SerializeField] private float spawnOffset = 10f;

    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //checks if we should be tracking the player or not
        if (trackPlayer)
        {
            //gets the target based on the player, which should be assigned when this is instantiated
            if (target == null && player != null)
                target = player.transform;

            if (target != null && trackPlayer)
            {
                Vector2 direction = (Vector2)target.position - rb.position;

                direction.Normalize();

                float rotateAmount = Vector3.Cross(direction, transform.up).z;

                rb.angularVelocity = -rotateAmount * rotateSpeed;

                rb.velocity = transform.up * speed;
            }
        }

    }

    public void shotInit(bool facingRight, GameObject parent)
    {
        player = parent;

        if (facingRight)
            transform.position = parent.transform.position + new Vector3(spawnOffset, 0, 0);
        if (!facingRight)
            transform.position = parent.transform.position + new Vector3(-spawnOffset, 0, 0);

        StartCoroutine(FlyStraight(facingRight));
    }

    public IEnumerator FlyStraight(bool facingRight)
    {
        trackPlayer = false;
        if(facingRight)
            rb.velocity = new Vector2(initialSpeed, 0);
        if (!facingRight)
            rb.velocity = new Vector2(-initialSpeed, 0);
        yield return new WaitForSeconds(flyStraightTime);
        trackPlayer = true;
    }


    void OnCollisionEnter2D(Collision2D c)
    {
       if(c.gameObject.tag == "PlayerOne")
        {
            Destroy(gameObject);
        }
    }


    

}
