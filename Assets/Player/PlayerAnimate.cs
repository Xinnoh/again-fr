using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;
    private Weapon curWeapon;
    private Target aimScript;
    [HideInInspector] public Animator animator;

    private Vector2 movement, lastMovementDirection;
    private float speed, attackAngle, aimX, aimY;


    private bool enemyAlive, attacking, dashing;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int IdleEnemy = Animator.StringToHash("IdleEnemy");

    private static readonly int PMovement = Animator.StringToHash("Movement");
    private static readonly int PMovementSafe = Animator.StringToHash("MovementSafe");

    private static readonly int Dashing = Animator.StringToHash("Dashing");
    private static readonly int PreDash = Animator.StringToHash("PreDash");



    private static readonly int lMove = Animator.StringToHash("lMove");
    private static readonly int lBasic = Animator.StringToHash("lBasic");

    private static readonly int hBasic = Animator.StringToHash("hBasic");

    private static readonly int gBasic = Animator.StringToHash("gBasic");
    private static readonly int gLaser = Animator.StringToHash("gLaser");
    private static readonly int gElectroball = Animator.StringToHash("gElectroball");

    private static readonly int Stunned = Animator.StringToHash("Stunned");
    private static readonly int Dead = Animator.StringToHash("Dead");

    private static readonly Dictionary<string, int> animationStates = new Dictionary<string, int>();

    private int currentState;


    void Start()
    {
        playerManager= GetComponent<PlayerManager>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        aimScript = GetComponent<Target>();
        // set state as idle


    }

    void Update()
    {
        if(playerManager.playerActive)
        {
            UpdateMovement();
            UpdateAimDirection();
            UpdateState();
        }
    }
    

private void UpdateState()
    {
        if (playerManager.gameOver)
        {
            ChangeAnimationState(Dead);
            return;
        }

        // don't interrupt the attack animation
        if (attacking)
            return;

        if (dashing)
        {
            if (speed > 0.02f)
            {
                ChangeAnimationState(Dashing);
                return;
            }
            ChangeAnimationState(PreDash);
            return;
        }

        // if moving
        if (speed > 0.02f)
        {
        // The player has different animations depending on if there are enemies
        if (enemyAlive)
            {
                ChangeAnimationState(PMovement);
                return;
            }

            ChangeAnimationState(PMovementSafe);
            return;
        }

        if (enemyAlive)
            ChangeAnimationState(IdleEnemy);
        
        else
            ChangeAnimationState(Idle);
    }


    public void ChangeAnimationState(int newState)
    {
        if(currentState == newState) return;

        animator.Play(newState);
        currentState = newState;
    }


    static PlayerAnimate()
    {
        // Use reflection to automatically populate the dictionary with field names and values
        FieldInfo[] fields = typeof(PlayerAnimate).GetFields(BindingFlags.Static | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(int))
            {
                // Assuming the field names are exactly the names you want to use
                animationStates.Add(field.Name, (int)field.GetValue(null));
            }
        }
    }
    public int GetAnimationState(string stateName)
    {
        if (animationStates.TryGetValue(stateName, out int stateHash))
        {
            return stateHash;
        }
        else
        {
            Debug.LogError($"Invalid state name: {stateName}");
            return -1; // Invalid state
        }
    }


    // not working
    public float GetAnimationLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.length;
    }



    private void UpdateMovement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        speed = movement.sqrMagnitude;
        lastMovementDirection = playerMovement.GetLastMovementDirection();

        animator.SetFloat("Horizontal", lastMovementDirection.x);
        animator.SetFloat("Vertical", lastMovementDirection.y);
        animator.SetFloat("Speed", speed);
    }

    private void UpdateAimDirection()
    {
        if (aimScript.GetAimAngle() != -1)
        {
            enemyAlive = true;
            animator.SetBool("Enemy", true);

            Vector3 aimVector = aimScript.GetAimVector();
            animator.SetFloat("AimX", aimVector.x);
            animator.SetFloat("AimY", aimVector.y);
            aimX = aimVector.x;
            aimY = aimVector.y;
        }
        else
        {
            enemyAlive = false;
            animator.SetBool("Enemy", false);
        }
    }


    public void SetAttacking(bool isAttacking)
    {
        animator.SetBool("Attacking", isAttacking);
        attacking = isAttacking;
    }
}
