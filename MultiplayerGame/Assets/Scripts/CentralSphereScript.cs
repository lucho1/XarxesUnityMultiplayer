using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralSphereScript : MonoBehaviour
{
    private Timer m_CheckTimer;
    private Collider[] CollidersInside;

    public Collider[] GetCollidersInCenter()
    {
        return CollidersInside;
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

            int inner_colliders_index = 0;
            for(int i = 0; i < colliders.Length; ++i)
            {
                if(colliders[i].gameObject.tag == "Player")
                {
                    CollidersInside[inner_colliders_index] = colliders[i];
                    ++inner_colliders_index;
                }                    
            }

            m_CheckTimer.RestartFromZero();
        }
    }
}
