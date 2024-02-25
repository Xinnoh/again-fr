using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [HideInInspector] public Animator animator;
    private Vector2 movement;
    private Vector2 lastMovementDirection; // Added to keep track of the last movement direction
    private Target aimScript;


    const string PIdle = "Idle";
    const string IdleEnemy = "IdleEnemy";
    const string PMovement = "Movement";
    const string PMovementSafe = " MovementSafe";
    const string lMove = "lMove";
    const string lPunch = "lPunch";
    const string hPunch = "hPunch";
    const string gLaser = "Laser";
    const string gLight = "gLight";

    private string currentState;


    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        aimScript = GetComponent<Target>();

        // set state as idle


    }

    void Update()
    {
        UpdateMovement();
        UpdateAimDirection();


    }



    void ChangeAnimationState(string newState)
    {
        if(currentState == newState) return;

        animator.Play(newState);
        currentState = newState;
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
