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

    [SerializeField]
    private Text MatchTimeText;
    public int MatchTime = 5;

    // --- Players List ---
    [SerializeField]
    private Transform ListContainer;

    [SerializeField]
    private GameObject ListElement;

    private List<GameObject> m_TeamAList = new List<GameObject>();
    private List<GameObject> m_TeamBList = new List<GameObject>();

    // --- Switch Team timer ---
    private Timer m_SwitchTeamTimer;

    // --- Player List Animation ---
    private bool[] m_AnimationSelected = new bool[10];

    // --- Class Methods ---
    private void Awake()
    {
        m_SwitchTeamTimer = GetComponent<Timer>();

        for (int i = 0; i < 10; ++i)
            m_AnimationSelected[i] = false;

        for (uint i = 0; i < 5; ++i)
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
        // Set room name
        RoomText.text = ConnectionManager.GetRoomName();

        // Check if there is a user with that name already
        uint player_name_repetitions = 0;
        foreach (KeyValuePair<string, string> player in ConnectionManager.GetPlayersInRoom())
            if (player.Value == ConnectionManager.GetUsername())
                ++player_name_repetitions;

        if (player_name_repetitions > 1)
        {
            ConnectionManager.LeaveRoom();
            ConnectionManager.ShowError("A user with that name already exists in the room!");
            return;
        }

        // Set players list
        uint playersA = 0, playersB = 0;
        foreach (KeyValuePair<string, string> player in ConnectionManager.GetPlayersInRoom())
        {
            if (player.Value == ConnectionManager.GetUsername())
                continue;

            object property = ConnectionManager.GetPlayerProperty(player.Key, "Team");
            if (property != null)
            {
                AddPlayer(player.Value, player.Key, (TEAMS)property, false);
                if ((TEAMS)property == TEAMS.TEAM_A)
                    ++playersA;
                else
                    ++playersB;
            }
        }        

        // Set player team
        TEAMS user_team = playersA <= playersB ? TEAMS.TEAM_A : TEAMS.TEAM_B;

        ConnectionManager.SetLocalPlayerProperty("Team", user_team);
        ConnectionManager.SendEvent(ConnectionManager.TeamJoinedEvent, ConnectionManager.GetUserID());        
        AddPlayer(ConnectionManager.GetUsername(), ConnectionManager.GetUserID(), user_team, true);
    }

    private void OnDisable()
    {
        for (int i = 0; i < 10; ++i)
            m_AnimationSelected[i] = false;

        // Reset the list of players in teams
        foreach (GameObject playerA in m_TeamAList)
            playerA.GetComponent<PlayerListElementScript>().Deactivate();

        foreach (GameObject playerB in m_TeamBList)
            playerB.GetComponent<PlayerListElementScript>().Deactivate();
    }

    private void AddPlayer(string player_name, string player_id, TEAMS team, bool user)
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
                    int anim_index = Random.Range(0, 5);
                    while(m_AnimationSelected[anim_index])
                        anim_index = Random.Range(0, 5);

                    m_AnimationSelected[anim_index] = true;
                    list_element.Activate(player_name, player_id, anim_index, user, host);
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
                    int anim_index = Random.Range(5, 10);
                    while (m_AnimationSelected[anim_index])
                        anim_index = Random.Range(5, 10);

                    m_AnimationSelected[anim_index] = true;
                    list_element.Activate(player_name, player_id, anim_index, user, host);
                    return;
                }
            }
        }
    }


    private PlayerListElementScript GetPlayer(string player_id)
    {
        foreach (GameObject playerA in m_TeamAList)
        {
            PlayerListElementScript list_element = playerA.GetComponent<PlayerListElementScript>();
            if (player_id == list_element.GetPlayerID())
                return list_element;
        }

        foreach (GameObject playerB in m_TeamBList)
        {
            PlayerListElementScript list_element = playerB.GetComponent<PlayerListElementScript>();
            if (player_id == list_element.GetPlayerID())
                return list_element;
        }

        return null;
    }

    public void SwitchPlayerTeamOnList(string player_id, string player_name, TEAMS team, bool user)
    {
        PlayerListElementScript list_element = GetPlayer(player_id);

        if (list_element.GetAnimationIndex() != -1)
            m_AnimationSelected[list_element.GetAnimationIndex()] = false;

        list_element.Deactivate();
        AddPlayer(player_name, player_id, team, user);
    }


    // --- Connection Callbacks ---
    public void PlayerJoinedRoom(string player_id)
    {
        ConnectionManager.SetRoomVisibility();
    }

    public void PlayerLeftRoom(string player_id)
    {
        ConnectionManager.SetRoomVisibility();
        PlayerListElementScript list_element = GetPlayer(player_id);
        if(list_element)
            list_element.Deactivate();
    }

    public void ChangeHost(string new_host_id)
    {
        PlayerListElementScript list_element = GetPlayer(new_host_id);
        if (list_element)
            list_element.SetHost();
    }

    public void PlayerJoinedTeam(string player_name, string player_id)
    {
        TEAMS team = (TEAMS)ConnectionManager.GetPlayerProperty(player_id, "Team");
        AddPlayer(player_name, player_id, team, false);
    }

    public void PlayerSwitchedTeam(string player_name, string player_id)
    {
        TEAMS team = (TEAMS)ConnectionManager.GetPlayerProperty(player_id, "Team");
        SwitchPlayerTeamOnList(player_id, player_name, team, false);
    }
    


    // --- UI Callbacks ---
    public void StartButton()
    {
        ConnectionManager.LoadLevel(2);
        // TODO: Pass here the MatchTime value to the game
    }

    public void LeaveButton()
    {
        ConnectionManager.LeaveRoom();
    }

    public void IncreaseTime()
    {
        if (MatchTime < 10)
        {
            ++MatchTime;
            MatchTimeText.text = MatchTime.ToString();
        }
    }

    public void DecreaseTime()
    {
        if (MatchTime > 1)
        {
            --MatchTime;
            MatchTimeText.text = MatchTime.ToString();
        }
    }

    public void SwitchTeamButton()
    {
        object property = ConnectionManager.GetPlayerProperty(ConnectionManager.GetUserID(), "Team");
        if (property != null)
        {
            // Check if button was clicked recently
            if(m_SwitchTeamTimer.IsRunning() && m_SwitchTeamTimer.ReadTime() < 1.0f)
            {
                ConnectionManager.ShowError("Still setting up your recent team switch");
                return;
            }

            m_SwitchTeamTimer.RestartFromZero();

            // Switch Team
            TEAMS team = TEAMS.NONE;
            List<GameObject> ListToRun = new List<GameObject>();

            if ((TEAMS)property == TEAMS.TEAM_A)
            {
                team = TEAMS.TEAM_B;
                ListToRun = m_TeamBList;
            }
            else if ((TEAMS)property == TEAMS.TEAM_B)
            {
                team = TEAMS.TEAM_A;
                ListToRun = m_TeamAList;
            }


            // First, make sure Team is not full, otherwise, log error
            bool team_available = false;
            foreach (GameObject obj in ListToRun)
            {
                if(!obj.GetComponent<PlayerListElementScript>().Occupied)
                {
                    team_available = true;
                    break;
                }
            }

            if(!team_available)
            {
                ConnectionManager.ShowError("Opposite Team is full :(");
                return;
            }

            // Set the "Team" property and cast SwitchedEvent
            ConnectionManager.SetLocalPlayerProperty("Team", team);
            ConnectionManager.SendEvent(ConnectionManager.TeamSwitchedEvent, ConnectionManager.GetUserID());

            // Finally, set the player team
            SwitchPlayerTeamOnList(ConnectionManager.GetUserID(), ConnectionManager.GetUsername(), team, true);
        }
        else
            ConnectionManager.ShowError("You are not in a Team yet");
    }
}