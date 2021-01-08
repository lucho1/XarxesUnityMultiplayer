using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float startingTime = 0.0f;

    private float m_Time = 0.0f;
    private bool m_timerActive = true;

    // Start is called before the first frame update
    public void Start()
    {
        m_timerActive = true;
        m_Time = startingTime;
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
