using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Boomerang_Bullet : MonoBehaviour
{

     public GameObject player;
     public Transform target;
    public Rigidbody2D rb;
    public bool trackPlayer = true;

    [SerializeField] private Script_GameManager gm;

     public float speed = 33f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float initialSpeed = 80f;
    [SerializeField] private float flyStraightTime = 1f;
    [SerializeField] private float spawnOffset = 10f;

    private AudioManager audioManager;

    // Use this for initialization
    void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<Script_GameManager>();
        if (gm == null)
            //Debug.Log(gameObject + " DIDN'T FIND THE GAME MANAGER");
        rb = GetComponent<Rigidbody2D>();


        //Asks the gm for which player to chase if tag mode is on
        if(gm.tagModeOn)
        {
            player = gm.tagModeInit();
        }
    }

    void Update()
    {
        if (gm.canSwitchTargets && gm.tagModeOn)
        {
            if (gm.chaseP1)
                player = gm.player1;
            else
                player = gm.player2;

            if (player == gm.player1)
                GetComponent<SpriteRenderer>().color = Color.blue;
            else
                GetComponent<SpriteRenderer>().color = Color.yellow;
        }

        audioManager = GameObject.FindObjectOfType<AudioManager>();

    }

    void FixedUpdate()
    {
        //checks if we should be tracking the player or not
        if (trackPlayer)
        {
            //gets the target based on the player, which should be assigned when this is instantiated
            if (player != null)
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
        if (gm.tagModeOn)
        {
            gm.respawn(c.gameObject);
            Destroy(gameObject);
        }
        else if (player.tag == c.gameObject.tag)
        {
            //if the bullet hits the player who shot it
            player.GetComponent<Character_Behavior>().BecomeHuman(player);
        }
        else
        {
            audioManager.PlaySound("hit");


            gm.respawn(c.gameObject);
        }

        //if (c.gameObject.tag == "PlayerTwo" || c.gameObject.tag == "PlayerOne" && c.gameObject.tag != c.gameObject.tag)
        //{
        //    Debug.Log("boomerang hit da other player");
        //    gm.respawn(gameObject);
        //}


        Destroy(gameObject);
        
    }

}
