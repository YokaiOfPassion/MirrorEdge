using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnCollision : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public static Action<bool> OnPlayerColliding;
    public bool final; 

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Female")
        {
            animator.SetBool("go",true);
            OnPlayerColliding(final);
        }
    }
}
