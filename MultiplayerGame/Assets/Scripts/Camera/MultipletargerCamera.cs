using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipletargerCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Vector3 offet;
    private Vector3 velocity;
    public float smoothTime = .05f;
    public float minZoom = 78f;
    public float maxZoom = 25f;
    public float zoomLimiter = 10f;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        Move();
        Zoom();
    }
    void Zoom()
    {
        //Debug.Log(getGreatestDistance());
        float newZoom = Mathf.Lerp(maxZoom, minZoom, getGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime*2);
    }
    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 NewPosition = centerPoint + offet;
        transform.position = Vector3.SmoothDamp(transform.position, NewPosition, ref velocity, smoothTime);
    }

    float getGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.magnitude;
    }
    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
