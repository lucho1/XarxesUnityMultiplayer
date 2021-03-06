﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public ConnectionAPIScript ConnectionManager;

    public void Connect()
    {
        ConnectionManager.TryConnection();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scenario");
    }

   public void ExitGame()
    {
        Application.Quit();
    }

    public void PlayOnClickSound()
    {
        GetComponentInParent<AudioSource>().Play();
    }
}