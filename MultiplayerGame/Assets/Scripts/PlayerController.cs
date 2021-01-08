using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float PlayerSpeed = 2.0f;
    [Range(0.0f, 1.0f)] public float PlayerAcceleration = 0.10f;

    private CharacterController m_CharacterController;
    private Animator m_Animator;

    private Vector3 m_CurrentSpeed = Vector3.zero;
    private Vector3 m_CurrentRotation = Vector3.zero;
    private Rigidbody m_RigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = gameObject.GetComponentInChildren<Animator>();
        m_RigidBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_RigidBody)
            m_RigidBody = gameObject.GetComponent<Rigidbody>();

        // If player has a Rigid Body, let's move!
        if (m_RigidBody)
        {
            if (Input.GetButtonDown("Shoot") || Input.GetAxis("Shoot") > 0) // This "GetAxis" will spawn a LOT of bullets, it's always entering on trigger press
                Debug.Log("PlayerShoot!");

            // Compute speed and apply acceleration
            Vector3 desired_speed = PlayerSpeed * new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
            m_CurrentSpeed += PlayerAcceleration * (desired_speed - m_CurrentSpeed);

            // Apply speed to position
            transform.position += m_CurrentSpeed * Time.deltaTime;

            // Compute & Apply Rotation to Rigid Body
            Vector3 desired_lookat_speed = new Vector3(-Input.GetAxis("RightVertical"), 0, Input.GetAxis("RightHorizontal"));
            m_CurrentRotation += (desired_lookat_speed - m_CurrentRotation) * Time.deltaTime * 10.0f;

            if (m_CurrentRotation != Vector3.zero)
                m_RigidBody.MoveRotation(Quaternion.LookRotation(m_CurrentRotation.normalized, Vector3.up));

            // Setup animation
            if (desired_speed != Vector3.zero)
                m_Animator.SetBool("Running", true);
            else
                m_Animator.SetBool("Running", false);
        }
        else
            Debug.LogError("Player Has Not RigidBody!!!!!");
    }
}
