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
        // Show list of players
        List<string> players = ConnectionManager.GetPlayerNamesInRoom();
        if (players.Count > 0)
        {
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
