using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_GameManager : MonoBehaviour
{

    private float respawntime = 0.2f;

    public GameObject player1;
    public float score1;
    public float player1respawn;
    public GameObject player1score;

    public GameObject player2;
    public float score2;
    public float player2respawn;
    public GameObject player2score;

    private bool isOnBeat = false;

    public float offSet;

    public float onTime;
    public float offTime;

    SpriteRenderer m_SpriteRenderer;

    public GameObject testing;

	// Use this for initialization
	void Start ()
	{
	    StartCoroutine(Rhythm());
	    m_SpriteRenderer = testing.GetComponent<SpriteRenderer>();
	}
	
    public void respawn (GameObject caller)
    {
        Debug.Log("respawn");
        if (caller.GetComponent<Character_Behavior>() != null)
        {
            caller.transform.position = new Vector2(Random.Range(-5, 5), Random.Range(-1, 1));
            score2 += 1;
            player1score.GetComponent<Text>().text = score1.ToString();
        }
        if (caller.GetComponent<Character_Behavior2>() != null)
        {
            caller.transform.position = new Vector2(Random.Range(-5, 5), Random.Range(-1, 1));
            score1 += 1;
            player2score.GetComponent<Text>().text = score2.ToString();
        }
    }

    IEnumerator Rhythm()
    {
        yield return new WaitForSeconds(offSet);

        while (true)
        {
            isOnBeat = true;
            yield return new WaitForSecondsRealtime(onTime);
            isOnBeat = false;
            yield return new WaitForSecondsRealtime(offTime);
        }
    }
}
