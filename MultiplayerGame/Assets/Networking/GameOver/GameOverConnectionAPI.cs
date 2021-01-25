using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PhHashtable = ExitGames.Client.Photon.Hashtable;


public class GameOverConnectionAPI : MonoBehaviourPunCallbacks, IOnEventCallback//, ILobbyCallbacks
{
    // --- Singleton ---
    private static GameOverConnectionAPI m_Instance;
    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this);
    }
    // ------------------


    // --- Online Settings ---
    public bool RoomIsJoinableAfterStart = false;

    // --- UI Object for Networking ---
    [SerializeField]
    private GameOverNetworkUIScript NetUIManager;

    // --- Match Info ---
    [SerializeField]
    private GameObject[] BlueUsersInfo;

    [SerializeField]
    private GameObject[] OrangeUsersInfo;

    [SerializeField]
    private Text WinnerText;

    [SerializeField]
    private Text BlueScoreText;

    [SerializeField]
    private Text OrangeScoreText;

    // --- Team Events ---
    public byte TeamJoinedEvent = 28;
    public byte TeamSwitchedEvent = 29;

    // --- Class Methods ---
    public void Quitting()
    {
        PhotonHandler.AppQuits = true;
    }

    public void Start()
    {
        int blue_score = 0, orange_score = 0;
        int blue_index = 0, orange_index = 0;
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            TEAMS team = (TEAMS)player.CustomProperties["Team"];
            int score = player.GetScore();

            if(team == TEAMS.TEAM_A)
            {
                OrangeUsersInfo[orange_index].SetActive(true);
                OrangeUsersInfo[orange_index].transform.Find("BluePlayer_Txt").GetComponent<Text>().text = player.NickName;
                OrangeUsersInfo[orange_index].transform.Find("BlueScore_Txt").GetComponent<Text>().text = score.ToString("0000");
                ++orange_index;
                orange_score += score;
            }
            else if (team == TEAMS.TEAM_B)
            {
                BlueUsersInfo[blue_index].SetActive(true);
                BlueUsersInfo[blue_index].transform.Find("BluePlayer_Txt").GetComponent<Text>().text = player.NickName;
                BlueUsersInfo[blue_index].transform.Find("BlueScore_Txt").GetComponent<Text>().text = score.ToString("0000");
                ++blue_index;
                blue_score += score;
            }
        }

        OrangeScoreText.text = orange_score.ToString("0000");
        BlueScoreText.text = blue_score.ToString("0000");

        if (blue_score > orange_score)
            WinnerText.text = "Team B Wins!";
        else if(orange_score > blue_score)
            WinnerText.text = "Team A Wins!";
        else
            WinnerText.text = "There is a Tie!";
    }


    // ----------------------------------------------------------------------
    // ------------------------- NETWORK FUNCTIONS --------------------------
    public void SendEvent(byte event_code, object content)
    {
        PhotonNetwork.RaiseEvent(event_code, content, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == TeamJoinedEvent)
            NetUIManager.PlayerJoinedTeam((string)photonEvent.CustomData);
        else if(photonEvent.Code == TeamSwitchedEvent)
            NetUIManager.PlayerSwitchedTeam((string)photonEvent.CustomData);
    }



    // ----------------------------------------------------------------------
    // ------------------------- SETTERS && GETTERS -------------------------
    // --- Status ---
    public bool     IsConnectedAndReady()       { return PhotonNetwork.IsConnectedAndReady; }
    public int      GetPing()                   { return PhotonNetwork.GetPing(); }
    public void     ShowError(string error)     { NetUIManager.ShowError(error); }

    // --- Player ---
    public string   GetUsername()               { return PhotonNetwork.NickName; }
    public string   GetUserID()                 { return PhotonNetwork.LocalPlayer.UserId; }

    public string GetPlayerByID(string player_id)
    {
        if (!PhotonNetwork.InRoom)
            return null;

        foreach (Player pl in PhotonNetwork.PlayerList)
            if (pl.UserId == player_id)
                return pl.NickName;

        return null;
    }

    public void SetLocalPlayerProperty<T>(string name, T property)
    {
        PhHashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash[name] = property;

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public object GetPlayerProperty(string player_id, string property_name)
    {
        if (!PhotonNetwork.InRoom)
            return null;

        foreach (Player pl in PhotonNetwork.PlayerList)
        {
            if (pl.UserId == player_id)
            {
                if (pl.CustomProperties.ContainsKey(property_name))
                    return pl.CustomProperties[property_name];

                return null;
            }
        }

        return null;
    }

    // --- Rooms ---
    public int GetPlayersCount()
    {
        if (PhotonNetwork.InRoom)
            return PhotonNetwork.PlayerList.Length;

        return -1;
    }

    public string GetRoomHost()
    {
        if (PhotonNetwork.InRoom)
            return PhotonNetwork.MasterClient.NickName;

        return null;
    }

    public string GetRoomName()
    {
        if (PhotonNetwork.InRoom)
            return PhotonNetwork.CurrentRoom.Name;
        
        return null;
    }

    ///<summary> First is ID, second is username </summary>
    public List<KeyValuePair<string, string>> GetPlayersInRoom()
    {
        List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();

        if (PhotonNetwork.InRoom)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
                ret.Add(new KeyValuePair<string, string>(player.UserId, player.NickName));
        }

        return ret;
    }



    // ----------------------------------------------------------------------
    // ------------------------ CONNECTION FUNCTIONS ------------------------
    public void LoadLevel(int scene_index)
    {
        // Check if we are on MasterClient (host - only the host can begin the match)
        if (PhotonNetwork.IsMasterClient)
        {
            // Check if we are ready to make operations
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            {
                // Set room visibility & openess once the game began
                PhotonNetwork.CurrentRoom.IsOpen = RoomIsJoinableAfterStart;
                PhotonNetwork.CurrentRoom.IsVisible = RoomIsJoinableAfterStart;

                // Load Scene
                PhotonNetwork.LoadLevel(scene_index);
            }
            else
                NetUIManager.ShowWarn("Still Connecting or Not in Room");
        }
        else
            NetUIManager.ShowWarn("Only the Room Host can Start the Game!");
    }

    public void LeaveRoom()
    {
        // Check if we are ready to make operations
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom(true);
    }



    // ----------------------------------------------------------------------
    // ------------------------ CONNECTION CALLBACKS ------------------------
    // --- Rooms ---
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    // --- Players ---
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        NetUIManager.PlayerJoined(newPlayer.NickName, newPlayer.UserId);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        NetUIManager.PlayerLeft(otherPlayer.NickName, otherPlayer.UserId);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        NetUIManager.SwitchHost(newMasterClient.NickName, newMasterClient.UserId);
    }
}