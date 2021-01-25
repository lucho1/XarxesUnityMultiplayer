using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMovement : MonoBehaviour
{
    public bool MoveToTarget = false;
    public bool Arrived = false;
    
    [SerializeField]
    public Transform Target;

    [SerializeField]
    private float SmoothTime = 0.05f;

    private Vector3 m_Velocity;
    private BackTimer m_FinishTimer;
    private bool m_ResetTimer = true;


    // Start is called before the first frame update
    void Start()
    {
        m_FinishTimer = GetComponent<BackTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveToTarget)
        {
            GetComponent<Camera>().fieldOfView = 60;
            transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref m_Velocity, SmoothTime * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, (SmoothTime/1000.0f) * Time.deltaTime);
        }

        float cam_arrived = Target.position.magnitude - transform.position.magnitude;
        if (Mathf.Abs(cam_arrived) < 0.2f)
        {
            if (m_FinishTimer.Finished)
            {
                Arrived = true;
                m_ResetTimer = false;
            }
            
            if(!m_FinishTimer.Running && m_ResetTimer)
                m_FinishTimer.Begin();
        }
    }
}
