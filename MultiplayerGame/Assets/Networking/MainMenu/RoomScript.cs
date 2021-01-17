using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScript : MonoBehaviour
{
    [SerializeField]
    private ConnectionAPIScript ConnectionManager;
    
    [SerializeField]
    private Text UsernameText;

    [SerializeField]
    private Text RoomText;

    [SerializeField]
    private Text PlayersText;

    // --- Players List ---
    [SerializeField]
    private Transform ListContainer;

    [SerializeField]
    private GameObject ListElement;
    private Dictionary<string, GameObject> m_PlayerList = new Dictionary<string, GameObject>();


    // --- Class Methods ---
    private void OnEnable()
    {
        // Set room name and username
        foreach (string player in ConnectionManager.GetPlayerNamesInRoom())
            AddPlayerToList(player);

        UsernameText.text = "You: " + ConnectionManager.GetUsername();
        RoomText.text = ConnectionManager.GetRoomName();
    }

    private void OnDisable()
    {
        foreach (KeyValuePair<string, GameObject> player in m_PlayerList)
            Destroy(player.Value);

        m_PlayerList.Clear();
    }

    private void AddPlayerToList(string player_name, GameObject instance = null)
    {
        PlayersText.text = "Players: " + ConnectionManager.GetPlayersCount();
        string show_name = player_name;

        if (player_name == ConnectionManager.GetUsername())
            show_name += " (You)";

        if(player_name == ConnectionManager.GetRoomHost())
            show_name += " (Host)";

        if(instance == null)
            instance = Instantiate(ListElement, ListContainer);

        instance.GetComponentInChildren<Text>().text = show_name;
        m_PlayerList.Add(player_name, instance);
    }

    // --- Connection Callbacks ---
    public void PlayerJoinedRoom(string player_name)
    {
        if (!m_PlayerList.ContainsKey(player_name))
            AddPlayerToList(player_name);
    }

    public void PlayerLeftRoom(string player_name)
    {
        if (m_PlayerList.ContainsKey(player_name))
        {
            Destroy(m_PlayerList[player_name]);
            m_PlayerList.Remove(player_name);
            PlayersText.text = "Players: " + ConnectionManager.GetPlayersCount();
        }
    }

    public void ChangeHost(string new_host_name)
    {
        GameObject new_host_object = m_PlayerList[new_host_name];
        m_PlayerList.Remove(new_host_name);
        AddPlayerToList(new_host_name, new_host_object);
    }


    // --- UI Callbacks ---
    public void StartButton()
    {
        ConnectionManager.LoadLevel(1);
    }

    public void LeaveButton()
    {
        ConnectionManager.LeaveRoom();
    }
}
