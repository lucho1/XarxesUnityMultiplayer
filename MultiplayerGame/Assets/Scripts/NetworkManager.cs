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

    public CentralSphereScript CentralSphere;
    public GameObject BluePlane, OrangePlane;
    private Timer m_CentralCheckTimer;
    private SpawnCharacter m_PlayerSpawn;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerSpawn = GetComponent<SpawnCharacter>();
        m_Camera = GameObject.Find("Main Camera");
        m_CentralCheckTimer = GetComponent<Timer>();
        PhotonNetwork.AddCallbackTarget(this);

        MatchTimer = GetComponent<BackTimer>();
        MatchTimer.StartTime = (int)PhotonNetwork.CurrentRoom.CustomProperties["MatchTime"] * 60.0f;
        MatchTimer.Begin();
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
            BluePlane.SetActive(false);
            OrangePlane.SetActive(false);
        }

        if(m_Ending)
        {
            if (m_Camera.GetComponent<GameOverMovement>().Arrived)
                GameOverTexts.SetActive(true);
        }

        // --- Central Stuff ---
        if (CentralSphere && m_CentralCheckTimer.ReadTime() > 1.0f)
        {
            m_CentralCheckTimer.RestartFromZero();
            bool playersA_inside = false, playersB_inside = false;

            List<Collider> colliders = CentralSphere.GetCollidersInCenter();
            if (colliders != null)
            {
                foreach (Collider coll in colliders)
                {
                    // Check here if there is TeamA players only, TeamB players only or both/none
                    if (coll.gameObject.layer == 8)
                        playersA_inside = true;
                    if (coll.gameObject.layer == 9)
                        playersB_inside = true;

                    if (playersA_inside && playersB_inside)
                        break;
                }
            }

            if (playersA_inside && !playersB_inside)
            {
                BluePlane.SetActive(true);
                OrangePlane.SetActive(false);

                foreach(Collider coll in colliders)
                    coll.gameObject.GetComponent<PlayerController>().AddScore(5);
            }
            else if (!playersA_inside && playersB_inside)
            {
                BluePlane.SetActive(false);
                OrangePlane.SetActive(true);

                foreach (Collider coll in colliders)
                    coll.gameObject.GetComponent<PlayerController>().AddScore(5);
            }
            else
            {
                BluePlane.SetActive(false);
                OrangePlane.SetActive(false);
            }
        }
    }


    // --- CALLBACKS ---
    public override void OnJoinedRoom()
    {
        int teamA = 0, teamB = 0;
        foreach (Player player in PhotonNetwork.PlayerList) {
            if(player.CustomProperties.ContainsKey("Team"))
            {
                switch (player.CustomProperties["Team"]) {
                    case TEAMS.TEAM_A:
                        ++teamA;
                        break;
                    case TEAMS.TEAM_B:
                        ++teamB;
                        break;
                }
            }
        }

        PhHashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        TEAMS team = teamA <= teamB ? team = TEAMS.TEAM_A : team = TEAMS.TEAM_B;
        hash["Team"] = team;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        m_PlayerSpawn.SpawnNewPlayer(team);
    }


    // --- UI CALLBACKS ---
    public void LoadGameOverScreen()
    {
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(3);
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
