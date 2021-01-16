using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour
{
    [SerializeField]
    private ConnectionAPIScript ConnectionManager;

    [SerializeField]
    private Text RoomListText;
    private List<string> m_RoomList = new List<string>();

    [SerializeField]
    private InputField UsernameInput;

    // Start is called before the first frame update
    void Start()
    {
        UsernameInput.textComponent.text = ConnectionManager.GetUsername();
        UsernameInput.text = ConnectionManager.GetUsername();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_RoomList.Count > 0)
        {
            RoomListText.text = "";
            foreach (string room in m_RoomList)
                RoomListText.text += "\n" + room;
        }
    }


    // --- Connection Callbacks ---
    public void RoomListUpdate(bool removed, string room_name)
    {
        if(removed)
            m_RoomList.Remove(room_name);
        else if(m_RoomList.FindIndex(0, m_RoomList.Count, x=>x == room_name) == -1)
            m_RoomList.Add(room_name);
    }


    // --- UI Callbacks ---
    public void SetUsername(Text name)
    {
        ConnectionManager.SetUsername(name.text);
    }

    public void JoinRoomButton(Text room_name)
    {
        ConnectionManager.JoinRoom(room_name.text);
    }

    public void HostRoomButton(Text room_name)
    {
        ConnectionManager.HostRoom(room_name.text);
    }

    public void QuickStartButton()
    {
        ConnectionManager.JoinRandomRoom();
    }
}
