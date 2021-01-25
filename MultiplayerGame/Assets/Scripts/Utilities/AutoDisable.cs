using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float lifeTime = 2.0f;
    private float startTime;

    private void OnEnable()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= lifeTime)
            this.gameObject.SetActive(false);
    }
}
