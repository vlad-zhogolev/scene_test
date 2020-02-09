using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    Animator animator;

    [SerializeField]
    bool changeState = false;

    GameObject doorHand;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (changeState)
        {
            changeState = false;
            animator.SetBool("isOpen", !animator.GetBool("isOpen"));
        }
    }
}
