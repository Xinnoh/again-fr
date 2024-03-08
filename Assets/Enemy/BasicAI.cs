using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Cinemachine.CinemachineImpulseDefinition;

public class BasicAI : MonoBehaviour
{

    #region variables


    [Header("Spawn Properties")]
    private Vector2 spawnPos;

    public bool debugMode = false;

    [Header("Combat Properties")]

    [Tooltip("Circumfrence of player that navmesh aims at.")]
    [SerializeField] private float playerBuffer = 2f;

    [Tooltip("How close you get before it starts running away. (Must be smaller than playerbuffer)")]
    [SerializeField] private float retreatDistance = 1f;

    private float originalRetreatDistance;
    private float originalStrafeDistance;
    private float originalBufferDistance;

    [Header("Attack Properties")]

    [SerializeField] private float attackRange = 1.5f;

    [Tooltip("How long between attacks.")]
    [SerializeField] private float attackCooldown = 2f;
    [Tooltip("Length of attack animation.")]
    [SerializeField] private float attackTime = 1f;
    [Tooltip("Randomise the time between attacks.")]
    [SerializeField] private float intervalRandomness = 1f;

    [Header("Ranged Enemy")]
    [SerializeField] private bool isRangedEnemy;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float bulletSpeed = 10f;


    private float lastAttackTime = -99f;
    private float angle;

    [Header("Strafe Properties")]
    [Tooltip("How likely the enemy is to strafe (percentage over 1).")]
    [SerializeField] private float strafeChance = 0f;

    [SerializeField] private float minStrafeChangeInterval = .8f;
    [SerializeField] private float maxStrafeChangeInterval = 6f;
    // [SerializeField] private float maxIntervalRandom = 1f;

    private float timeSinceChangeInterval;

    private bool strafeRight;
    [Tooltip("The distance before it is close enough to attack/strafe.")]
    [SerializeField] private float strafeDistance = 3f;
    private float lastStrafeChangeTime;
    [SerializeField] private float minMovementThreshold = .04f;
    private Vector2 lastPosition;
    [HideInInspector] public bool retreating;
    private bool attacking;

    [Header("Hitstun")]
    [HideInInspector] public bool isStunned;
    private float stunTimer;
    [SerializeField] private float stunArmor = 0.5f;

    [Header("Gizmos")]
    [SerializeField] private GameObject detectionDraw;
    [SerializeField] private GameObject attackDraw;
    [SerializeField] private GameObject patrolDraw;
    [SerializeField] private GameObject spawnDraw;


    private bool interruptEnemy; // prrioritise one action over others

    private Transform target;
    private NavMeshAgent agent;
    private StateMachine stateMachine;
    private GameObject player;
    private Coroutine retreatCoroutine;
    private EnemyAnimator animator;


    #endregion



    void Start()
    {
        animator = GetComponent<EnemyAnimator>();
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");


        agent.enabled = true;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        spawnPos = transform.position;
        lastPosition = transform.position;

        originalRetreatDistance = retreatDistance;
        originalStrafeDistance = strafeDistance;
        originalBufferDistance = playerBuffer;

        lastAttackTime = Time.time + Random.Range(0, intervalRandomness);
    }
    

    void FixedUpdate()
    {
        StrafeChangeDirection();

        if (isStunned)
        {
            UpdateStunTimer();
        }

        else
        {
            InteractPlayer();
        }

        #region debug mode
        if (debugMode)
        {
            DrawDebugs();
        }
        #endregion

    }

    private void UpdateStunTimer()
    {
        stunTimer -= Time.fixedDeltaTime;
        if (stunTimer <= 0f)
        {
            interruptEnemy = false;
            isStunned = false;
            agent.isStopped = false;
        }
    }


