using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour {

    public static bool isPaused = false;

    public GameObject pauseMenuUI;

    //[SerializeField] public Scene startMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        //Debug.Log("Loading Menu...");
        isPaused = false;
        SceneManager.LoadScene("Main_Menu"); //get rid of this hardcode
    }

    public void QuitGame()
    {
        //Debug.Log("Quiting Game...");
        Application.Quit();
    }
}
