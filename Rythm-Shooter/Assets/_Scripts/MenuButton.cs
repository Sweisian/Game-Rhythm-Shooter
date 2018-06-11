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
        SceneManager.LoadScene("Map_Menu");
    }

    void Options()
    {
        SceneManager.LoadScene("Options_Menu");
    }

    void OptionReturn()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    void GoToStart()
    {
        SceneManager.LoadScene("Main_Menu");
    }
    void QuitGame()
    {
        Application.Quit();
    }
}
