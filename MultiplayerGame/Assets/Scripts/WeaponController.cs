using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponController : MonoBehaviour
{
    public GameObject FirePosition;
    public GameObject BulletPrefab;
    public float FireRate = 1.0f;
    public AudioClip ShootSound;
    public bool NetworkMode = true;

    private Timer m_WeaponShootTimer;
    private AudioSource m_AudioSource;
    private PhotonView m_PhotonView;

    // Start is called before the first frame update
    void Start()
    {
        m_WeaponShootTimer = GetComponent<Timer>();
        m_AudioSource = GetComponentInChildren<AudioSource>();

        m_PhotonView = GetComponent<PhotonView>();
        if (NetworkMode && m_PhotonView && !m_PhotonView.IsMine)
            this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_AudioSource || !m_WeaponShootTimer)
        {
            Debug.LogError("Player couldn't find the Audio Source or the Timer Script!");
            return;
        }

        if (!FirePosition || !BulletPrefab || !ShootSound)
        {
            Debug.LogError("Player has not setted the FirePosition GO or the ShootSound Audio or the BulletPrefab!");
            return;
        }

        if (m_WeaponShootTimer.ReadTime() > FireRate && (Input.GetButtonDown("Shoot") || Input.GetAxis("Shoot") > 0)) // This will spawn a LOT of bullets at the FireRate time, it's always entering on trigger/button press! (Can be changed!)
        {
            m_AudioSource.clip = ShootSound;
            m_AudioSource.Play();
            if (NetworkMode)
                m_PhotonView.RPC("NetworkPlayShootingSound", RpcTarget.Others);

            m_WeaponShootTimer.Start();
            Instantiate(BulletPrefab, FirePosition.transform.position, transform.rotation);
        }
    }

    [PunRPC]
    void NetworkPlayShootingSound(PhotonMessageInfo info) {
        m_AudioSource.clip = ShootSound;
        m_AudioSource.Play();
    }
}