    private void InteractPlayer()
    {
        if(interruptEnemy || attacking)
        {
            return;
        }

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.Log("No player");
            }
            return;
        }
        
        float distanceToTarget = Vector2.Distance(GetPlayerPosition(), transform.position);

        // if too close
        if (distanceToTarget <= retreatDistance)
        {
            RetreatTarget();
            return;
        }

        // ideal distance
        float timeSinceLastAttack = Time.time - lastAttackTime - attackTime;
        if (distanceToTarget <= strafeDistance)
        {
            if(timeSinceLastAttack >= attackCooldown)
            {
                AttackTarget();  Debug.Log("Attack");
                return;
            }

            if (Random.Range(0f, 1f) <= strafeChance)
            {
                StrafeTarget();
                return;
            }
        }

        // too far
        ChaseTarget();
    }


    private void RetreatTarget()
    {
        Vector3 awayDirection = GetPlayerDirection();
        Vector3 newDestination = transform.position - awayDirection * retreatDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newDestination, out hit, retreatDistance, 1))
        {
            agent.SetDestination(hit.position);
        }
    }


    private void StrafeTarget()
    {

        Vector2 directionToTarget = GetPlayerDirection();
        Vector2 strafeDirection = strafeRight ? new Vector2(-directionToTarget.y, directionToTarget.x) : new Vector2(directionToTarget.y, -directionToTarget.x);
        strafeDirection.Normalize();

        Vector2 strafeTarget = (Vector2)transform.position + strafeDirection * 2f;
        agent.destination = strafeTarget;

        // If the target is too close, it means there's something in the way. Change direction.
        if (agent.remainingDistance < 1f)
        {
            lastStrafeChangeTime = Time.time;
            strafeRight = !strafeRight;
        }
    }

    private void AttackTarget()
    {
        Vector2 direction = (GetPlayerPosition() - transform.position).normalized;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        interruptEnemy = true;

        lastAttackTime = Time.time + Random.Range(0, intervalRandomness);

        if (isRangedEnemy)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            BulletBehavior bulletScript = projectile.GetComponent<BulletBehavior>();

            if (bulletScript != null)
            {
                bulletScript.SetDirection(angle, bulletSpeed);
            }

            StartCoroutine(InterruptMove(attackTime));
        }

        else
        {
            // temporary
            StartCoroutine(Interrupt(attackTime));
        }
    }

    public Vector2 GetPlayerDirection()
    {
        Vector2 direction = (GetPlayerPosition() - transform.position).normalized;
        return direction;
    }

    private IEnumerator InterruptMove(float time)
    {
        interruptEnemy = true;
        yield return new WaitForSeconds(time);
        interruptEnemy = false;
    }

    private IEnumerator Interrupt(float time)
    {
        interruptEnemy = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(time);
        interruptEnemy = false;
        agent.isStopped = false;
    }

    private void StrafeChangeDirection()
    {
        // After X time, randomly change direction
        if (Time.time > lastStrafeChangeTime + maxStrafeChangeInterval)
        {
            if (Random.value <= 0.9f) // chance to retreat
            {
                strafeRight = !strafeRight;
                lastStrafeChangeTime = Time.time;
            }
            else
            {
                RetreatMode(3f);
            }
            return;
        }

        AvoidAllies();

        lastPosition = transform.position;
    }


    private void AvoidAllies()
    {
        // If minimum time since last change of direction
        if (Time.time < lastStrafeChangeTime + minStrafeChangeInterval)
        {
            return;
        }

        // if not moving
        Vector2 currentPosition = transform.position;
        if ((currentPosition - lastPosition).magnitude > minMovementThreshold)
        {
            return;
        }

        // If any nearby enemies
        // If they're not retreating, retreat to avoid them
        // If they are retreating, change direction
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hit in hits)
        {
            if (hit != null && hit.gameObject != gameObject && hit.CompareTag("Enemy")) // Assuming "Enemy" is your tag
            {
                // If they're not retreating, retreat
                BasicAI otherAI = hit.GetComponent<BasicAI>();
                if (otherAI.retreating == false)
                {
                    RetreatMode(4f);
                    lastStrafeChangeTime = Time.time;
                }
            }
            else
            {
                // Otherwise, change direction to avoid them
                strafeRight = !strafeRight;
            }
        }

        lastStrafeChangeTime = Time.time;
    }


    private void ChaseTarget()
    {
        // Create a buffer of 1f around the player so we don't push them
        Vector3 toTarget = GetPlayerPosition() - transform.position;
        Vector3 targetPosition = GetPlayerPosition() - (toTarget.normalized * playerBuffer);
        agent.SetDestination(targetPosition);
    }


    // Makes enemy retreat for a brief time
    private void RetreatMode(float time)
    {
        if (retreatCoroutine != null)
        {
            StopCoroutine(retreatCoroutine);
        }
        retreatCoroutine = StartCoroutine(RetreatCoroutine(time)); 
    }
    private IEnumerator RetreatCoroutine(float time)
    {
        retreating = true;
        retreatDistance = originalRetreatDistance + 2f;
        strafeDistance = originalStrafeDistance + 2f;
        playerBuffer = originalBufferDistance + 2f;

        yield return new WaitForSeconds(time);

        retreating = false;
        retreatDistance = originalRetreatDistance;
        strafeDistance = originalStrafeDistance; 
        playerBuffer = originalBufferDistance;
    }

    public void HitStun(float damage)
    {
        if (damage > stunArmor)
        {
            isStunned = true;
            interruptEnemy = true;
            stunTimer = damage / 3;
            if (agent.enabled)
            {
                agent.isStopped = true;
            }
        }
    }

    private Vector3 GetPlayerPosition()
    {
        if (player == null)
        {
            Debug.LogError("Player object is not assigned or found.");
            return Vector3.zero; 
        }

        // The player hitbox is offset by this amount
        Vector3 loweredPosition = player.transform.position;
        loweredPosition.y -= 1.075f;
        return loweredPosition;
    }





    #region Debug
    private void DrawDebugs()
    {
        attackDraw.transform.localScale = new Vector3(attackRange * 2, attackRange * 2, 1);
        spawnDraw.transform.position = spawnPos;
    }


    // Helper method to draw a circle using Gizmos
    private void DrawGizmoCircle(Vector3 center, float radius)
    {
        float theta = 0;
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);
        Vector3 pos = center + new Vector3(x, y, 0);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            newPos = center + new Vector3(x, y, 0);
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        Gizmos.DrawLine(lastPos, pos);
    }
    private void OnDrawGizmos()
    {
        if (!agent) return;

        Vector2 directionToTarget = (Vector2)target.position - (Vector2)transform.position;
        Vector2 strafeDirection = strafeRight ? new Vector2(-directionToTarget.y, directionToTarget.x) : new Vector2(directionToTarget.y, -directionToTarget.x);
        strafeDirection.Normalize();
        Vector2 strafeTarget = (Vector2)transform.position + strafeDirection * strafeDistance;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, strafeDirection * strafeDistance);
        Gizmos.DrawSphere(strafeTarget, 0.25f);
    }


    private void OnDrawGizmosSelected()
    {
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

    }
    #endregion

}
