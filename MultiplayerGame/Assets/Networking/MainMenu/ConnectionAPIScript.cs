using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PhHashtable = ExitGames.Client.Photon.Hashtable;


public class ConnectionAPIScript : MonoBehaviourPunCallbacks, IOnEventCallback, ILobbyCallbacks
{
    // --- Singleton ---
    private static ConnectionAPIScript m_Instance;
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


    // --- Online General Settings ---
    public int MaxPlayersPerMatch = 8;
    public string GameVersion = "1.0";
    public bool RoomIsJoinableAfterStart = false;

    // --- UI Object for Networking ---
    [SerializeField]
    private NetworkUIScript NetUIManager;

    // --- For Room Info ---
    private List<RoomInfo> m_RoomInfoList = new List<RoomInfo>();

    // --- Team Events ---
    public byte PlayerTeamUpdated_Event = 1;


    // --- Class Methods ---
    // Start is called before the first frame update
    private void Start()
    {
        ConnectToNetwork();
    }


    
    // ----------------------------------------------------------------------
    // ------------------------- NETWORK FUNCTIONS --------------------------
    private void ConnectToNetwork()
    {
        print("Connecting to Server...");

        // This allow us to use PhotonNetwork.LoadLevel() on the master and all clients in room will sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        // Set Settings
        string name = "User" + Random.Range(0000, 9999).ToString("0000");
        PhotonNetwork.NickName = name;
        PhotonNetwork.GameVersion = GameVersion;

        // Connect and add callback
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void SendEvent(byte event_code, object content)
    {
        PhotonNetwork.RaiseEvent(event_code, content, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte ev_code = photonEvent.Code;
        if(ev_code == PlayerTeamUpdated_Event)
        {
            string player_name = (string)photonEvent.CustomData;
            NetUIManager.PlayerJoinedTeam(player_name);
        }
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master Server - Ready to Operate");

        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        print("Lobby Joined Successfully");
    }

    public override void OnConnected()
    {
        print("Connected to Network");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from Server: " + cause.ToString());
    }



    // ----------------------------------------------------------------------
    // ------------------------- SETTERS && GETTERS -------------------------
    // --- Status ---
    public bool     IsConnectedAndReady()       { return PhotonNetwork.IsConnectedAndReady; }
    public int      GetPing()                   { return PhotonNetwork.GetPing(); }
    
    // --- Player ---
    public void     SetUsername(string name)    { PhotonNetwork.NickName = name; }
    public string   GetUsername()               { return PhotonNetwork.NickName; }
    
    public void SetLocalPlayerProperty<T>(string name, T property)
    {
        PhHashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash[name] = property;

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void SetPlayerProperty<T>(string player_name, string property_name, T property)
    {
        if (!PhotonNetwork.InRoom)
            return;

        foreach (Player pl in PhotonNetwork.PlayerList)
        {
            if (pl.NickName == player_name)
            {
                PhHashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
                hash[property_name] = property;

                pl.SetCustomProperties(hash);
                return;
            }
        }
    }

    public object GetPlayerProperty(string player_name, string property_name)
    {
        if (!PhotonNetwork.InRoom)
            return null;

        foreach (Player pl in PhotonNetwork.PlayerList)
        {
            if (pl.NickName == player_name)
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

    public List<string> GetPlayerNamesInRoom()
    {
        List<string> ret = new List<string>();

        if (PhotonNetwork.InRoom)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
                ret.Add(player.NickName);
        }

        return ret;
    }

    public int GetRoomPlayerCount(string room_name)
    {
        foreach(RoomInfo room in m_RoomInfoList)
        {
            if (room.Name == room_name)
                return room.PlayerCount;
        }

        return -1;
    }



    // ----------------------------------------------------------------------
    // ------------------------ CONNECTION FUNCTIONS ------------------------
    public void JoinRoom(string room_name)
    {
        // Check if passed string is valid
        if(string.IsNullOrEmpty(room_name) || string.IsNullOrWhiteSpace(room_name))
        {
            NetUIManager.ShowWarn("No Room Selected or Invalid Room Name!");
            return;
        }

        // Check if we are ready to make operations
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom && PhotonNetwork.InLobby)
        {
            string error = "";
            if(!PhotonNetwork.JoinRoom(room_name, ref error))
                NetUIManager.ShowError(("Join Room Error: " + error));
        }
        else
            NetUIManager.ShowWarn("Still Connecting or Not Ready to Join Room");
    }


    public void JoinRandomRoom()
    {
        // Check if we are ready to make operations
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom && PhotonNetwork.InLobby)
        {
            string error = "";
            if (!PhotonNetwork.JoinRandomRoom(ref error))
                NetUIManager.ShowError(("Join Random Room Error: " + error));
        }
        else
            NetUIManager.ShowWarn("Still Connecting or Not Ready to Join Room");
    }


    public void HostRoom(string room_name)
    {
        // Check if passed string is valid
        if (string.IsNullOrEmpty(room_name) || string.IsNullOrWhiteSpace(room_name))
        {
            NetUIManager.ShowWarn("Invalid Room Name!");
            return;
        }

        // Check if we are ready to make operations
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom && PhotonNetwork.InLobby)
        {
            string error = "";
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)MaxPlayersPerMatch;
            options.BroadcastPropsChangeToAll = true;

            if (!PhotonNetwork.CreateRoom(room_name, ref error, options, TypedLobby.Default))
                NetUIManager.ShowError(("Create Room Error: " + error));
        }
        else
            NetUIManager.ShowWarn("Still Connecting or Not Ready to Create Room");
    }

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
        {
            PhotonNetwork.LeaveRoom(true);
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }



    // ----------------------------------------------------------------------
    // ------------------------ CONNECTION CALLBACKS ------------------------
    // --- Rooms ---
    public override void OnJoinedRoom()
    {
        NetUIManager.JoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        NetUIManager.JoinedRoomFailed(message, (int)returnCode);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        NetUIManager.JoinedRoomFailed(message, (int)returnCode);
    }

    public override void OnCreatedRoom()
    {
        NetUIManager.RoomCreated();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Very ugly hardcoded piece of code, but this message is more readable this way :)
        // and I cannot access to the message creator because it's a server response :(
        if(message.Contains("A game with the specified id already exist"))
        {
            message = "A Room with this Name already exists!";
            returnCode = 0;
        }

        NetUIManager.RoomCreatedFailure(message, (int)returnCode);
    }

    public override void OnLeftRoom()
    {
        NetUIManager.RoomLeft();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            NetUIManager.RoomListUpdated(room.RemovedFromList, room.Name);
            if (room.RemovedFromList)
                m_RoomInfoList.Remove(room);
            else
                m_RoomInfoList.Add(room);
        }
    }

    // --- Players ---
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        NetUIManager.PlayerJoined(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        NetUIManager.PlayerLeft(otherPlayer.NickName);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        NetUIManager.SwitchHost(newMasterClient.NickName);
    }
}