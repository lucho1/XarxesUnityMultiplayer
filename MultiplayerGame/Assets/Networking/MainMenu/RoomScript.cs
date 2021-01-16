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


    private void OnEnable()
    {
        // Set room name and username
        UsernameText.text = "You: " + ConnectionManager.GetUsername();
        RoomText.text = ConnectionManager.GetRoomName();
    }

    // Update is called once per frame
    private void Update()
    {
        // Show players quantity (in room)
        PlayersText.text = "Players: " + ConnectionManager.GetPlayersCount();

        // Show list of players
        List<string> players = ConnectionManager.GetPlayerNamesInRoom();
        if (players.Count > 0)
        {
            // Add a marker for your user and Host user
            int player_index = players.FindIndex(x => x == ConnectionManager.GetUsername());
            int master_index = players.FindIndex(x => x == ConnectionManager.GetRoomHost());

            if(master_index != -1)
                players[master_index] += " (Master)";
            if (player_index != -1)
                players[player_index] += " (You)";

            // Setup the Players List
            PlayersListText.text = "";
            foreach (string player in players)
                PlayersListText.text += "\n" + player;
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
