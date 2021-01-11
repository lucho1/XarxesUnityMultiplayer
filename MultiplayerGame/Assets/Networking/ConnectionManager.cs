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
    public string Username = "Lucho";

    private string CurrentRoomName = "";
    public string GetCurrentRoomName { get { return CurrentRoomName; } }

    public GameObject PlayerListObject;

    private void Awake()
    {
        // This will allow us to use PhotonNetwork.LoadLevel() on the master and all clients in room will sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        print("Connecting to Server");

        SetUsername(Username);
        PhotonNetwork.GameVersion = GameVersion;
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.AddCallbackTarget(this);

    }

    public void SetUsername(string name)
    {
        Username = name;
        PhotonNetwork.NickName = Username;
    }

    public void SetRoomName()
    {
        if(PhotonNetwork.InRoom)
            CurrentRoomName = PhotonNetwork.CurrentRoom.Name;
    }

    public bool IsInRoom()
    {
        return PhotonNetwork.InRoom;
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
    public bool JoinRandomRoom()
    {
        string error_msg = "";
        if (PhotonNetwork.JoinRandomRoom(ref error_msg))
        {
            if (PhotonNetwork.InRoom)
                CurrentRoomName = PhotonNetwork.CurrentRoom.Name;

            return true;
        }

        ShowError(error_msg, -1);        
        return false;
    }

    public bool JoinRoom(string room_name)
    {
        string error_msg = "";
        if (PhotonNetwork.JoinRoom(room_name, ref error_msg))
        {
            CurrentRoomName = room_name;
            return true;
        }

        ShowError(error_msg, -1);
        return false;
    }

    public bool CreateRoom(string room_name)
    {
        string error_msg = "";
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)MaxPlayers;

        if (PhotonNetwork.CreateRoom(room_name, ref error_msg, options, TypedLobby.Default))
        {
            CurrentRoomName = room_name;
            return true;
        }

        ShowError(error_msg, -1);
        return false;
    }

    public bool JoinOrCreateRoom(string room_name)
    {
        string error_msg = "";        
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)MaxPlayers;

        if (PhotonNetwork.JoinOrCreateRoom(room_name, options, TypedLobby.Default, ref error_msg))
        {
            CurrentRoomName = room_name;
            return true;
        }

        ShowError(error_msg, -1);        
        return false;
    }


    // --- Informative Override Callbacks ---
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room Successfully", this);
    }
    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("Joined Room Successfully", this);
    //}

    private void ShowError(string message_log, short error_num)
    {
        GameObject UIObj = GameObject.Find("NetworksUI");
        if (UIObj)
        {
            UIConnectionManager UI = UIObj.GetComponent<UIConnectionManager>();

            if (UI)
                UI.ShowErrorOnUI(message_log, error_num);
        }
    }

    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        ShowError(errorInfo.Info, -1);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowError("Error on Room Creation: " + message, returnCode);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowError("Error on Room Join: " + message, returnCode);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ShowError("Error on Room Join: " + message, returnCode);
    }
}