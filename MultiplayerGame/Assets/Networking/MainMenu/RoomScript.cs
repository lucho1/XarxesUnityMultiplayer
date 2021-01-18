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
    

    // --- Class Methods ---
    private void OnEnable()
    {
        // Set players list & teams
        uint PlayersTeamA = 0, PlayersTeamB = 0;
        foreach (string player in ConnectionManager.GetPlayerNamesInRoom())
        {
            AddPlayerToList(player);
            if (player != ConnectionManager.GetUsername())
            {
                TEAMS team = (TEAMS)ConnectionManager.GetPlayerProperty(player, "Team");
                Text show_name = m_PlayerList[player].GetComponentInChildren<Text>();

                if (team == TEAMS.TEAM_A)
                {
                    ++PlayersTeamA;
                    show_name.text += " -- TEAM A";
                }
                else if (team == TEAMS.TEAM_B)
                {
                    ++PlayersTeamB;
                    show_name.text += " -- TEAM B";
                }
            }
        }

        // Set player team
        if (PlayersTeamA <= PlayersTeamB)
            SetPlayerTeam(TEAMS.TEAM_A);
        else
            SetPlayerTeam(TEAMS.TEAM_B);

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

    private void SetPlayerTeam(TEAMS team)
    {
        // Set team property & send event
        ConnectionManager.SetLocalPlayerProperty("Team", team);
        ConnectionManager.SendEvent(ConnectionManager.PlayerTeamUpdated_Event, ConnectionManager.GetUsername());
        
        // Set Team Text
        string team_str = "";
        if (team == TEAMS.TEAM_A)
            team_str = "TEAM A";
        else if (team == TEAMS.TEAM_B)
            team_str = "TEAM B";

        Text player_show_name = m_PlayerList[ConnectionManager.GetUsername()].GetComponentInChildren<Text>();
        player_show_name.text += " -- " + team_str;
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

        // Set host team again
        object team = ConnectionManager.GetPlayerProperty(new_host_name, "Team");

        if (team != null)
            SetPlayerTeam((TEAMS)team);
    }

    public void PlayerJoinedTeam(string player_name)
    {
        if (!m_PlayerList.ContainsKey(player_name))
            return;

        TEAMS team = (TEAMS)ConnectionManager.GetPlayerProperty(player_name, "Team");
        Text player_show_name = m_PlayerList[player_name].GetComponentInChildren<Text>();

        if (team == TEAMS.TEAM_A)
            player_show_name.text += " -- TEAM A";
        else if (team == TEAMS.TEAM_B)
            player_show_name.text += " -- TEAM B";
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
