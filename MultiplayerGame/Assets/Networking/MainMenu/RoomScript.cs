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

    [SerializeField]
    private Text PlayersListText;

    // --- Players List ---
    [SerializeField]
    private Transform ListContainer;
    [SerializeField]
    private GameObject ListElement;
    private Dictionary<string, GameObject> m_PlayerList = new Dictionary<string, GameObject>();


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

    // Update is called once per frame
    private void Update()
    {
        // Show players quantity (in room)
        PlayersText.text = "Players: " + ConnectionManager.GetPlayersCount();

        // Show list of players
        //List<string> players = ConnectionManager.GetPlayerNamesInRoom();
        //if (players.Count > 0)
        //{
        //    // Add a marker for your user and Host user
        //    int player_index = players.FindIndex(x => x == ConnectionManager.GetUsername());
        //    int master_index = players.FindIndex(x => x == ConnectionManager.GetRoomHost());
        //
        //    if(master_index != -1)
        //        players[master_index] += " (Master)";
        //    if (player_index != -1)
        //        players[player_index] += " (You)";
        //
        //    // Setup the Players List
        //    PlayersListText.text = "";
        //    foreach (string player in players)
        //        PlayersListText.text += "\n" + player;
        //}
    }

    public void AddPlayerToList(string player_name)
    {
        GameObject new_element = Instantiate(ListElement, ListContainer);
        new_element.GetComponentInChildren<Text>().text = player_name;

        m_PlayerList.Add(player_name, new_element);
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
        }
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
