using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class Script_Beat_Bar : MonoBehaviour {

    private bool isOnBeat = false;

    public float beatGenOffSet;

    public float onTime;
    public float offTime;

    SpriteRenderer m_SpriteRenderer;

    public GameObject testing;

    public Transform beatPrefab;
    public Transform beatSpawnLoc;

    private BeatObserver beatObserver;

    // Use this for initialization
    void Start()
    {
        //beat observation stuff
        beatObserver = GetComponent<BeatObserver>();
        Debug.Log(beatObserver);
        Debug.Log("TRIED TO FIND BEAT OBSERVER)");

        //StartCoroutine(RhythmTester());
        //StartCoroutine(BeatGenerator());
        m_SpriteRenderer = testing.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - .1f, transform.position.z);
            Debug.Log("Detected on beat");
        }

        if ((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);
        }

        //if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat)
        //{
        //    transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        //}

        //if ((beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat)
        //{
        //    transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        //}





        //BeatTester();
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
            Instantiate(beatPrefab, beatSpawnLoc);
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
