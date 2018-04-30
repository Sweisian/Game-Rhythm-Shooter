using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class Script_Beat_Bar : MonoBehaviour {

    private bool isOnBeat = false;

    SpriteRenderer m_SpriteRenderer;

    public Transform beatPrefab;
    public Transform beatSpawnLoc;
    public Transform trigger;

    public float bpm;
    public float beat_rate;

    public GameObject myBeat;

    private float beatSpeed;

    private BeatObserver beatObserver;

    // Use this for initialization
    void Start()
    {
        //beat observation stuff
        beatObserver = GetComponent<BeatObserver>();
        Debug.Log(beatObserver);
        Debug.Log("TRIED TO FIND BEAT OBSERVER)");

        beatSpeed = BeatSpeedCalc();

        //StartCoroutine(RhythmTester());
        //StartCoroutine(BeatGenerator());
    }

    // Update is called once per frame
    void Update()
    {
        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            GameObject thisBeat;
            thisBeat = Instantiate(myBeat, beatSpawnLoc);
            thisBeat.GetComponent<Script_Beat>().moveSpeed = beatSpeed;

            //transform.position = new Vector3(transform.position.x, transform.position.y - .1f, transform.position.z);
            //Debug.Log("Detected on beat");
        }

        //if ((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat)
        //{
        //    transform.position = new Vector3(transform.position.x, transform.position.y + .1f, transform.position.z);
        //}

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

    private float BeatSpeedCalc()
    {
        float spb = 60f / bpm;
        float travelSpeed = Vector3.Distance(beatSpawnLoc.position, trigger.position) / (spb * beat_rate);
        return travelSpeed;
    }
}
