using UnityEngine;
using UnityEngine.UI;

public class NetworkUIScript : MonoBehaviour
{
    [SerializeField]
    private ConnectionAPIScript ConnectionManager;    

    [SerializeField]
    private Text ConnectionText;

    [SerializeField]
    private Text WarnText;

    [SerializeField]
    private Text ErrorText;

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
        bool hide_error = false, hide_warn = false;

        // Show Connection Status
        if (ConnectionManager.IsConnectedAndReady())
            ConnectionText.GetComponent<Text>().text = "Connected";
        else
            ConnectionText.GetComponent<Text>().text = "Connecting...";

        // Hide Warn
        if (hide_warn || WarnText.gameObject.activeSelf && m_WarnTimer.ReadTime() > 3.0f)
        {
            m_WarnTimer.RestartAndStop();
            WarnText.gameObject.SetActive(false);
        }

        // Hide Error
        if (hide_error || ErrorText.gameObject.activeSelf && m_ErrorTimer.ReadTime() > 3.0f)
        {
            m_ErrorTimer.RestartAndStop();
            ErrorText.gameObject.SetActive(false);
        }
    }

    public void ShowError(string message_log, int error_num)
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

    // --- Room Creation, Join & Left Callbacks ---
    public void RoomCreated()
    {

    }

    public void RoomCreatedFailure()
    {

    }

    public void JoinedRoom()
    {

    }

    public void JoinedRoomFailed()
    {

    }

    public void RoomLeft()
    {

    }
}
