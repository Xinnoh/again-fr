using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public Animator animator;
    private Vector2 movement;
    private Vector2 lastMovementDirection; // Added to keep track of the last movement direction
    private Target aimScript;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        aimScript = GetComponent<Target>();
    }

    void Update()
    {
        UpdateMovement();
        UpdateAimDirection();
    }

    private void UpdateMovement()
    {

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        lastMovementDirection = playerMovement.GetLastMovementDirection();

        animator.SetFloat("Horizontal", lastMovementDirection.x);
        animator.SetFloat("Vertical", lastMovementDirection.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void UpdateAimDirection()
    {
        if (aimScript.GetAimAngle() != -1)
        {
            animator.SetBool("Enemy", true);

            Vector3 aimVector = aimScript.GetAimVector();
            animator.SetFloat("AimX", aimVector.x);
            animator.SetFloat("AimY", aimVector.y);
        }
        else
        {
            animator.SetBool("Enemy", false);
        }
    }


    public void SetAttacking(bool attackState)
    {
        animator.SetBool("Attacking", attackState);
    }
}
