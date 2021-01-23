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
    private Text RoomText;

    // --- Players List ---
    [SerializeField]
    private Transform ListContainer;

    [SerializeField]
    private GameObject ListElement;

    private List<GameObject> m_TeamAList = new List<GameObject>();
    private List<GameObject> m_TeamBList = new List<GameObject>();


    // --- Class Methods ---
    private void Awake()
    {
        for(uint i = 0; i < 5; ++i)
            m_TeamAList.Add(Instantiate(ListElement, ListContainer));

        for(uint i = 0; i < 5; ++i)
        {
            GameObject obj = Instantiate(ListElement, ListContainer);
            obj.GetComponent<PlayerListElementScript>().SetOrangeImage();
            m_TeamBList.Add(obj);
        }
    }

    private void OnEnable()
    {
        // Check if there is a user with that name already
        /*uint player_name_repetitions = 0;
        foreach(string player in ConnectionManager.GetPlayerNamesInRoom())
            if (player == ConnectionManager.GetUsername())
                ++player_name_repetitions;

        if(player_name_repetitions > 1)
        {
            ConnectionManager.LeaveRoom();
            ConnectionManager.ShowError("A user with that name already exists in the room!");
            return;
        }*/

        // Set players list
        uint playersA = 0, playersB = 0;
        foreach (string player in ConnectionManager.GetPlayerNamesInRoom())
        {
            if (player == ConnectionManager.GetUsername())
                continue;

            object property = ConnectionManager.GetPlayerProperty(player, "Team");
            if (property != null)
            {
                AddPlayer(player, (TEAMS)property, false);
                if ((TEAMS)property == TEAMS.TEAM_A)
                    ++playersA;
                else
                    ++playersB;
            }
        }        

        // Set player team
        TEAMS user_team = TEAMS.NONE;
        if (playersA <= playersB)
            user_team = TEAMS.TEAM_A;
        else
            user_team = TEAMS.TEAM_B;

        // Set user's team
        ConnectionManager.SetLocalPlayerProperty("Team", user_team);
        ConnectionManager.SendEvent(ConnectionManager.TeamJoinedEvent, ConnectionManager.GetUsername());        
        AddPlayer(ConnectionManager.GetUsername(), user_team, true);

        // Set room name
        RoomText.text = ConnectionManager.GetRoomName();
    }

    private void OnDisable()
    {
        // Reset the list of players in teams
        foreach (GameObject playerA in m_TeamAList)
            playerA.GetComponent<PlayerListElementScript>().Deactivate();

        foreach (GameObject playerB in m_TeamBList)
            playerB.GetComponent<PlayerListElementScript>().Deactivate();
    }

    private void AddPlayer(string player_name, TEAMS team, bool user)
    {
        // Activate a team's list element
        bool host = player_name == ConnectionManager.GetRoomHost();
        if (team == TEAMS.TEAM_A)
        {
            foreach (GameObject playerA in m_TeamAList)
            {
                PlayerListElementScript list_element = playerA.GetComponent<PlayerListElementScript>();
                if (!list_element.Occupied)
                {
                    list_element.Activate(player_name, user, host);
                    return;
                }
            }
        }
        else if (team == TEAMS.TEAM_B)
        {
            foreach (GameObject playerB in m_TeamBList)
            {
                PlayerListElementScript list_element = playerB.GetComponent<PlayerListElementScript>();
                if (!list_element.Occupied)
                {
                    list_element.Activate(player_name, user, host);
                    return;
                }
            }
        }
    }


    private PlayerListElementScript GetPlayer(string player_name)
    {
        foreach (GameObject playerA in m_TeamAList)
        {
            PlayerListElementScript list_element = playerA.GetComponent<PlayerListElementScript>();
            if (player_name == list_element.GetPlayerName())
                return list_element;
        }


        foreach (GameObject playerB in m_TeamBList)
        {
            PlayerListElementScript list_element = playerB.GetComponent<PlayerListElementScript>();
            if (player_name == list_element.GetPlayerName())
                return list_element;
        }

        return null;
    }


    // --- Connection Callbacks ---
    public void PlayerJoinedRoom(string player_name)
    {
    }

    public void PlayerLeftRoom(string player_name)
    {
        PlayerListElementScript list_element = GetPlayer(player_name);
        if(list_element)
            list_element.Deactivate();
    }

    public void ChangeHost(string new_host_name)
    {
        PlayerListElementScript list_element = GetPlayer(new_host_name);
        if (list_element)
            list_element.SetHost();
    }

    public void PlayerJoinedTeam(string player_name)
    {
        TEAMS team = (TEAMS)ConnectionManager.GetPlayerProperty(player_name, "Team");
        AddPlayer(player_name, team, false);
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