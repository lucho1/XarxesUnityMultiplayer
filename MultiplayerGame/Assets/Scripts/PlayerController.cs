using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, IPunInstantiateMagicCallback
{
    public float PlayerSpeed = 2.0f;
    [Range(0.0f, 1.0f)] public float PlayerAcceleration = 0.10f;
    public bool NetworkMode = true;


    private PhotonView m_PhotonView;
    private CharacterController m_CharacterController;
    private Animator m_Animator;

    private Vector3 m_CurrentSpeed = Vector3.zero;
    private Vector3 m_CurrentRotation = Vector3.zero;
    private Rigidbody m_RigidBody;

    private Vector3 m_LastMousePos;

    // Start is called before the first frame update
    void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
        if (NetworkMode && m_PhotonView && !m_PhotonView.IsMine)
            this.enabled = false;

        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
        m_RigidBody = gameObject.GetComponent<Rigidbody>();

        m_LastMousePos = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_RigidBody)
            m_RigidBody = gameObject.GetComponent<Rigidbody>();

        // If player has a Rigid Body, let's move!
        if (m_RigidBody)
        {
            // --- ROTATION ---
            // Decide to rotate with mouse (if moved) or with gamepad
            Quaternion rotation = Quaternion.identity;
            if (Input.mousePosition != m_LastMousePos)
            {
                m_LastMousePos = Input.mousePosition;

                Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                rotation = Quaternion.AngleAxis(-angle, Vector3.up);
            }
            else
            {
                Vector3 desired_lookat_speed = new Vector3(-Input.GetAxis("RightVertical"), 0, Input.GetAxis("RightHorizontal"));
                m_CurrentRotation += (desired_lookat_speed - m_CurrentRotation) * Time.deltaTime * 10.0f;

                if (m_CurrentRotation != Vector3.zero)
                    rotation = Quaternion.LookRotation(m_CurrentRotation.normalized, Vector3.up);
            }

            // Apply Rotation to Rigid Body
            if (rotation != Quaternion.identity)
                m_RigidBody.MoveRotation(rotation);


            // --- MOVEMENT ---
            // Compute speed and apply acceleration
            Vector3 desired_speed = PlayerSpeed * new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            m_CurrentSpeed += PlayerAcceleration * (desired_speed - m_CurrentSpeed);

            // Apply speed to position
            transform.position += m_CurrentSpeed * Time.deltaTime;


            // --- ANIMATION ---
            if (desired_speed != Vector3.zero)
                m_Animator.SetBool("Running", true);
            else
                m_Animator.SetBool("Running", false);
        }
        else
            Debug.LogError("Player Has Not RigidBody!!!!!");
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] instantiatonData = info.photonView.InstantiationData;
        this.gameObject.layer = (int) instantiatonData[0];
        if (m_PhotonView.IsMine)
            info.Sender.TagObject = this.gameObject;
    }
}