using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : MonoBehaviour
{
    public GameObject IngameMenu;
    private bool menuEnabled = false;


    void Update()
    {
        menuEnabled = IngameMenu.active;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("StartButton"))
        {
            if (menuEnabled)
                ResumeGame();
            else
                PauseGame();

            IngameMenu.SetActive(!menuEnabled);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void ResetScene()
    {
        SceneManager.LoadScene("Blocking_Scene");
    }

    public void PauseGame()
    {
        Time.timeScale= 0.00f;
    }

    public void ResumeGame()
    {
        Time.timeScale =1.00f;
    }

    public void PlayOnClickSound()
    {
        GetComponentInParent<AudioSource>().Play();
    }
}
