using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectionManager : MonoBehaviour
{
    public GameObject ConnectionManagerObject;
    private ConnectionManager ConnectionManager;

    public GameObject ConnectionUI;
    public GameObject RoomUI;

    public Text Username;

    [SerializeField]
    private Text ErrorText;
    private Timer m_ErrorTextTime;

    private bool m_ChangeScreen = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_ErrorTextTime = GetComponent<Timer>();
        ConnectionManager = ConnectionManagerObject.GetComponent<ConnectionManager>();
    }

    private void Update()
    {
        if (!ConnectionManager)
            Debug.LogError("Connection Manager GameObject DOES NOT HAVE a ConnectionManager Script!");

        // If UIScreen should change & we are in a room, change to RoomUI
        bool hide_error = false;
        if(m_ChangeScreen && ConnectionManager.IsInRoom())
        {
            ConnectionManager.SetRoomName();
            SetRoomScreen();
            m_ChangeScreen = false;
            hide_error = true;
        }

        if (RoomUI.activeInHierarchy && !ConnectionManager.IsInRoom())
            Debug.LogError("WE ARE NOT IN A ROOM!");

        // If the error text is being shown, unactive & reset it after 3s OR when it must
        if ((ErrorText.IsActive() && m_ErrorTextTime.ReadTime() > 3.0f) || hide_error)
        {
            m_ErrorTextTime.Restart();
            m_ErrorTextTime.Stop();

            ErrorText.text = "Error Text";
            ErrorText.gameObject.SetActive(false);
        }
    }

    public void ShowErrorOnUI(string message_log, short error_num)
    {
        // Show & Log error message
        string error_message = message_log + " (#" + error_num + ")";

        ErrorText.text = error_message;
        ErrorText.gameObject.SetActive(true);
        m_ErrorTextTime.Start();

        Debug.Log(error_message, this);
    }

    public void SetUsername(Text name_text)
    {
        ConnectionManager.SetUsername(name_text.text);
    }

    private void SetRoomScreen()
    {
        ConnectionUI.SetActive(false);

        Text[] text_components = RoomUI.GetComponentsInChildren<Text>();
        foreach (Text text in text_components)
        {
            if (text.gameObject.name == "RoomNameText")
            {
                text.text = ConnectionManager.GetCurrentRoomName;
                break;
            }
        }

        RoomUI.SetActive(true);
    }


    // ---- MENU FUNCTIONS FOR BUTTONS ----
    public void JoinSelectedRoom()
    {
        if (ConnectionUI.activeInHierarchy)
        {
            if(string.IsNullOrEmpty(Username.text) || string.IsNullOrWhiteSpace(Username.text))
            {
                ShowErrorOnUI("A Valid Username is Required!", 0);
                return;
            }

            RoomListController rooms = ConnectionUI.GetComponentInChildren<RoomListController>();
            if(rooms.CurrentSelectedRoom == "")
            {
                ShowErrorOnUI("A Room must be selected!", 0);
                return;
            }

            if (rooms)
            {
                string room_name = rooms.CurrentSelectedRoom;
                if (ConnectionManager.JoinRoom(room_name))
                    m_ChangeScreen = true;
            }
            else
                Debug.LogError("Couldn't Find Rooms List Controller!");
        }
    }

    public void QuickStart()
    {
        if (ConnectionUI.activeInHierarchy)
        {
            if (string.IsNullOrEmpty(Username.text) || string.IsNullOrWhiteSpace(Username.text))
            {
                ShowErrorOnUI("A Valid Username is Required!", 0);
                return;
            }

            if (ConnectionManager.JoinRandomRoom())
                m_ChangeScreen = true;
        }
    }

    public void HostNewRoom(Text new_room_name)
    {
        if (ConnectionUI.activeInHierarchy)
        {
            if (string.IsNullOrEmpty(Username.text) || string.IsNullOrWhiteSpace(Username.text))
            {
                ShowErrorOnUI("A Valid Username is Required!", 0);
                return;
            }

            if (string.IsNullOrEmpty(new_room_name.text) || string.IsNullOrWhiteSpace(new_room_name.text))
            {
                ShowErrorOnUI("A Valid Room Name is Required!", 0);
                return;
            }

            if (ConnectionManager.CreateRoom(new_room_name.text))
                m_ChangeScreen = true;
        }
    }
}