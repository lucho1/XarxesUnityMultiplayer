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
        {
            Destroy(this);
        }
    }
    // ------------------


    public int MaxPlayers = 10;
    public string GameVersion = "1.0";

    //[SerializeField]
    //private GameObject RoomObject;
    //private RoomScript RoomManager;
    //
    //[SerializeField]
    //private GameObject LobbyObject;
    //private LobbyScript LobbyManager;
    //
    //[SerializeField]
    //private GameObject NetworkObject;
    //private NetworkUIScript NetUIManager;


    // Start is called before the first frame update
    void Start()
    {
        ConnectToNetwork();

        //NetUIManager = NetworkObject.GetComponent<NetworkUIScript>();
        //LobbyManager = LobbyManager.GetComponent<LobbyScript>();
        //RoomManager = RoomObject.GetComponent<RoomScript>();
    }



    // ----------------------------------------------------------------------
    // ------------------------ CONNECTION FUNCTIONS ------------------------
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
        print("Connecting to Master Server - Ready to Operate");

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
    // ------------------------ CONNECTION CALLBACKS ------------------------
    
}
