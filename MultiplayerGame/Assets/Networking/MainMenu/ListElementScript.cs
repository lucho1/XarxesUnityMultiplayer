using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListElementScript : MonoBehaviour
{
    public GameObject LobbyObject = null;
    private Timer m_Timer;

    [SerializeField]
    private Text PlayersInRoom;
    [SerializeField]
    private Text RoomName;

    private void Start()
    {
        m_Timer = GetComponent<Timer>();
        m_Timer.Play();
    }

    private void Update()
    {
        if(m_Timer.ReadTime() > 1.0f)
        {
            SetRoomPlayers(LobbyObject.GetComponent<LobbyScript>().GetPlayersInRoom(RoomName.text));
            m_Timer.RestartFromZero();
        }
    }

    public void SetSelectedRoom(Text text)
    {
        GetComponentInParent<AudioSource>().Play();

        if (LobbyObject)
            LobbyObject.GetComponent<LobbyScript>().SetCurrentSelectedRoom(text.text);
    }

    public void SetRoomPlayers(int players)
    {
        PlayersInRoom.text = players.ToString() + "/10";
    }
}
