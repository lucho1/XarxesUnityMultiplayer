using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject FirePosition;
    public GameObject BulletPrefab;
    public float FireRate = 1.0f;
    public AudioClip ShootSound;

    private Timer m_WeaponShootTimer;
    private AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        m_WeaponShootTimer = GetComponent<Timer>();
        m_AudioSource = GetComponentInChildren<AudioSource>();
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

            m_WeaponShootTimer.Start();
            Instantiate(BulletPrefab, FirePosition.transform.position, transform.rotation);
        }
    }
}
