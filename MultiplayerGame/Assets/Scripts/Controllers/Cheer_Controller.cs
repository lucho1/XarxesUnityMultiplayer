using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheer_Controller : MonoBehaviour
{
    Animator animator;
    float counter;
    public float TimeToChange_Anim=100;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        counter = TimeToChange_Anim;
    }

    // Update is called once per frame
    void Update()
    {

        if (counter >= TimeToChange_Anim)
        {
            int i = Random.Range(1, 4);
            if (i == 1)
            {
                animator.SetBool("Cheer", true);
                animator.SetBool("Idle_Sit", false);
                animator.SetBool("Pump", false);
            }
            else if (i == 2)
            {
                animator.SetBool("Cheer", false);
                animator.SetBool("Idle_Sit", false);
                animator.SetBool("Pump", true);
            }
            else if (i == 3)
            {
                animator.SetBool("Cheer", false);
                animator.SetBool("Idle_Sit", true);
                animator.SetBool("Pump", false);
            }
            counter = 0;
        }
        counter+=Time.deltaTime;
    }
}
