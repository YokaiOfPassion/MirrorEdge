using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnCollision : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Female")
        {
            animator.SetBool("go",true);
        }
    }
}
