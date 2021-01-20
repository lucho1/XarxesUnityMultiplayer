using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour, IPunInstantiateMagicCallback 
{
    public float Speed = 40.0f;
    public float LifeTime = 2.0f;
    [TagSelector]
    public string[] PlayerTags;
    private bool m_UseNetworking = false;
    private PhotonView m_PhotonView;
    private Timer m_BulletLife;

    // Start is called before the first frame update
    void Awake()
    {
        m_PhotonView = gameObject.GetPhotonView();

        m_BulletLife = GetComponent<Timer>();
        m_BulletLife.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_BulletLife.ReadTime() >= LifeTime) {
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
        bool isPlayer = false;
        foreach(string tag in PlayerTags) 
        {
            if (other.tag == tag)
                isPlayer = true;
        }

        if (!isPlayer)
        {
            if (m_UseNetworking)
                PhotonNetwork.Destroy(m_PhotonView);
            else
                Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool isPlayer = false;
        foreach(string tag in PlayerTags) 
        {
            if (collision.collider.tag == tag)
                isPlayer = true;
        }

        if (!isPlayer)
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
        this.gameObject.layer = (int)info.photonView.InstantiationData[0];
        if (!m_PhotonView.IsMine)
            this.enabled = false;
    }
}
