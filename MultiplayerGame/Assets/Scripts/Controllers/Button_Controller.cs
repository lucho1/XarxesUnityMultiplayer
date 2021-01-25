using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Controller : MonoBehaviour
{
    public GameObject Button;
    public GameObject MainMenuContainer;
    public ConnectionAPIScript ConnectionManager;

    public bool OnButton = false;
    
    private AudioSource m_AudioSrc;
    private Animation m_BtnAnimation;

    private bool start_game = false;

    void Start()
    {
        m_AudioSrc = GetComponent<AudioSource>();
        m_BtnAnimation = Button.GetComponent<Animation>();
    }

    public void ButtonPressed()
    {
        m_AudioSrc.Play();
        m_BtnAnimation.Play();

        start_game = true;
    }

    public void GoBack()
    {
        if (start_game)
            return;

        OnButton = false;
        MainMenuContainer.SetActive(true);

        GameObject.Find("Main Camera").transform.Find("DeathLoop").gameObject.SetActive(true);
        GameObject.Find("Main Camera").GetComponent<ToTarget>().ResetCameraMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (OnButton)
        {
            if (start_game && !m_BtnAnimation.isPlaying)
                ConnectionManager.LoadLevel(2);
        }
    }
}
