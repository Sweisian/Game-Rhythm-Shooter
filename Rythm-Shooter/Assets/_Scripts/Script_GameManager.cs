using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Script_GameManager : MonoBehaviour
{

    private float respawntime = 0.2f;

    public GameObject player1;
    public float score1;
    public GameObject player1score;

    public GameObject player2;
    public float score2;
    public GameObject player2score;

    public Transform playerOneRespawn;
    public Transform playerTwoRespawn;

    // Use this for initialization
    void Start ()
	{
        DontDestroyOnLoad(gameObject);
        player1 = GameObject.FindGameObjectWithTag("PlayerOne");
        player2 = GameObject.FindGameObjectWithTag("PlayerTwo");
    }

    //temp testing code
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            respawn(player1);
        }
    }
	
    public void respawn (GameObject caller)
    {
        //Debug.Log("respawn");
        if (caller.GetComponent<Character_Behavior>() != null)
        {
            score2 += 1;
            player1score.GetComponent<Text>().text = score1.ToString();

            player1.transform.position = playerOneRespawn.position;
            player2.transform.position = playerTwoRespawn.position;
        }
        if (caller.GetComponent<Character_Behavior2>() != null)
        {
            score1 += 1;
            player2score.GetComponent<Text>().text = score2.ToString();

            player1.transform.position = playerOneRespawn.position;
            player2.transform.position = playerTwoRespawn.position;
        }

        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.name);
    }

    //IEnumerator Rhythm()
    //{
    //    yield return new WaitForSeconds(offSet);

    //    while (true)
    //    {
    //        isOnBeat = true;
    //        yield return new WaitForSecondsRealtime(onTime);
    //        isOnBeat = false;
    //        yield return new WaitForSecondsRealtime(offTime);
    //    }
    //}
}
