using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { Idle, Running, Airborne, Falling, Climbing }

public class CharacterController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private LayerMask groundLayerMask;
    private Vector3 direction; 

    [Header("Run")]
    [SerializeField, Range(1, 20)] private float movementSpeed = 10;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private KeyCode jumpKey = KeyCode.Joystick1Button0;

    [Header("Jump")]
    [SerializeField, Range(1, 20)] private float jumpForce = 5;
    [SerializeField, Range(0f, 1f)] private float coyoteTimeDuration = 0.5f;

    private bool touchingGround = true;
    private bool coyoteTimeIsUp;
    private PlayerState previousState;
    private PlayerState currentState;

    private bool coyoteTimeCoroutineIsCalled;

    [Header("Climb")]
    [SerializeField] private LayerMask climbableLayerMask;
    [SerializeField, Range(0.5f, 10)] private float climbSpeed = 3;
    private bool canClimb;
    public static bool isClimbing;
    private Collider climbableCollider;

    [Header("Vault")]
    [SerializeField, Range(1f, 3f)] private float momentumBoostMultiplier = 1.5f;

    [Header("-- DEBUG --")]
    public bool checkGround;

    private void Start()
    {
        currentState = PlayerState.Idle;
    }

    private void OnTriggerEnter(Collider other)
    {
        climbableCollider = other;
        if (Mathf.Pow(2, other.gameObject.layer) == climbableLayerMask)
        {
            transform.position = new Vector3(climbableCollider.bounds.center.x, transform.position.y, transform.position.z); ; // align with object
            transform.LookAt(new Vector3(climbableCollider.bounds.center.x, transform.position.y, transform.position.z + 3f));
            canClimb = true;
            rb.useGravity = false;
            // SetPlayerState(PlayerState.Climbing);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == climbableLayerMask)
        {
            rb.useGravity = true;
            // SetPlayerState(PlayerState.Idle);
        }
    }

    private void FixedUpdate()
    {
        SetPlayerState(PlayerState.Idle);
        direction = new Vector3(ProcessJoystickInputs.NormalizedDirection.x * (canClimb ? 0f : 1f), 
                                ProcessJoystickInputs.NormalizedDirection.y * (canClimb ? 1f : 0f), 
                                ProcessJoystickInputs.NormalizedDirection.z * (canClimb ? 0f : 1f));

        transform.Translate(direction * (canClimb ? climbSpeed : movementSpeed) * Time.fixedDeltaTime, Space.Self);
        touchingGround = Physics.Raycast(transform.position + new Vector3(0f, 0.1f, 0f), Vector3.down, 0.2f, groundLayerMask);
        // Debug.DrawRay(transform.position, Vector3.down * 0.2f, Color.red, 3f);

        canClimb = ProcessJoystickInputs.NormalizedDirection.x > 0f;
        isClimbing = canClimb && ProcessJoystickInputs.NormalizedDirection.y > 0f;

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
