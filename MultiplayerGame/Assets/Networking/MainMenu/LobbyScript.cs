using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour
{
    [SerializeField]
    private ConnectionAPIScript ConnectionManager;

    [SerializeField]
    private GameObject RoomListText;
    private List<string> m_RoomList = new List<string>();


    // Update is called once per frame
    void Update()
    {
        if (m_RoomList.Count > 0)
        {
            RoomListText.GetComponent<Text>().text = "";
            foreach (string room in m_RoomList)
                RoomListText.GetComponent<Text>().text += "\n" + room;
        }
    }


    // --- Connection Callbacks ---
    public void RoomListUpdate(bool removed, string room_name)
    {
        if(removed)
            m_RoomList.Remove(room_name);
        else if(m_RoomList.FindIndex(0, m_RoomList.Count, x=>x == room_name) != -1)
                m_RoomList.Add(room_name);
    }


    // --- UI Callbacks ---
    private void SetUsername(Text name)
    {
        ConnectionManager.SetUsername(name.text);
    }

    private void JoinRoomButton()
    {

    }

    private void HostRoomButton()
    {

    }

    private void QuickStartButton()
    {

    }
}
