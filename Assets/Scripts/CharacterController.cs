using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle, Running, Airborne, Falling }

public class CharacterController : MonoBehaviour
{
    [SerializeField, Range(1, 20)] private float movementSpeed = 10;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private KeyCode jumpKey = KeyCode.Joystick1Button0;
    [SerializeField, Range(1, 20)] private float jumpForce = 5;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField, Range(0f, 1f)] private float coyoteTimeDuration = 0.5f;

    private bool touchingGround = true;
    private bool coyoteTimeIsUp;
    private PlayerState previousState;
    private PlayerState currentState;

    private bool coyoteTimeCoroutineIsCalled;

    [Header("DEBUG")]
    public bool checkGround;

    private void Start()
    {
        currentState = PlayerState.Idle;
    }

    private void FixedUpdate()
    {
        SetPlayerState(PlayerState.Idle);

        transform.Translate(ProcessJoystickInputs.NormalizedDirection * movementSpeed * Time.fixedDeltaTime, Space.Self);
        touchingGround = Physics.Raycast(transform.position + new Vector3(0f, 0.1f, 0f), Vector3.down, 0.2f, groundLayerMask);
        // Debug.DrawRay(transform.position, Vector3.down * 0.2f, Color.red, 3f);

        if (touchingGround)
        {
            coyoteTimeIsUp = false; 
        }

        if (!coyoteTimeIsUp)
        {
            if (ProcessJoystickInputs.NormalizedDirection.magnitude > 0f)
            {
                SetPlayerState(PlayerState.Running);
            }

            if (Input.GetKeyDown(jumpKey))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                SetPlayerState(PlayerState.Airborne);
            }
        }

        if (coyoteTimeIsUp && currentState != PlayerState.Airborne)
        {
            SetPlayerState(PlayerState.Falling); // can't jump while falling
        }


        if (!touchingGround && !coyoteTimeCoroutineIsCalled)
        {
            Debug.Log("calling coyote time");
            coyoteTimeCoroutineIsCalled = true;
            StartCoroutine(nameof(SetCoyoteTime));
        }

        // DEBUG
        if (!touchingGround && checkGround)
        {
            Time.timeScale = 0f;
            Debug.DrawRay(transform.position - new Vector3(0f, 0.1f, 0f), Vector3.down * 0.3f, Color.yellow, 3f);
        }
    }

    private PlayerState SetPlayerState(PlayerState newCurrentState)
    {
        previousState = currentState;
        currentState = newCurrentState;
        return currentState;
    }

    private IEnumerator SetCoyoteTime()
    {
        yield return new WaitForSeconds(coyoteTimeDuration);
        Debug.Log("end of coyote time");
        coyoteTimeIsUp = true;
        coyoteTimeCoroutineIsCalled = false;
    }
}
