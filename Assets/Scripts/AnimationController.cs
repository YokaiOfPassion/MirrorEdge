using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private float normalizedVelocity;
    private int velocityHash = Animator.StringToHash("Velocity");

    // Update is called once per frame
    void Update()
    {
        normalizedVelocity = ProcessJoystickInputs.NormalizedDirection.magnitude;
        animator.SetFloat(velocityHash, normalizedVelocity);
    }
}
