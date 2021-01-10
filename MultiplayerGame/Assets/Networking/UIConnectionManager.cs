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

    [SerializeField]
    private Text ErrorText;
    private Timer m_ErrorTextTime;
    private Timer m_ChangeScreenTime;

    private bool m_ChangeScreen = false;

    // Start is called before the first frame update
    private void Start()
    {
        m_ErrorTextTime = GetComponent<Timer>();

        Timer[] timers = GetComponents<Timer>();
        m_ErrorTextTime = timers[0];
        m_ChangeScreenTime = timers[1];

        ConnectionManager = ConnectionManagerObject.GetComponent<ConnectionManager>();
    }

    private void Update()
    {
        if (!ConnectionManager)
            Debug.LogError("Connection Manager GameObject DOES NOT HAVE a ConnectionManager Script!");

        if(m_ChangeScreen && m_ChangeScreenTime.ReadTime() > 0.5f)
        {
            m_ChangeScreen = false;
            m_ChangeScreenTime.Restart();
            m_ChangeScreenTime.Stop();
            
            SetRoomScreen();
        }

        if (ErrorText.IsActive() && m_ErrorTextTime.ReadTime() > 2.0f)
        {
            m_ErrorTextTime.Restart();
            m_ErrorTextTime.Stop();

            ErrorText.text = "Error Text";
            ErrorText.gameObject.SetActive(false);
        }
    }

    public void ShowErrorOnUI(string message_log, short error_num)
    {
        // Check & Set for Change Screen
        m_ChangeScreen = false;
        m_ChangeScreenTime.Restart();
        m_ChangeScreenTime.Stop();

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

        Text[] text_components = RoomUI.GetComponentsInChildren<Text>();//.text = ConnectionManager.GetCurrentRoomName;
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

    private void SetConnectionScreen()
    {
        ConnectionUI.SetActive(true);
        RoomUI.SetActive(false);
    }

    public void JoinSelectedRoom()
    {
        if (ConnectionUI.activeInHierarchy)
        {
            RoomListController rooms = ConnectionUI.GetComponentInChildren<RoomListController>();
            if (rooms)
            {
                string room_name = rooms.CurrentSelectedRoom;
                if (ConnectionManager.JoinRoom(room_name))
                {
                    m_ChangeScreen = true;
                    m_ChangeScreenTime.Start();
                }
            }
            else
                Debug.LogError("Couldn't Find Rooms List Controller!");
        }
    }

    public void QuickStart()
    {
        if (ConnectionUI.activeInHierarchy)
        {
            if (ConnectionManager.JoinRandomRoom())
            {
                m_ChangeScreen = true;
                m_ChangeScreenTime.Start();
            }
        }
    }

    public void HostNewRoom(Text new_room_name)
    {
        if (ConnectionUI.activeInHierarchy)
        {
            if (ConnectionManager.CreateRoom(new_room_name.text))
            {
                m_ChangeScreen = true;
                m_ChangeScreenTime.Start();
            }
        }
    }
}