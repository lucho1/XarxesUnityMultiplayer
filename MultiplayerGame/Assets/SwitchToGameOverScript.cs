using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToGameOverScript : MonoBehaviour
{
    private Timer m_Timer;
    public NetworkManager NetworkManager;

    // Start is called before the first frame update
    void Start()
    {
        m_Timer = GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Timer.ReadTime() > 6.0f)
            NetworkManager.LoadGameOverScreen();
    }
}
