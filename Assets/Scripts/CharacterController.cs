using System.Collections;
using UnityEngine;

public enum PlayerState { Idle, Running, Airborne, Falling, Climbing }

public class CharacterController : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField, Range(0.5f, 2f)] private float turnSpeedMultiplier = 0.8f;
    private Vector3 direction;
    private float turnSpeed;
    private Collider detectedCollider;

    [Header("Run")]
    [SerializeField, Range(1, 20)] private float movementSpeed = 10;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private KeyCode jumpKey = KeyCode.Joystick1Button0;

    [Header("Jump")]
    [SerializeField, Range(1, 20)] private float jumpForce = 5;
    [SerializeField, Range(0f, 1f)] private float coyoteTimeDuration = 0.5f;

    private bool canJump;
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

    [Header("Vault")]
    [SerializeField] private LayerMask vaultableLayerMask;
    [SerializeField, Range(1f, 3f)] private float momentumBoostMultiplier = 1.5f;
    [SerializeField, Range(1f, 5f)] private float boostDuration = 3f;
    private float initialMovementSpeed;
    private bool canVault;

    [Header("-- DEBUG --")]
    public bool checkGround;

    private void Start()
    {
        currentState = PlayerState.Idle;
        initialMovementSpeed = movementSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        detectedCollider = other;
        if (Mathf.Pow(2, other.gameObject.layer) == climbableLayerMask)
        {
            transform.forward = other.transform.forward;
            canClimb = true;
            rb.useGravity = false;
            SetPlayerState(PlayerState.Climbing);
        }
        else if (Mathf.Pow(2, other.gameObject.layer) == vaultableLayerMask)
        {
            canVault = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Mathf.Pow(2, other.gameObject.layer) == climbableLayerMask)
        {
            canClimb = false;
            rb.useGravity = true;
        }
        else if (Mathf.Pow(2, other.gameObject.layer) == vaultableLayerMask)
        {
            canVault = false;
        }
    }

    private void FixedUpdate()
    {
        if (touchingGround)
        {
            canJump = currentState == PlayerState.Idle || currentState == PlayerState.Running;
        }

        turnSpeed = 360f * turnSpeedMultiplier;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + (ProcessJoystickInputs.turnValue * turnSpeed * Time.deltaTime), 0f);

        if (touchingGround && direction == Vector3.zero)
        {
            SetPlayerState(PlayerState.Idle);
        }

        direction = new Vector3(ProcessJoystickInputs.NormalizedDirection.x * (canClimb ? 0f : 1f), 
                                ProcessJoystickInputs.NormalizedDirection.y * (canClimb ? 1f : 0f), 
                                ProcessJoystickInputs.NormalizedDirection.z * (canClimb ? 0f : 1f));

        if (canClimb && ProcessJoystickInputs.NormalizedDirection.y <= -0.5f && touchingGround)
        {
            canClimb = false;
            rb.useGravity = true;
            SetPlayerState(PlayerState.Idle);
        }

        transform.Translate(direction * (canClimb ? climbSpeed : initialMovementSpeed) * Time.fixedDeltaTime, Space.Self);
        touchingGround = Physics.Raycast(transform.position + new Vector3(0f, 0.1f, 0f), Vector3.down, 0.2f, groundLayerMask);
        // Debug.DrawRay(transform.position, Vector3.down * 0.2f, Color.red, 3f);

        isClimbing = canClimb && ProcessJoystickInputs.NormalizedDirection.y > 0f;

        if (canVault && ProcessJoystickInputs.Vaulting)
        {
            transform.position += new Vector3(0f, detectedCollider.bounds.extents.y * 2.25f, 0f);
            rb.AddForce(transform.forward, ForceMode.Impulse);
        }

        if (touchingGround)
        {
            coyoteTimeIsUp = false;
        }

        if (!coyoteTimeIsUp)
        {
            if (ProcessJoystickInputs.NormalizedDirection.magnitude > 0f && touchingGround)
            {
                SetPlayerState(PlayerState.Running);
            }

            if (Input.GetKeyDown(jumpKey) && canJump)
            {
                Debug.Log("jump");
                canJump = false;
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
        canJump = false;
        coyoteTimeIsUp = true;
        coyoteTimeCoroutineIsCalled = false;
    }
}
