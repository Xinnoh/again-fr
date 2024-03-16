using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimate : MonoBehaviour
{
    private BasicAI enemyAI;
    private Vector2 playerDirection;
    private Animator animator;
    private NavMeshAgent agent;


    private Vector2 movement;
    private float speed;
    private bool dying, attacking, retreating;


    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Movement = Animator.StringToHash("Movement");

    private static readonly Dictionary<string, int> animationStates = new Dictionary<string, int>();
    private int currentState;

    [SerializeField] private GameObject hitboxContainer, hitbox;
    private BoxCollider2D hitboxCollider;
    private bool attackActive;
    private GameObject player;
    private CircleCollider2D playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponentInParent<BasicAI>();
        animator = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>(); // Access the NavMeshAgent from the parent
        hitboxCollider = hitbox.GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<CircleCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        AimPlayer();
        StateMachine();
        AdjustAnimationSpeed();
        CheckAttackAnimationCompletion();
    }


    private void AimPlayer()
    {
        playerDirection = enemyAI.GetPlayerDirection();
        float aimDirX = 0;
        float aimDirY = 0;
        if (enemyAI.isRangedEnemy)
        {
            aimDirX = playerDirection.x;
            aimDirY = playerDirection.y - .2f;
        }
        else
        {
            aimDirX = Mathf.Round(playerDirection.x);
            aimDirY = Mathf.Round(playerDirection.y);
        }

        animator.SetFloat("AimX", aimDirX);
        animator.SetFloat("AimY", aimDirY);
    }

    private void CheckAttackAnimationCompletion()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == Attack)
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                OnAttackAnimationComplete();
                return;
            }

            animator.speed = 1f;
        }

    }

    private void OnAttackAnimationComplete()
    {
        ChangeAnimationState(Movement);
        enemyAI.StopAttack();
        hitbox.SetActive(false);
    }


    private void StateMachine()
    {
        if (dying)
        {
            // die

            return;
        }


        if (animator.GetFloat("AttackActive") > 0f)
        {
            AttackPlayer();
        }

        if (attacking)
        {
            return;
        }

        if(speed > 0.02f)
        {
            if (retreating)
            {
                // Retreat

                return;
            }

            // walk
            return;
        }


        // Idle

    }

    private void AttackPlayer()
    {
        Collider2D[] collidersToDamage = new Collider2D[10];

        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitboxCollider, filter, collidersToDamage);

        // Iterate through the detected colliders
        for (int i = 0; i < colliderCount; i++)
        {
            // Check if the detected collider is the player and hasn't been damaged yet
            if (collidersToDamage[i].gameObject.tag == "Player")
            {
                Health playerHealth = player.GetComponent<Health>();
                playerHealth.TakeDamage(enemyAI.attackDamage);
                return;
            }
        }
    }


    public void AttackAnimation(Vector2 direction)
    {
        float aimDirX = 0;
        float aimDirY = 0;
        if (enemyAI.isRangedEnemy)
        {
            aimDirX = playerDirection.x;
            aimDirY = playerDirection.y - .2f;
        }
        else
        {
            aimDirX = Mathf.Round(playerDirection.x);
            aimDirY = Mathf.Round(playerDirection.y);
        }

        animator.SetFloat("AttackX", aimDirX);
        animator.SetFloat("AttackY", aimDirY);
        ChangeAnimationState(Attack);
        animator.speed = 1f;

        /*
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle = (angle + 360) % 360;

        float roundedAngle = Mathf.Round(angle / 45) * 45;

        Quaternion targetRotation = Quaternion.Euler(0, 0, roundedAngle - 90); 
        hitboxContainer.transform.rotation = targetRotation;
        */
        hitbox.SetActive(true);
    }


    static EnemyAnimate()
    {
        // Use reflection to automatically populate the dictionary with field names and values
        FieldInfo[] fields = typeof(EnemyAnimate).GetFields(BindingFlags.Static | BindingFlags.NonPublic);

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

    public void ChangeAnimationState(int newState)
    {
        if (currentState == newState) return;

        animator.Play(newState);
        currentState = newState;
    }
    

    private void AdjustAnimationSpeed()
    {
        if(currentState == Movement)
        {
            float speed = agent.velocity.magnitude;
            float animationSpeed = Mathf.Clamp(speed, 0.1f, 1f);
            animator.speed = animationSpeed;
        }
        else
        {
            animator.speed = 1f;
        }
    }
}
