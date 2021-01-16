using UnityEngine;
using UnityEngine.UI;

public class NetworkUIScript : MonoBehaviour
{
    // --- Connection Manager ---
    [SerializeField]
    private ConnectionAPIScript ConnectionManager;    

    // --- Log Texts ---
    [SerializeField]
    private Text ConnectionText;

    [SerializeField]
    private Text WarnText;

    [SerializeField]
    private Text ErrorText;

    private bool m_HideLogTexts = false;

    // --- UI Objects ---
    [SerializeField]
    private GameObject RoomUI;

    [SerializeField]
    private Text RoomName_UIText;

    [SerializeField]
    private GameObject LobbyUI;

    // --- Timers ---
    private Timer m_WarnTimer;
    private Timer m_ErrorTimer;


    // Start is called before the first frame update
    private void Start()
    {
        Timer[] timers = GetComponents<Timer>();
        m_WarnTimer = timers[0];
        m_ErrorTimer = timers[1];
    }

    // Update is called once per frame
    void Update()
    {
        // Show Connection Status
        if (ConnectionManager.IsConnectedAndReady())
            ConnectionText.GetComponent<Text>().text = "Connected";
        else
            ConnectionText.GetComponent<Text>().text = "Connecting...";

        // Hide Warn
        if (m_HideLogTexts || WarnText.gameObject.activeSelf && m_WarnTimer.ReadTime() > 3.0f)
        {
            m_WarnTimer.RestartAndStop();
            WarnText.gameObject.SetActive(false);
        }

        // Hide Error
        if (m_HideLogTexts || ErrorText.gameObject.activeSelf && m_ErrorTimer.ReadTime() > 3.0f)
        {
            m_ErrorTimer.RestartAndStop();
            ErrorText.gameObject.SetActive(false);
        }

        m_HideLogTexts = false;
    }


    // --- UI Methods ---
    private void ChangeScreen(bool enter_room)
    {
        if(enter_room)
        {
            //m_HideLogTexts = true;
            RoomName_UIText.text = ConnectionManager.GetRoomName();
            LobbyUI.SetActive(false);
            RoomUI.SetActive(true);
        }
        else
        {
            //m_HideLogTexts = true;
            RoomUI.SetActive(false);
            LobbyUI.SetActive(true);
        }
    }
    

    // --- Errors ---
    public void ShowError(string message_log, int error_num = 0)
    {
        // Show & Log error message
        if(error_num != 0)
            message_log += " (#" + error_num + ")";

        m_ErrorTimer.Start();
        ErrorText.text = message_log;
        ErrorText.gameObject.SetActive(true);

        Debug.LogError(message_log, this);
    }

    public void ShowWarn(string message_log)
    {
        // Show & Log warn message
        m_WarnTimer.Start();
        WarnText.text = message_log;
        WarnText.gameObject.SetActive(true);

        Debug.Log(message_log, this);
    }


    // --- Room Callbacks ---
    public void RoomCreated()
    {
        ChangeScreen(true);
        ShowWarn("Created Room Successfully");
    }

    public void RoomCreatedFailure(string message_log, int error_num)
    {
        ShowError(message_log, error_num);
    }

    public void JoinedRoom()
    {
        ChangeScreen(true);
        ShowWarn("Joined Room Successfully");
    }

    public void JoinedRoomFailed(string message_log, int error_num)
    {
        ShowError(message_log, error_num);
    }

    public void RoomLeft()
    {
        ChangeScreen(false);
        ShowWarn("Room Left Successfully");
    }

    // --- Player Callbacks ---
    public void PlayerJoined(string player_name)
    {
        ShowWarn(player_name + " Joined the Room!");
    }

    public void PlayerLeft(string player_name)
    {
        ShowWarn(player_name + " Left the Room!");
    }
}
