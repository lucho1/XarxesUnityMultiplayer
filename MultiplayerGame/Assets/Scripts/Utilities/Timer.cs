using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float startingTime = 0.0f;
    public bool beginStarted = true;

    private float m_Time = 0.0f;
    private bool m_timerActive = true;

    // Start is called before the first frame update
    public void Start()
    {
        m_Time = startingTime;
        if (beginStarted)
            m_timerActive = true;
        else
            m_timerActive = false;
    }

    // Update is called once per frame
    public void Update()
    {
        if (m_timerActive == true)
            m_Time += Time.deltaTime;
    }

    public float ReadTime()
    {
        return m_Time;
    }

    public bool IsRunning()
    {
        return m_timerActive;
    }

    public void Stop()
    {
        m_timerActive = false;
    }

    public void RestartAndStop()
    {
        m_Time = startingTime;
        m_timerActive = false;
    }

    public void Play()
    {
        m_timerActive = true;
    }

    public void RestartFromZero()
    {
        m_Time = 0.0f;
        m_timerActive = true;
    }

    public void Restart()
    {
        m_Time = startingTime;
        m_timerActive = true;
    }
}
