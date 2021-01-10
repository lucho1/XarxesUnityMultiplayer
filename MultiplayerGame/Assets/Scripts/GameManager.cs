﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject StartingMusic;

    // Start is called before the first frame update
    void Start()
    {
        if (StartingMusic)
            StartingMusic.GetComponent<MusicManager>().ActivateMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}