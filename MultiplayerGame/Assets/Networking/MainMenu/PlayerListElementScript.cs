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
    private GameObject UserHighlight;

    [SerializeField]
    private List<GameObject> AnimationsList = new List<GameObject>();
    private int m_AnimationIndex = -1;

    public bool Occupied = false;
    private string m_PlayerName = "";
    private string m_PlayerID = "";


    public void SetHost()
    {
        UsernameObject.GetComponentInChildren<Text>().text += "\n(HOST)";
    }

    public string GetPlayerName()
    {
        return m_PlayerName;
    }

    public string GetPlayerID()
    {
        return m_PlayerID;
    }

    public int GetAnimationIndex()
    {
        return m_AnimationIndex;
    }

    public void Deactivate()
    {
        Occupied = false;
        m_PlayerName = m_PlayerID = "";
        UsernameObject.GetComponentInChildren<Text>().text = "";

        UsernameObject.SetActive(false);
        UserHighlight.SetActive(false);

        if(m_AnimationIndex != -1)
        {
            AnimationsList[m_AnimationIndex].SetActive(false);
            m_AnimationIndex = -1;
        }        
    }

    public void Activate(string player_name, string playerID, int animation_index, bool user, bool host)
    {
        Occupied = true;
        m_PlayerID = playerID;
        m_PlayerName = player_name;
        UsernameObject.SetActive(true);

        if (user)
            UserHighlight.SetActive(true);

        if(host)
            player_name += "\n(HOST)";

        UsernameObject.GetComponentInChildren<Text>().text = player_name;

        m_AnimationIndex = animation_index;
        AnimationsList[m_AnimationIndex].SetActive(true);
    }

    public void SetBlueImage()
    {
        BlueImageObject.SetActive(true);
        OrangeImageObject.SetActive(false);        
    }

    public void SetOrangeImage()
    {
        OrangeImageObject.SetActive(true);
        BlueImageObject.SetActive(false);
    }

    // --- Kick User ---
    [SerializeField]
    private GameObject m_KickButton;

    public void KickMenu()
    {
        ConnectionAPIScript conn = GameObject.Find("ConnectionManager").GetComponent<ConnectionAPIScript>();
        if (!conn.IsHost())
            return;

        if (m_PlayerName != conn.GetRoomHost())
        {
            if(m_KickButton.activeInHierarchy)
                m_KickButton.SetActive(false);
            else
                m_KickButton.SetActive(true);
        }
    }

    public void Kick()
    {
        ConnectionAPIScript conn = GameObject.Find("ConnectionManager").GetComponent<ConnectionAPIScript>();
        conn.SendEvent(conn.PlayerKickedEvent, m_PlayerID);
        m_KickButton.SetActive(false);
    }
}
