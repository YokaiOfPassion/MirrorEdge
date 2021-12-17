using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class CheckpointManager : MonoBehaviour
{
    private List<Transform> checkpointsList = new List<Transform>(); 

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            checkpointsList.Add(transform.GetChild(i));
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CharacterController.playerTransform.position = checkpointsList[0].position;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            CharacterController.playerTransform.position = checkpointsList[1].position;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            CharacterController.playerTransform.position = checkpointsList[2].position;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            CharacterController.playerTransform.position = checkpointsList[3].position;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            CharacterController.playerTransform.position = checkpointsList[4].position;
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            CharacterController.playerTransform.position = checkpointsList[5].position;
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            CharacterController.playerTransform.position = checkpointsList[6].position;
        }
    }
}
