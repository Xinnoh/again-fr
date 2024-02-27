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
    // [SerializeField] private float spawnRange = 3f;
    [SerializeField] private float patrolRadius = 10f;
    private Vector2 patrolAreaCenter;
    private Vector2 nextPatrolPoint;
    private bool isPatrolling;
    private Vector2 spawnPos;

    public bool debugMode = false;

    [Header("Combat Properties")]
    [SerializeField] private bool isRangedEnemy;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float bulletSpeed = 10f;

    // Not used currently
    [SerializeField] private float attackRange = 1.5f;

    [Tooltip("How close you get before it starts running away.")]
    [SerializeField] private float retreatDistance = 2f;
    [Tooltip("How long between attacks.")]
    [SerializeField] private float attackCooldown = 2f;
    [Tooltip("Length of attack animation.")]
    [SerializeField] private float attackTime = 1f;
    [Tooltip("Randomise the time between attacks.")]
    [SerializeField] private float intervalRandomness = 1f;

    private float lastAttackTime = -99f;
    private float angle;

    [Header("Strafe Properties")]
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
    [HideInInspector] public bool retreatMode;

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
    private float originalRetreatDistance;
    private float originalStrafeDistance;
    private EnemyAnimator animator;


    #endregion



    void Start()
    {
        animator = GetComponent<EnemyAnimator>();
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");

        patrolAreaCenter = transform.position;
        agent.enabled = true;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        PickNextPatrolPoint();

        spawnPos = transform.position;
        lastPosition = transform.position;

        originalRetreatDistance = retreatDistance;
        originalStrafeDistance = strafeDistance;

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
        if(interruptEnemy)
        {
            return;
        }

        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            if (player == null)
            {

            }
            return;
        }

        target = player.transform;
        float distanceToTarget = Vector2.Distance(target.position, transform.position);

        if (distanceToTarget <= detectionRange)
        {
            isPatrolling = false;

            if (distanceToTarget <= retreatDistance)
            {
                RetreatTarget();
                return;
            }

            float timeSinceLastAttack = Time.time - lastAttackTime - attackTime;
            if (distanceToTarget <= strafeDistance && timeSinceLastAttack >= attackCooldown)
            {
                AttackTarget();
                Debug.Log("Attack");
                return;
            }
            if (distanceToTarget <= strafeDistance)
            {
                StrafeTarget();
                return;
            }

            ChaseTarget();
            return;
        }
        
        if (!isPatrolling || agent.remainingDistance < 0.5f)
        {
            StartPatrolling();
        }
    }


    private void RetreatTarget()
    {
        Vector3 awayDirection = (transform.position - target.position).normalized;
        Vector3 newDestination = transform.position + awayDirection * retreatDistance;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newDestination, out hit, retreatDistance, 1))
        {
            agent.SetDestination(hit.position);
        }
    }


    private void StrafeTarget()
    {

        Vector2 directionToTarget = (Vector2)target.position - (Vector2)transform.position;
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
        Vector2 direction = (target.position - transform.position).normalized;

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
            StartCoroutine(Interrupt(attackTime));
        }
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
                if (otherAI.retreatMode == false)
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
        agent.SetDestination(target.position);
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
        retreatMode = true;
        retreatDistance = retreatDistance + 2f;
        strafeDistance = strafeDistance + 2f;
        yield return new WaitForSeconds(time);

        retreatMode = false;
        retreatDistance = originalRetreatDistance;
        strafeDistance = originalStrafeDistance; 
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



    // mostly unused code, just patrols if not in range of player

    #region mostly unused code
    private void PickNextPatrolPoint()
    {
        Vector2 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += patrolAreaCenter;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        nextPatrolPoint = hit.position;
        agent.SetDestination(nextPatrolPoint);
        isPatrolling = true;
    }

    private void StartPatrolling()
    {
        if (!isPatrolling || agent.remainingDistance < 0.5f)
        {
            PickNextPatrolPoint();
        }
    }


    #region Debug
    private void DrawDebugs()
    {
        detectionDraw.transform.localScale = new Vector3(detectionRange * 2, detectionRange * 2, 1);
        attackDraw.transform.localScale = new Vector3(attackRange * 2, attackRange * 2, 1);
        patrolDraw.transform.localScale = new Vector3(patrolRadius * 2, patrolRadius * 2, 1);
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
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Patrol area representation
        Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(patrolAreaCenter, patrolRadius);
    }
    #endregion

    #endregion
}
