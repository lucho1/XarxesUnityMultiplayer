using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    public MainMenuManager MainMenu;
    public GameObject Button;
    public GameObject PressStartText;

    public bool OnButton = false;
    
    private AudioSource m_AudioSrc;
    private Animation m_BtnAnimation;

    private bool start_game = false;

    void Start()
    {
        m_AudioSrc = GetComponent<AudioSource>();
        m_BtnAnimation = Button.GetComponent<Animation>();
        
        PressStartText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (OnButton)
        {
            PressStartText.SetActive(true);            

            if (Input.GetButtonDown("Start"))
            {
                m_AudioSrc.Play();
                m_BtnAnimation.Play();

                start_game = true;
            }

            if(Input.GetButtonDown("Cancel") && !start_game)
            {
                OnButton = false;
                PressStartText.SetActive(false);

                Transform main_menu = GameObject.Find("MainMenu").transform;
                main_menu.Find("StartGameButton").gameObject.SetActive(true);
                main_menu.Find("ExitGameButton").gameObject.SetActive(true);
                main_menu.Find("InstructionsButton").gameObject.SetActive(true);

                GameObject.Find("Main Camera").transform.Find("DeathLoop").gameObject.SetActive(true);
                GameObject.Find("Main Camera").GetComponent<ToTarget>().ResetCameraMovement();
            }

            if (start_game && !m_BtnAnimation.isPlaying)
                MainMenu.StartGame();
        }
    }
}
