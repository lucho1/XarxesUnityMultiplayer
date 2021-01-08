using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct WeaponStats
{
    public float fire_rate;
}

public class WeaponController : MonoBehaviour
{
    public GameObject firePositionGO;
    public GameObject bullet_prefav;
    private Timer weaponShotTime;
    public float fireRate = 1.0f;
    public AudioClip shootFX;
    private AudioSource Audiosrc;

    [Range (1,2)]public int controllerNumber;

    private string fireButton;

    WeaponStats current_weapon_stats;

    public int layer;

    // Start is called before the first frame update
    void Start()
    {
        fireButton = "FireButton" + controllerNumber.ToString();
        weaponShotTime = gameObject.GetComponent<Timer>();
        weaponShotTime.Start();
        Audiosrc = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(fireButton) > 0 && weaponShotTime.ReadTime() > fireRate)
        {
            Audiosrc.clip = shootFX;
            Audiosrc.Play();

            weaponShotTime.Start();

            Transform fire_position_transform = null;

            foreach (Transform t in gameObject.transform)
            {
                if (t.tag == "FirePosition")
                {
                    fire_position_transform = t;
                }
            }

            Vector3 fire_position = firePositionGO.transform.position;


            GameObject newBullet = Instantiate(bullet_prefav, fire_position, transform.rotation);
            newBullet.layer = layer;

            //gameObject.GetComponent<PlayerDataController>().saved_data_temp.AddShotTimeStamp(Time.time - FindObjectOfType<RoundManager>().last_round_start_timestamp,this.gameObject); ;
        }
    }
}
