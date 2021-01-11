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


    // Start is called before the first frame update
    private void Start()
    {
        print("Connecting to Server");

        // This will allow us to use PhotonNetwork.LoadLevel() on the master and all clients in room will sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        SetUsername(Username);
        PhotonNetwork.GameVersion = GameVersion;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AddCallbackTarget(this.gameObject);

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
        if(!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby(TypedLobby.Default);
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
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string error_msg = "";
            if (PhotonNetwork.JoinRandomRoom(ref error_msg))
            {
                if (PhotonNetwork.InRoom)
                    CurrentRoomName = PhotonNetwork.CurrentRoom.Name;

                return true;
            }

            ShowError(error_msg, -1);
        }
        else
            ShowError("Still Not Prepared to JoinRoom, Wait", 0);

        return false;
    }

    public bool JoinRoom(string room_name)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string error_msg = "";
            if (PhotonNetwork.JoinRoom(room_name, ref error_msg))
            {
                CurrentRoomName = room_name;
                return true;
            }

            ShowError(error_msg, -1);
        }
        else
            ShowError("Still Not Prepared to JoinRoom, Wait", 0);

        return false;
    }

    public bool CreateRoom(string room_name)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string error_msg = "";
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)MaxPlayers;
            options.BroadcastPropsChangeToAll = true;

            if (PhotonNetwork.CreateRoom(room_name, ref error_msg, options, TypedLobby.Default))
            {
                CurrentRoomName = room_name;
                return true;
            }

            ShowError(error_msg, -1);
        }
        else
            ShowError("Still Not Prepared to JoinRoom, Wait", 0);

        return false;
    }

    public bool JoinOrCreateRoom(string room_name)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string error_msg = "";
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)MaxPlayers;
            options.BroadcastPropsChangeToAll = true;

            if (PhotonNetwork.JoinOrCreateRoom(room_name, options, TypedLobby.Default, ref error_msg))
            {
                CurrentRoomName = room_name;
                return true;
            }

            ShowError(error_msg, -1);
        }
        else
            ShowError("Still Not Prepared to JoinRoom, Wait", 0);

        return false;
    }

    public void LeaveFromRoom()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LeaveRoom(true);
            
            if(!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    public void LoadScene(int scene_index)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // Room can't be joined (or we should allow it?)
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(scene_index);
        }
        else
            ShowError("Only the Room Host can Start the Game!", 0);
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