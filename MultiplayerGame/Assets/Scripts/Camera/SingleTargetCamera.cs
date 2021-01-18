using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetCamera : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset;
    private Vector3 m_velocity;
    public float SmoothTime = .05f;
    public float Zoom = 78f;
    public bool FollowTarget = false;
    private Camera m_camera;

    void Start ()
    {
        m_camera = GetComponent<Camera>();
        if (m_camera)
            m_camera.fieldOfView = Zoom;
    }

    void LateUpdate()
    {
        if (!FollowTarget)
            return;

        Move();
    }
    void Move()
    {
        Vector3 NewPosition = Target.position + Offset;
        transform.position = Vector3.SmoothDamp(transform.position, NewPosition, ref m_velocity, SmoothTime);
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
    }


}
