using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSettings : MonoBehaviour
{
    public string setting;
    public bool useSetting = true;
    private BeatSynchronizer beatSynch;
    private BeatObserver beatObserver;

    [SerializeField] public GameObject beatBar;

    // Use this for initialization
    void Start ()
	{

	    beatSynch = GetComponent<BeatSynchronizer>();
	    beatObserver = beatBar.GetComponent<BeatObserver>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (useSetting)
	    {
	        Debug.Log("startDelay is: ");
	        Debug.Log(beatSynch.startDelay);
	        Debug.Log("beatWindow is: ");
	        Debug.Log(beatObserver.beatWindow);
	        // 0 is default beat setting
	        switch (setting)
	        {
	            case "default":
	                beatSynch.startDelay = 1000f;
	                beatObserver.beatWindow = 300f;
	                break;
                case "easy":
                    beatSynch.startDelay = 1000f;
                    beatObserver.beatWindow = 400f;
                    break;
	            case "tight":
	                beatSynch.startDelay = 1100f;
	                beatObserver.beatWindow = 200f;
	                break;
	            case "early":
	                beatSynch.startDelay = 1050f;
	                beatObserver.beatWindow = 300f;
	                break;
	            case "late":
	                beatSynch.startDelay = 1100f;
	                beatObserver.beatWindow = 300f;
	                break;
	            case "tightEarly":
	                beatSynch.startDelay = 1050f;
	                beatObserver.beatWindow = 200f;
	                break;
	            case "tightLate":
	                beatSynch.startDelay = 1150f;
	                beatObserver.beatWindow = 200f;
	                break;
	            case "tightEarlier":
	                beatSynch.startDelay = 1000f;
	                beatObserver.beatWindow = 200f;
	                break;
	            case "tightLater":
	                beatSynch.startDelay = 1200f;
	                beatObserver.beatWindow = 200f;
	                break;
	            case "tightEarliest":
	                beatSynch.startDelay = 950f;
	                beatObserver.beatWindow = 200f;
	                break;
	            case "tightLatest":
	                beatSynch.startDelay = 1250f;
	                beatObserver.beatWindow = 200f;
	                break;
                case "stupid":
                    beatSynch.startDelay = 500f;
                    beatObserver.beatWindow = 100f;
                    break;
            }
	    }
	}
}
