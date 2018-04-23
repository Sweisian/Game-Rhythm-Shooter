using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_rhythm : MonoBehaviour {

    private bool isOnBeat = false;

    public float beatGenOffSet;

    public float onTime;
    public float offTime;

    SpriteRenderer m_SpriteRenderer;

    public GameObject testing;

    public Transform beatPrefab;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(RhythmTester());
        StartCoroutine(BeatGenerator());
        m_SpriteRenderer = testing.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        BeatTester();
    }

    void BeatTester()
    {
        if (isOnBeat)
        {
            //Debug.Log("on beat");
            //m_SpriteRenderer.color = Color.red;
        }
        else
        {
            //Debug.Log("off beat");
           // m_SpriteRenderer.color = Color.green;
        }

    }

    IEnumerator BeatGenerator()
    {
        yield return new WaitForSecondsRealtime(beatGenOffSet);

        while (true)
        {
            Instantiate(beatPrefab);
            yield return new WaitForSecondsRealtime(offTime + onTime);
        }
    }

    IEnumerator RhythmTester()
    {
        while (true)
        {
            isOnBeat = true;
            yield return new WaitForSecondsRealtime(onTime);
            isOnBeat = false;
            yield return new WaitForSecondsRealtime(offTime);
        }
    }
}
