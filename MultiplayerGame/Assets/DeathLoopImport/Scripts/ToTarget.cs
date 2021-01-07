using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTarget : MonoBehaviour
{
    Animation anim;
    public Transform Target;
    public bool goTarget;
    public Button_Controller but_Control;
    private Vector3 velocity;
    public float smoothTime = .05f;
    bool Done = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        goTarget = false;
        Done = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Done == false) {
            if (goTarget)
            {
                anim.Stop();
                transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref velocity, smoothTime / 2);
                transform.rotation = Quaternion.Lerp(transform.rotation, Target.rotation, smoothTime * 2);
            }
            if (Mathf.Approximately(Target.position.magnitude - transform.position.magnitude, 0)){
                
                but_Control.OnButton = true;
            }
        }
    }

    public void SetGoTarget()
    {
        goTarget = true;
    }
}
