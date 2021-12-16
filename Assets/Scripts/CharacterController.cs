using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private float movementSpeed = 3;

    private void FixedUpdate()
    {
        transform.Translate(ProcessJoystickInputs.NormalizedDirection * movementSpeed * Time.fixedDeltaTime, Space.Self);
    }
}
