using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using EZCameraShake;

public class Script_GameManager : MonoBehaviour
{
    private float respawntime = 0.2f;

    public GameObject player1;
    public float score1;
    public TextMeshProUGUI player1score;

    public GameObject player2;
    public float score2;
    public TextMeshProUGUI player2score;

    public Transform playerOneRespawn;
    public Transform playerTwoRespawn;

    public TextMeshProUGUI gameOverText;

    public int scoreCap = 5;

    // Use this for initialization
    void Start ()
	{
        //DontDestroyOnLoad(gameObject);

        player1 = GameObject.FindGameObjectWithTag("PlayerOne");
        player2 = GameObject.FindGameObjectWithTag("PlayerTwo");

        player1score = GameObject.Find("Player One Score").GetComponent<TextMeshProUGUI>();
        player2score = GameObject.Find("Player Two Score").GetComponent<TextMeshProUGUI>();
        gameOverText = GameObject.Find("Game Over Text").GetComponent<TextMeshProUGUI>();

        gameOverText.text = "";
    }

    //temp testing code
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            respawn(player1);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            respawn(player2);
        }

        if (score1 >= scoreCap)
        {
            gameOverText.text = "Game Over. Green Wins!";
            gameOverText.color = Color.green;
            StartCoroutine("gameOverRoutine");
        }
        if (score2 >= scoreCap)
        {
            gameOverText.text = "Game Over. Red Wins!";
            gameOverText.color = Color.red;
            StartCoroutine("gameOverRoutine");
        }
    }
	
    public void respawn (GameObject caller)
    {
        //Debug.Log("respawn");
        if (caller.tag == "PlayerOne")
        {
            score2 += 1;
            player2score.text = "Victories: " + score2.ToString();

   
            StartCoroutine("redWinsRoutine");

            player1.transform.position = playerOneRespawn.position;
            player2.transform.position = playerTwoRespawn.position;

        }

        if (caller.tag == "PlayerTwo")
        {
            score1 += 1;
            player1score.text = "Victories: " + score1.ToString();

            StartCoroutine("greenWinsRoutine");

            player1.transform.position = playerOneRespawn.position;
            player2.transform.position = playerTwoRespawn.position;
        }

        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.name);
    }

    IEnumerator redWinsRoutine()
    {
        gameOverText.text = "Point: Red";
        gameOverText.color = Color.red;

        CameraShaker.Instance.ShakeOnce(10f, 10f, 0f, 1f);

        yield return new WaitForSecondsRealtime(2);
        gameOverText.color = Color.white;
        gameOverText.text = "FIGHT!";
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1);
        gameOverText.text = "";

    }

    IEnumerator greenWinsRoutine()
    {
        gameOverText.text = "Point: Green";
        gameOverText.color = Color.green;

        CameraShaker.Instance.ShakeOnce(10f, 10f, 0f, 1f);

        yield return new WaitForSecondsRealtime(2);
        gameOverText.color = Color.white;
        gameOverText.text = "FIGHT!";
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1);
        gameOverText.text = "";
    }

    IEnumerator gameOverRoutine()
    {
     
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1f;
        Application.LoadLevel(Application.loadedLevel);
    }
}
