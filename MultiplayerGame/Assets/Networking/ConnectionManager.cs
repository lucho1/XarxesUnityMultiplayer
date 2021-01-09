using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public int MaxPlayers = 10;   
    public string GameVersion = "1.0";
    public Text Username;
    
    [SerializeField]
    private Text RoomName;

    [SerializeField]
    private Text ErrorText;

    private Timer m_ErrorTextTime;

    private void Awake()
    {
        // This will allow us to use PhotonNetwork.LoadLevel() on the master and all clients in room will sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        m_ErrorTextTime = GetComponent<Timer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        print("Connecting to Server");

        PhotonNetwork.NickName = Username.text;
        PhotonNetwork.GameVersion = GameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (ErrorText.IsActive() && m_ErrorTextTime.ReadTime() > 2.0f)
        {
            m_ErrorTextTime.Restart();
            m_ErrorTextTime.Stop();

            ErrorText.text = "Error Text";
            ErrorText.gameObject.SetActive(false);
        }
    }

    // --- Connection Override Callbacks ---
    public override void OnConnectedToMaster()
    {
        print("Connected to MasterServer");
        PhotonNetwork.JoinLobby();
    }

    public override void OnConnected()
    {
        print("Connected to Server");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from Server: " + cause.ToString());
    }


    // ---------- ROOMS ----------
    public void JoinOrCreateRoom()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)MaxPlayers;
        PhotonNetwork.JoinOrCreateRoom(RoomName.text, options, TypedLobby.Default);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    // --- Informative Override Callbacks ---
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room Successfully", this);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        string error_message = "Error on Room Creation: " + message + " (#" + returnCode + ")";

        ErrorText.text = error_message;
        ErrorText.gameObject.SetActive(true);
        m_ErrorTextTime.Start();

        Debug.Log(error_message, this);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room Successfully", this);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        string error_message = "Error on Room Creation: " + message + " (#" + returnCode + ")";

        ErrorText.text = error_message;
        ErrorText.gameObject.SetActive(true);
        m_ErrorTextTime.Start();

        Debug.Log(error_message, this);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string error_message = "Error on Room Creation: " + message + " (#" + returnCode + ")";

        ErrorText.text = error_message;
        ErrorText.gameObject.SetActive(true);
        m_ErrorTextTime.Start();

        Debug.Log(error_message, this);
    }
}
