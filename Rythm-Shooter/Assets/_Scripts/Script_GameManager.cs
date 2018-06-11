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
    public Canvas winCanvas;

    public int scoreCap = 5;


    public GameObject[] shots;

    public bool tagModeOn;

    public bool chaseP1;

    public GameObject tagBoomerang;
    public Transform tagRespawn;
    public float tagRefractoryPeriod = 1;

    public bool canSwitchTargets = true;

    private AudioManager audioManager;

    private Color myLightBlue;
    private Color myLightOrange;

    // Use this for initialization
    void Start ()
	{
        //DontDestroyOnLoad(gameObject);
        //ColorUtility.TryParseHtmlString("97E9FF", out myLightBlue);
        //ColorUtility.TryParseHtmlString("FFAB56", out myLightOrange);

        myLightBlue = new Color(0.5882353f, 0.9137255f, 1f);
        myLightOrange = new Color(1f, 0.670058f, 0.3333334f);

        Debug.Log(myLightBlue);
        Debug.Log(myLightOrange);

        player1 = GameObject.FindGameObjectWithTag("PlayerOne");
        player2 = GameObject.FindGameObjectWithTag("PlayerTwo");

        player1score = GameObject.Find("Player One Score").GetComponent<TextMeshProUGUI>();
        player2score = GameObject.Find("Player Two Score").GetComponent<TextMeshProUGUI>();
        gameOverText = GameObject.Find("Game Over Text").GetComponent<TextMeshProUGUI>();

        gameOverText.text = "";

	    audioManager = GameObject.FindObjectOfType<AudioManager>();

    }

    //temp testing code
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Scene loadedLevel = SceneManager.GetActiveScene();
            SceneManager.LoadScene(loadedLevel.buildIndex);
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
            player1.GetComponent<Character_Behavior>().myAnim.SetBool("Win",true);
            player2.GetComponent<Character_Behavior>().myAnim.SetBool("Dead", true);
            gameOverText.text = "Game Over. Blue Wins!";
            gameOverText.color = myLightBlue;
            winCanvas.gameObject.SetActive(true);

            StartCoroutine(gameOverRoutine());
        }
        if (score2 >= scoreCap)
        {
            player2.GetComponent<Character_Behavior>().myAnim.SetBool("Win", true);
            player1.GetComponent<Character_Behavior>().myAnim.SetBool("Dead", true);
            gameOverText.text = "Game Over. Orange Wins!";
            gameOverText.color = myLightOrange;
            winCanvas.gameObject.SetActive(true);

            StartCoroutine(gameOverRoutine());
        }
    }
	
    //gets called by the bullet when it hits something
    public void respawn (GameObject caller)
    {

        player1.transform.position = playerOneRespawn.position;
        player2.transform.position = playerTwoRespawn.position;

        //resets player state to be human
        player1.GetComponent<Character_Behavior>().BecomeHuman(player1);
        player2.GetComponent<Character_Behavior>().BecomeHuman(player2);

        audioManager.PlaySound("respawn");

        //Debug.Log("respawn");
        if (caller.tag == "PlayerOne")
        {
            score2 += 1;
            player1.GetComponent<Character_Behavior>().myAnim.SetTrigger("Hit");
            player2score.text = "Victories: " + score2.ToString();

            if (score1 < scoreCap && score2 < scoreCap)
                StartCoroutine("orangeWinsRoutine");
        }

        if (caller.tag == "PlayerTwo")
        {
            score1 += 1;
            player2.GetComponent<Character_Behavior>().myAnim.SetTrigger("Hit");
            player1score.text = "Victories: " + score1.ToString();

            if (score1 < scoreCap && score2 < scoreCap)
                StartCoroutine("blueWinsRoutine");
        }

        if(tagModeOn)
        {
            GameObject newTagBoomerang = Instantiate(tagBoomerang);
            newTagBoomerang.transform.position = tagRespawn.position;
        }


        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.name);

        //shots = GameObject.FindGameObjectsWithTag("shot");
        //foreach(GameObject s in shots)
        //{
        //    Destroy(s);
        //}
    }

    public GameObject tagModeInit()
    {
        //If this is a new game
        if(score1 + score2 == 0)
        {
            if (Random.value > .50)
                chaseP1 = true;
            else
                chaseP1 = false;
        }

        if (chaseP1)
        {
            chaseP1 = !chaseP1;
            return player1;
        }
        else
        {
            chaseP1 = !chaseP1;
            return player2;
        }

    }

    IEnumerator orangeWinsRoutine()
    {
        //Debug.Log("I fucking reached the orange wins routine you cocksucker");

        gameOverText.text = "Point: Orange";
        gameOverText.color = myLightOrange;

        CameraShaker.Instance.ShakeOnce(10f, 10f, 0f, 1f);

        yield return new WaitForSecondsRealtime(2);
        gameOverText.color = Color.white;
        gameOverText.text = "FIGHT!";
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1);
        gameOverText.text = "";

    }

    IEnumerator blueWinsRoutine()
    {
        //Debug.Log("I fucking reached the blue wins routine you cocksucker");

        gameOverText.text = "Point: Blue";
        gameOverText.color = myLightBlue;

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
        audioManager.PlaySound("gameOver");
        Time.timeScale = .25f;
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1f;
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);

        //Doesn't seem to work but I don't care
        GameObject[] shots = GameObject.FindGameObjectsWithTag("Shot");
        foreach (GameObject s in shots)
        {
            s.GetComponent<Script_Boomerang_Bullet>().speed = 0;
        }
    }

    public IEnumerator tagRefractoryRoutine()
    {
        canSwitchTargets = false;
        yield return new WaitForSecondsRealtime(tagRefractoryPeriod) ;
        canSwitchTargets = true;
    }
}
