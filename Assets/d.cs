using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class d : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("d");
        }
    }
}
