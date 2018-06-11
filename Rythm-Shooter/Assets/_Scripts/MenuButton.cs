using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour {

    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject PlayScene;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void StartGame()
    {

        SceneManager.LoadScene("Tag 6_4 Swei");
    }

    void Options()
    {
        

        Application.Quit();
        //SceneManager.LoadScene("Options_Menu");
    }

    void OptionReturn()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    void GoToStart()
    {
        SceneManager.LoadScene("Main_Menu");
    }
    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
