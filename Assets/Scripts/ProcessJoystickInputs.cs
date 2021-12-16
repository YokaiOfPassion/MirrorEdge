using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessJoystickInputs : MonoBehaviour
{
    public static Vector3 NormalizedDirection { get; private set; }
    public static float turnValue { get; private set; }

    void Update()
    {
        NormalizedDirection = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Vertical")).normalized;
        turnValue = Input.GetAxis("Turn");
    }
}
