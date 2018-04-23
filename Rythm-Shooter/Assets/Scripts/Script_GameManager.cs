using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_GameManager : MonoBehaviour
{

    private float respawntime = 0.2f;

    public GameObject player1;
    public float score1;
    public float player1respawn;

    public GameObject player2;
    public float score2;
    public float player2respawn;

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
        if (caller == player1)
        {
            player1.transform.position = new Vector2(Random.Range(-3, 3), Random.Range(-1, 1));
            score1 += 1;
        }
        if (caller == player2)
        {
            player2.transform.position = new Vector2(Random.Range(-3, 3), Random.Range(-1, 1));
            score2 += 1;
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
