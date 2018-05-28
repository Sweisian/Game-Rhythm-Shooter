using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] AudioSource dashSound;

	// Use this for initialization
	void Start () {
        //PlayCoinSound();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(string input)
    {
        Debug.Log("Play coin sound");
        switch(input){
            case "dash":
                //Debug.Log("Play dash sound");
                dashSound.Play();
                break;
            case "fire":
                dashSound.Play();
                break;
            case "hit":
                dashSound.Play();
                break;
            case "playerCollision":
                dashSound.Play();
                break;
            case "objectCollision":
                dashSound.Play();
                break;
            case "gameOver":
                dashSound.Play();
                break;
            default:
                Debug.Log("invalid input");
            break;
        }
        Debug.Log(input);
            
    
        
        //dashSound.Play();
    }
}
