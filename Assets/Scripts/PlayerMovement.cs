using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 20f;
    public float maxDashTime = 0.8f;
    public float dashAdjustmentAcceleration = 1f; // Acceleration rate for adjusting dash direction
    public float dashStopSpeedThreshold = 10f; // Speed threshold below which dashing stops if moving opposite
    public float preDashDelay = 0.2f; // Delay before dash starts, during which the player cannot move
    public float maxDashSpeed = 0.5f;
    public int maxDashes = 2; // Maximum number of stored dashes
    public float dashRecoveryTime = 2f; // Time to recover a dash
    public ParticleSystem dashParticles;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector2 dashDirection;
    private bool isDashing;
    private bool isPreDashing; // Indicates the pre-dash delay phase
    private float currentDashTime;
    private float dashDirectionHoldTime; // Time since a new direction was held during dash
    private float lastDashTime; // Last time the player dashed
    private int currentDashes; // Current number of available dashes

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDashes = maxDashes; // Initialize with the maximum number of dashes
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        // Dash recovery mechanism
        if (!isDashing && Time.time - lastDashTime > dashRecoveryTime && currentDashes < maxDashes)
        {
            currentDashes++;
            lastDashTime = Time.time; // Reset the timer upon recovering a dash
        }

        if (!isPreDashing || moveDirection != Vector2.zero)
        {
            if (moveDirection == Vector2.zero && isDashing)
            {
                StopDash(); // Stop the dash if currently dashing and no input is detected
                return; // Exit early to prevent starting a new dash without input
            }

            if (Input.GetKeyDown(KeyCode.Space) && !isDashing && !isPreDashing && currentDashes > 0)
            {
                StartCoroutine(PreDash());
            }

            if (isDashing && (Input.GetKeyUp(KeyCode.Space) || currentDashTime >= maxDashTime))
            {
                StopDash();
            }
        }
    }

    void FixedUpdate()
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
        }
        else if (!isPreDashing) // Allow movement if not in pre-dash
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

    }

    IEnumerator PreDash()
    {
        isPreDashing = true;
        rb.velocity = Vector2.zero; // Ensure velocity is zero and remains so during the delay
        yield return new WaitForSeconds(preDashDelay); // Wait for the specified delay

        if (moveDirection != Vector2.zero) // Start dash only if there's movement input after delay
        {
            StartDash();
        }
        else
        {
            // If there's no input after delay, ensure the player doesn't dash
            rb.velocity = Vector2.zero;
        }
        isPreDashing = false; // End pre-dash phase
    }

    private void StartDash()
    {
        isDashing = true;
        currentDashTime = 0f;
        dashDirectionHoldTime = 0f;
        dashDirection = moveDirection; // Use current move direction for dash
        currentDashes--; // Decrement the dash counter
        lastDashTime = Time.time; // Update the last dash time
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
        float adjustmentFactor = Mathf.Sqrt(timeFactor) * dashAdjustmentAcceleration;
        adjustmentFactor = Mathf.Min(adjustmentFactor, maxDashSpeed);
        return Vector2.Lerp(dashDirection, moveDirection, adjustmentFactor).normalized;
    }
}
