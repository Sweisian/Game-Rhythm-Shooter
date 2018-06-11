using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] AudioSource dashSound;
    [SerializeField] AudioSource shootSound;
    [SerializeField] AudioSource playerCollisionSound;
    [SerializeField] AudioSource hitSound;
    [SerializeField] AudioSource respawnSound;
    [SerializeField] AudioSource objectCollisionSound;
    [SerializeField] AudioSource gameOverSound;
    [SerializeField] AudioSource gameStartSound;

    // Use this for initialization
    void Start () {
        //PlayCoinSound();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlaySound(string input)
    {
        switch(input){
            case "dash":
                //Debug.Log("Play dash sound");
                dashSound.Play();
                break;
            case "shoot":
                shootSound.Play();
                break;
            case "hit":
                hitSound.Play();
                break;
            case "playerCollision":
                playerCollisionSound.Play();
                break;
            case "objectCollision":
                objectCollisionSound.Play();
                break;
            case "gameOver":
                gameOverSound.Play();
                break;
            case "gameStart":
                gameStartSound.Play();
                break;
            case "respawn":
                respawnSound.Play();
                break;
            default:
                Debug.Log("invalid input");
                break;
        }
            
    
        
        //dashSound.Play();
    }
}
