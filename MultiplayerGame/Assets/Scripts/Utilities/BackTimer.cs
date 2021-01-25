using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackTimer : MonoBehaviour
{
    // --- Simple Backwards Timer class ---
    // It has a start time, a time in which it has to begin from
    // To make it automatically start from game's beginning, check the Running bool to true
    // To make it start from code, call Begin() whenever necessary
    // To see if it's running just get this Running bool
    public float StartTime = 10.0f;
    public bool Running = false;
    public bool Finished = false;

    private float m_TimeRemaining;

    // Start is called before the first frame update
    private void Start()
    {
        if (Running)
            Begin();
    }

    // Update is called once per frame
    private void Update()
    {
        if(Running)
        {
            if (m_TimeRemaining > 0.0f)
                m_TimeRemaining -= Time.deltaTime;
            else
            {
                m_TimeRemaining = 0.0f;
                Running = false;
                Finished = true;
            }
        }
    }

    public void Begin()
    {
        Running = true;
        Finished = false;
        m_TimeRemaining = StartTime;
    }

    // --- Getters ---
    public int GetMinutes()
    {
        return Mathf.FloorToInt((m_TimeRemaining + 1) / 60.0f);
    }

    public int GetSeconds()
    {
        return Mathf.FloorToInt((m_TimeRemaining + 1) % 60.0f);
    }

    public int GetTimeLeftInSeconds()
    {
        return Mathf.FloorToInt(m_TimeRemaining);
    }

    public string GetTimeString()
    {
        return string.Format("{0:00}:{1:00}", GetMinutes(), GetSeconds());
    }
}
