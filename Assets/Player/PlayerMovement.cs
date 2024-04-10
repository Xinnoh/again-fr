using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement")]

    public float moveSpeed = 5f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float maxDashTime = 0.8f;
    [Tooltip("Acceleration rate for adjusting dash direction")]
    public float dashTurnRate = 1f;

    [Tooltip("Speed threshold below which dashing stops if moving opposite")]
    public float dashStopSpeedThreshold = 10f;

    [Tooltip("Delay before dash starts, during which the player cannot move")]
    public float preDashDelay = 0.2f;

    public float maxDashSpeed = 0.5f;
    public int maxDashes = 2;
    public ParticleSystem dashParticles;
    public AudioClip dashSound;

    [Header("Recharge")]
    [SerializeField] private float dashRecoveryTime = 1.5f; // Delay before recharge
    [SerializeField] private float dashRecoveryRate = .1f;


    private Vector2 lastMovementDirection = Vector2.down; 

    private float speedMultiplier = 1f;
    private PlayerAnimate playerAnimate;
    private PlayerManager playerManager;
    private Rigidbody2D rb;

    private bool moveEnabled = true;
    private Vector2 moveDirection;
    private Vector2 dashDirection;
    private bool isDashing;
    private bool isPreDashing; // Indicates the pre-dash delay phase
    private float currentDashTime;
    private float dashDirectionHoldTime; // Time since a new direction was held during dash
    private float lastDashTime; // Last time the player dashed
    [HideInInspector] public float currentDashes; // Current number of available dashes

    public PlayerInputActions playerControls;
    private InputAction move, dash;

    #endregion


    /*
     * This controls movement
     * The player can press space to dash, this is on a charge and has a cooldown
     * A predash happens when the player presses dash. It stops the playermovement for a brief second before executing the dash.
     */


    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
        dash = playerControls.Player.Dash;
        dash.Enable();
        dash.performed += Dash;
        dash.canceled += StopDashing;
    }

    private void OnDisable()
    {
        move.Disable();
        dash.Disable(); 
        dash.performed -= Dash;
    }


    void Start()
    {
        moveEnabled = true;
        rb = GetComponent<Rigidbody2D>();
        playerAnimate = GetComponent<PlayerAnimate>();
        currentDashes = maxDashes;
        playerManager = GetComponent<PlayerManager>();
    }

    void Update()
    {
        if (playerManager.playerActive)
        {
            UpdateMoveDirection();
            UpdateDash();
        }
    }

    void FixedUpdate()
    {
        if(playerManager.playerActive)
        {
            UpdateMovement();
        }
    }

    private void UpdateMoveDirection()
    {
        if (moveEnabled)
        {
            moveDirection = move.ReadValue<Vector2>();

            if (moveDirection.sqrMagnitude > 0)
            {
                lastMovementDirection = moveDirection;
            }
        }

        else
        {
            moveDirection = new Vector2(0, 0).normalized;
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if (!isDashing && !isPreDashing && currentDashes > maxDashes / 3f)
        {
            StartCoroutine(PreDash());
        }
    }

    private void StopDashing(InputAction.CallbackContext context)
    {
        StopDash();
    }


    private void UpdateDash()
    {
        RechargeDashes();

        if (!isDashing || isPreDashing)
            return;

        if(currentDashTime >= maxDashTime || moveDirection == Vector2.zero)
        {
            StopDash();
        }
    }

    private void RechargeDashes()
    {
        if (isDashing || currentDashes >= maxDashes)
        {
            return;
        }


        float timeSinceLastDash = Time.time - lastDashTime;
        if(timeSinceLastDash <= dashRecoveryTime)
        {
            return;
        }


        float dashRecovery = dashRecoveryRate * Time.deltaTime;
        currentDashes += dashRecovery;

        if (currentDashes > maxDashes)
        {
            currentDashes = maxDashes;
        }
    }

    private void UpdateMovement()
    {
        if (isDashing)
        {
            currentDashTime += Time.fixedDeltaTime;
            dashDirectionHoldTime += Time.fixedDeltaTime;

            Vector2 adjustedDashDirection = CalculateAdjustedDashDirection();
            rb.velocity = adjustedDashDirection * dashSpeed;

            if (rb.velocity.magnitude < dashStopSpeedThreshold || (Input.GetKeyUp(KeyCode.Space)))
            {
                StopDash();
            }

            return;
        }

        if (!isPreDashing)
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * speedMultiplier * Time.fixedDeltaTime);
        }
    }

    public void SetSpeedMultiplier(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        playerAnimate.SetAttacking(true);
        StartCoroutine(ResetSpeedMultiplierAfterDelay(duration));
    }

    IEnumerator ResetSpeedMultiplierAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speedMultiplier = 1f;
        playerAnimate.SetAttacking(false);
    }

    IEnumerator PreDash()
    {
        isPreDashing = true;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(preDashDelay);

        if (moveDirection != Vector2.zero)
        {
            StartDash();
        }
        else
        {
            // If there's no input after delay, ensure the player doesn't dash
            rb.velocity = Vector2.zero;
        }
        isPreDashing = false;
    }

    private void StartDash()
    {
        AudioSource.PlayClipAtPoint(dashSound, transform.position, 1f);
        isDashing = true;
        currentDashTime = 0f;
        dashDirectionHoldTime = 0f; // ???
        dashDirection = moveDirection;
        currentDashes--; // how many times we can dash
        lastDashTime = Time.time;
        dashParticles.Emit(30);
    }

    private void StopDash()
    {
        isDashing = false;
        rb.velocity = Vector2.zero; // Reset velocity to stop immediately when dash stops
    }

    private Vector2 CalculateAdjustedDashDirection()
    {
        float timeFactor = Mathf.Clamp(dashDirectionHoldTime, 0, maxDashTime);
        float adjustmentFactor = Mathf.Sqrt(timeFactor) * dashTurnRate;
        adjustmentFactor = Mathf.Min(adjustmentFactor, maxDashSpeed);
        return Vector2.Lerp(dashDirection, moveDirection, adjustmentFactor).normalized;
    }

    public Vector2 GetLastMovementDirection()
    {// The last direction we travelled before stopping
        return lastMovementDirection;
    }

    public void ResetSpeedMultiplier()
    {
        speedMultiplier = 1f;
    }

    public void SetSpeedModifier(float speed)
    {
        speedMultiplier = speed;
    }

    public void DisableMovemet(){ moveEnabled = false; }
    public void EnableMovement() { moveEnabled = true; }
}
