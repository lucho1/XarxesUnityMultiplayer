using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    AudioSource audioSrc;
    public AudioClip init = null;
    public AudioClip Loop = null;

    private bool hasToPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponentInParent<AudioSource>();
    }

    public void StopMusic()
    {
        audioSrc.Stop();
        audioSrc.clip = init;
        audioSrc.time = 0.0f;
        hasToPlay = false;
    }

    public void ActivateMusic()
    {
        if(!audioSrc)
            audioSrc = GetComponentInParent<AudioSource>();

        if (init != null)
        {
            audioSrc.clip = init;
            audioSrc.loop = false;
        }
        else
        {
            audioSrc.clip = Loop;
            audioSrc.loop = true;
        }

        hasToPlay = true;
        audioSrc.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!audioSrc.isPlaying && hasToPlay)
        {
            audioSrc.clip = Loop;
            audioSrc.loop = true;
            audioSrc.Play();
        }
    }
}
