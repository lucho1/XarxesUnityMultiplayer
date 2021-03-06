﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    public float SmoothTime = .05f;
    public float Zoom = 78f;
    public bool FollowTarget = false;

    private Vector3 m_velocity;
    private Camera m_camera;
    private Vector3 m_originalPosition;

    void Start ()
    {
        m_originalPosition = transform.position;
        m_camera = GetComponent<Camera>();
        if (m_camera)
            m_camera.fieldOfView = Zoom;
    }

    void LateUpdate()
    {
        #if UNITY_EDITOR // this code is for adjusting the camera zoom while editing
        if (Zoom != m_camera.fieldOfView)
            m_camera.fieldOfView = Zoom;
        #endif
            
        if (!FollowTarget)
            return;

        Move();
    }
    void Move()
    {
        Vector3 NewPosition = Target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, NewPosition, ref m_velocity, SmoothTime);
    }

    public void SetPlayerTarget (PlayerController target)
    {
        if (target == null)
            return;
        Target = target.gameObject.transform;
        FollowTarget = true;
        target.PlayerDead.AddListener(StopFollowing);
        target.PlayerRespawn.AddListener(StartFollowing);
    }

    public void SetTarget (GameObject target) {
        if (target == null)
            return;
        Target = target.GetComponent<Transform>();
        if (Target != null)
            FollowTarget = true;
    }

    public void SetTarget (Transform target) {
        if (target == null)
            return;
        Target = target;
        FollowTarget = true;
    }

    public void RemoveTarget () {
        Target = null;
        FollowTarget = false;
        transform.position = m_originalPosition;
    }

    public void StopFollowing() {
        FollowTarget = false;
        transform.position = m_originalPosition;
    }

    public void StartFollowing() {
        FollowTarget = true;
    }


}
