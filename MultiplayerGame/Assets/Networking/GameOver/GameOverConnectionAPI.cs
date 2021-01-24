using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using ExitGames.Client.Photon;
using Photon.Pun;
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

    // --- Team Events ---
    public byte TeamJoinedEvent = 28;
    public byte TeamSwitchedEvent = 29;

    // --- Class Methods ---
    public void Quitting()
    {
        PhotonHandler.AppQuits = true;
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