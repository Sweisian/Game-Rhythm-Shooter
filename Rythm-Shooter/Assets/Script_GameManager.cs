using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_GameManager : MonoBehaviour
{

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
