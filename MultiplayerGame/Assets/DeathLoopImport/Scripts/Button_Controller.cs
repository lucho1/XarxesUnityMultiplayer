using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Controller : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource AuSRC;
    private Animation anim;
    public GameObject Button;
    public GameObject PlayerOneLabel;
    public GameObject PlayerTwoLabel;
    public GameObject PRessStart;

    private bool JoysticStart1;
    private bool JoysticStart2;
    public bool OnButton;
    public MainMenuManager MainMenu;

    void Start()
    {
        AuSRC=GetComponent<AudioSource>();
        OnButton = false;
        anim = Button.GetComponent<Animation>();
        PlayerOneLabel.SetActive(false);
        PlayerTwoLabel.SetActive(false);
        PRessStart.SetActive(false);
        JoysticStart1 =false;
        JoysticStart2=false;
    }

    // Update is called once per frame
    void Update()
    {
        if (OnButton)
        {
            PRessStart.SetActive(true);
            if (Input.GetButtonDown("Start1"))
            {
                AuSRC.Play();
                anim.Play();
                PlayerOneLabel.SetActive(true);
                JoysticStart1 = true;
            }
            if (Input.GetButtonDown("Start2"))
            {
                AuSRC.Play();
                anim.Play();
                PlayerTwoLabel.SetActive(true);
                JoysticStart2 = true;
            }
        }
        if (JoysticStart1 && JoysticStart2)
        {
            MainMenu.StartGame();
        }
    }
}
