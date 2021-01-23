using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListElementScript : MonoBehaviour
{
    [SerializeField]
    private GameObject UsernameObject;

    [SerializeField]
    private GameObject BlueImageObject;

    [SerializeField]
    private GameObject OrangeImageObject;

    [SerializeField]
    private GameObject Player1Image;

    [SerializeField]
    private GameObject Player2Image;

    [SerializeField]
    private GameObject UserHighlight;

    public bool Occupied = false;
    private bool m_Blue = true;
    private string m_PlayerName = "";

    public void SetHost()
    {
        UsernameObject.GetComponentInChildren<Text>().text += "\n(HOST)";
    }

    public string GetPlayerName()
    {
        return m_PlayerName;
    }

    public void Deactivate()
    {
        Occupied = false;
        m_PlayerName = "";
        UsernameObject.GetComponentInChildren<Text>().text = "";

        Player1Image.SetActive(false);
        Player2Image.SetActive(false);
        UsernameObject.SetActive(false);
        UserHighlight.SetActive(false);
    }

    public void Activate(string player_name, bool user, bool host)
    {
        Occupied = true;
        m_PlayerName = player_name;
        UsernameObject.SetActive(true);

        if (user)
            UserHighlight.SetActive(true);

        if(host)
            player_name += "\n(HOST)";

        UsernameObject.GetComponentInChildren<Text>().text = player_name;

        if (m_Blue)
        {
            Player1Image.SetActive(true);
            Player2Image.SetActive(false);
        }
        else
        {
            Player1Image.SetActive(false);
            Player2Image.SetActive(true);
        }
    }

    public void SetBlueImage()
    {
        m_Blue = true;
        BlueImageObject.SetActive(true);
        OrangeImageObject.SetActive(false);        
    }

    public void SetOrangeImage()
    {
        m_Blue = false;
        OrangeImageObject.SetActive(true);
        BlueImageObject.SetActive(false);
    }
}
