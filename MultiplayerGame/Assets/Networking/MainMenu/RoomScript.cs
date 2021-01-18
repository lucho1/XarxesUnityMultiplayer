using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomScript : MonoBehaviour
{
    // --- Connection Manager ---
    [SerializeField]
    private ConnectionAPIScript ConnectionManager;

    // --- UI Objects ---
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

    private bool update_teams = false;

    // --- Class Methods ---
    private void OnEnable()
    {
        // Set players list & teams
        uint TeamA_Players = 0, TeamB_Players = 0;
        foreach (string player in ConnectionManager.GetPlayerNamesInRoom())
        {
            AddPlayerToList(player);

            if (player != ConnectionManager.GetUsername())
            {
                TEAMS team = (TEAMS)ConnectionManager.GetPlayerProperty(player, "Team");

                if (team == TEAMS.TEAM_A)
                    ++TeamA_Players;
                else if (team == TEAMS.TEAM_B)
                    ++TeamB_Players;
            }
        }

        // Set player team
        Text player_show_name = m_PlayerList[ConnectionManager.GetUsername()].GetComponentInChildren<Text>();
        if (TeamA_Players <= TeamB_Players)
        {
            ConnectionManager.SetLocalPlayerProperty("Team", TEAMS.TEAM_A);
            player_show_name.text += " -- TEAM A";
        }
        else
        {
            ConnectionManager.SetLocalPlayerProperty("Team", TEAMS.TEAM_B);
            player_show_name.text += " -- TEAM B";
        }

        // Set room name and username
        UsernameText.text = "You: " + ConnectionManager.GetUsername();
        RoomText.text = ConnectionManager.GetRoomName();
    }

    private void OnDisable()
    {
        // Destroy & Clear the list of players
        foreach (KeyValuePair<string, GameObject> player in m_PlayerList)
            Destroy(player.Value);

        m_PlayerList.Clear();
    }

    private void Update()
    {
        if (update_teams)
        {
            foreach(KeyValuePair<string, GameObject> player in m_PlayerList)
            {
                Text player_show_name = player.Value.GetComponentInChildren<Text>();
                if(!player_show_name.text.Contains(" -- TEAM ") && ConnectionManager.GetPlayerProperty(player.Key, "Team") != null)
                {
                    TEAMS team = (TEAMS)ConnectionManager.GetPlayerProperty(player.Key, "Team");

                    if (team == TEAMS.TEAM_A)
                        player_show_name.text += " -- TEAM A";
                    else if (team == TEAMS.TEAM_B)
                        player_show_name.text += " -- TEAM B";
                }
            }

            update_teams = false;
        }
    }

    private void AddPlayerToList(string player_name, GameObject instance = null)
    {
        // Update the text for the quantity of players in room
        PlayersText.text = "Players: " + ConnectionManager.GetPlayersCount();
        
        // Set the name to show (which is not always the same as the username)
        string show_name = player_name;

        if (player_name == ConnectionManager.GetUsername())
            show_name += " (You)";

        if(player_name == ConnectionManager.GetRoomHost())
            show_name += " (Host)";

        // Set GameObject instance if null
        if (instance == null)
            instance = Instantiate(ListElement, ListContainer);

        // Set object show name & Add it to players list
        instance.GetComponentInChildren<Text>().text = show_name;
        m_PlayerList.Add(player_name, instance);
        update_teams = true;
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

            // Update the text for the quantity of players in room
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
