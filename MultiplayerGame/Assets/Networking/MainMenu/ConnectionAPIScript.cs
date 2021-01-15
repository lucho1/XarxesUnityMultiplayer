using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ConnectionAPIScript : MonoBehaviourPunCallbacks
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


    public int MaxPlayers = 10;
    public string GameVersion = "1.0";

    [SerializeField]
    private RoomScript RoomManager;
    
    [SerializeField]
    private LobbyScript LobbyManager;
    
    [SerializeField]
    private NetworkUIScript NetUIManager;


    // Start is called before the first frame update
    void Start()
    {
        ConnectToNetwork();
    }


    // ----------------------------------------------------------------------
    // ------------------------- NETWORK FUNCTIONS --------------------------
    public void ConnectToNetwork()
    {
        print("Connecting to Server...");

        // This allow us to use PhotonNetwork.LoadLevel() on the master and all clients in room will sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        // Set Settings
        string name = "User" + Random.Range(0000, 9999);
        PhotonNetwork.NickName = name;
        PhotonNetwork.GameVersion = GameVersion;

        // Connect and add callback
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AddCallbackTarget(this);
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
    // --- Username ---
    public void SetUsername(string name)    { PhotonNetwork.NickName = name; }
    public string GetUsername()             { return PhotonNetwork.NickName; }
    
    // --- Status ---
    public bool IsConnectedAndReady()       { return PhotonNetwork.IsConnectedAndReady; }

    // --- Room Stuff ---
    public string GetRoomName()
    {
        if (PhotonNetwork.InRoom)
            return PhotonNetwork.CurrentRoom.Name;
        else
            return null;
    }

    public List<string> GetPlayerNamesInRoom()
    {
        List<string> ret = null;

        if (PhotonNetwork.InRoom)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
                ret.Add(player.NickName);
        }

        return ret;
    }



    // ----------------------------------------------------------------------
    // ------------------------ CONNECTION FUNCTIONS ------------------------
    public void JoinRoom(string room_name)
    {
        if(string.IsNullOrEmpty(room_name) || string.IsNullOrWhiteSpace(room_name))
        {
            NetUIManager.ShowWarn("Invalid Room Name!");
            return;
        }

        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
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
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
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
        if (string.IsNullOrEmpty(room_name) || string.IsNullOrWhiteSpace(room_name))
        {
            NetUIManager.ShowWarn("Invalid Room Name!");
            return;
        }

        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
        {
            string error = "";
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)MaxPlayers;
            options.BroadcastPropsChangeToAll = true;

            if (!PhotonNetwork.CreateRoom(room_name, ref error, options, TypedLobby.Default))
                NetUIManager.ShowError(("Create Room Error: " + error));
        }
        else
            NetUIManager.ShowWarn("Still Connecting or Not Ready to Create Room");
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
        foreach(RoomInfo room in roomList)
            LobbyManager.RoomListUpdate(room.RemovedFromList, room.Name);
    }
}
