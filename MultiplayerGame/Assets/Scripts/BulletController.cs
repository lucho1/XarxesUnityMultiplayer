using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float Speed = 40.0f;
    private Timer m_BulletLife;

    // Start is called before the first frame update
    void Start()
    {
        m_BulletLife = GetComponent<Timer>();
        m_BulletLife.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_BulletLife.ReadTime() > 4.0f)
            Destroy(gameObject);
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
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
