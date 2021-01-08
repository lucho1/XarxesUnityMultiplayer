using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTarget : MonoBehaviour
{
    public Transform Target;
    public bool MoveToTarget = false;
    public Button_Controller ButtonController;
    public float SmoothTime = 0.05f;

    private Vector3 m_IntialPosition = new Vector3(18.09f, 17.88f, 16.68f);
    private Quaternion m_InitialRotation;
    private Vector3 m_Velocity;
    private Animation m_Animation;

    private bool m_MoveBackwards = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //m_IntialPosition = gameObject.transform.position;
        //m_InitialRotation = gameObject.transform.rotation;

        Quaternion quat = Quaternion.Euler(13.345f, -128.817f, -4.594f);
        m_InitialRotation = quat;

        m_Animation = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveToTarget && !m_MoveBackwards)
        {
            m_Animation.Stop();
            transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref m_Velocity, SmoothTime / 2);
            transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation, SmoothTime * 2);
        }

        if (Mathf.Approximately(Target.position.magnitude - transform.position.magnitude, 0) && !m_MoveBackwards)
            ButtonController.OnButton = true;

        if(m_MoveBackwards)
        {
            transform.position = Vector3.SmoothDamp(transform.position, m_IntialPosition, ref m_Velocity, SmoothTime / 2);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_InitialRotation, SmoothTime * 4);

            if (Mathf.Approximately(m_IntialPosition.magnitude - transform.position.magnitude, 0))
            {
                m_MoveBackwards = false;
                m_Animation.Play();
            }
        }
    }

    public void SetMoveToTarget()
    {
        MoveToTarget = true;
    }

    public void ResetCameraMovement()
    {
        MoveToTarget = false;
        m_MoveBackwards = true;
        ButtonController.OnButton = false;
    }
}
