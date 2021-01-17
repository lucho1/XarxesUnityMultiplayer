using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour
{
    public float Speed = 40.0f;
    private bool m_UseNetworking = false;
    private PhotonView m_PhotonView;
    private Timer m_BulletLife;

    // Start is called before the first frame update
    void Start()
    {
        m_PhotonView = gameObject.GetPhotonView();
        if (m_UseNetworking && m_PhotonView && !m_PhotonView.IsMine)
            this.enabled = false;

        m_BulletLife = GetComponent<Timer>();
        m_BulletLife.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_BulletLife.ReadTime() > 4.0f) {
            if (m_UseNetworking)
                PhotonNetwork.Destroy(m_PhotonView);
            else
                Destroy(gameObject);
        }
        else
        {
            Vector3 pos = transform.position + transform.forward * Speed * Time.deltaTime;
            transform.SetPositionAndRotation(pos, transform.rotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            if (m_UseNetworking)
                PhotonNetwork.Destroy(m_PhotonView);
            else
                Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            if (m_UseNetworking)
                PhotonNetwork.Destroy(m_PhotonView);
            else
                Destroy(gameObject);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) 
    {
        m_UseNetworking = true;
    }
}
