using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessJoystickInputs : MonoBehaviour
{
    public static Vector3 NormalizedDirection { get; private set; }

    void Update()
    {
        NormalizedDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
    }
}
