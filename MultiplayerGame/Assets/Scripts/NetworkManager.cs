using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PhHashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // --- Singleton ---
    private static NetworkManager m_Instance;
    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this);
    }
    // ------------------

    // Score
    [SerializeField]
    private Text A_Score_Text;

    [SerializeField]
    private Text B_Score_Text;

    // Match Timer & Game Over
    [SerializeField]
    private GameObject GameOverTexts;
    
    [SerializeField]
    private Text TimerText;

    private BackTimer MatchTimer;
    private bool m_ActivateGameEnd = true;
    private bool m_Ending = false;
    private GameObject m_Camera;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
        MatchTimer = GetComponent<BackTimer>();
        m_Camera = GameObject.Find("Main Camera");
    }

    private void OnLevelWasLoaded(int level)
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        m_ActivateGameEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
                PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.IsVisible = false;
            else
                PhotonNetwork.CurrentRoom.IsOpen = PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        if(MatchTimer.Running)
            TimerText.text = "Time: " + MatchTimer.GetTimeString();

        if (MatchTimer.Finished && m_ActivateGameEnd)
        {
            m_ActivateGameEnd = false;
            m_Ending = true;
            m_Camera.GetComponent<SingleTargetCamera>().FollowTarget = false;
            m_Camera.GetComponent<GameOverMovement>().MoveToTarget = true;
        }

        if(m_Ending)
        {
            if (m_Camera.GetComponent<GameOverMovement>().Arrived)
                GameOverTexts.SetActive(true);
        }
    }


    // --- CALLBACKS ---
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int teamA = 0, teamB = 0;
        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            if(player.Value.CustomProperties.ContainsKey("Team"))
            {
                if ((TEAMS)player.Value.CustomProperties["Team"] == TEAMS.TEAM_A)
                    ++teamA;
                else if ((TEAMS)player.Value.CustomProperties["Team"] == TEAMS.TEAM_B)
                    ++teamB;
            }
        }

        PhHashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        TEAMS team = teamA <= teamB ? team = TEAMS.TEAM_A : team = TEAMS.TEAM_B;        
        hash["Team"] = team;
    }

    public override void OnLeftRoom()
    {
        //SceneManager.LoadScene("MainMenu");
    }

    // --- UI CALLBACKS ---
    public void LoadGameOverScreen()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public void LeaveMatch()
    {
        // Check if we are ready to make operations
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom(true);
            print("Connecting to Lobby...");
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby(TypedLobby.Default);

            SceneManager.LoadScene("MainMenu");
        }
    }
}
