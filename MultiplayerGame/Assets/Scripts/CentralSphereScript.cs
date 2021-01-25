using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralSphereScript : MonoBehaviour
{
    private Timer m_CheckTimer;
    private List<Collider> CollidersInside = new List<Collider>();

    public List<Collider> GetCollidersInCenter()
    {
        if (CollidersInside.Count > 0)
            return CollidersInside;
        else
            return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_CheckTimer = GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_CheckTimer.ReadTime() > 1.5f)
        {
            // Include here a prameter for the colliders layer that shall be checking
            Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
            CollidersInside = new List<Collider>();

            for(int i = 0; i < colliders.Length; ++i)
            {
                if(colliders[i].gameObject.tag == "Player")
                    CollidersInside.Add(colliders[i]);
            }

            m_CheckTimer.RestartFromZero();
        }
    }
}
