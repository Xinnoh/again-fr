using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class BasicAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    private NavMeshAgent agent;

    // Patrol properties
    public Vector2 patrolAreaCenter;
    public float patrolRadius = 10f;
    private Vector2 nextPatrolPoint;
    private bool isPatrolling;
    public float spawnRange = 3f;
    public float bulletSpeed = 10f;

    public bool debugMode = false;

    // Attack properties
    public float attackRange = 1.5f;
    public GameObject projectilePrefab;
    public float attackCooldown = 2f;
    private float lastAttackTime = -99f;
    private float angle; 
    // Detection properties
    public float detectionRange = 5f;

    //Gizmos
    [SerializeField ]    public GameObject detectionDraw, attackDraw, patrolDraw, spawnDraw;
    public Vector2 spawnPos;
    public Vector2 currentPos;

    // haven't bothered to clean up this code yet since I might not use it


    void Start()
    {

        patrolAreaCenter = transform.position;

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        PickNextPatrolPoint();

        spawnDraw.transform.localScale = new Vector3(spawnRange * 2, spawnRange * 2, 1);
        spawnPos = transform.position;

    }

    private void OnEnable()
    {
        if (Vector2.Distance(transform.position, spawnPos) > 1f)
        {
            // transform.position = spawnPos;
        }
    }

    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");

        target = player.transform;

        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget <= detectionRange)
        {
            ChaseTarget();
        }
        else if (!isPatrolling || agent.remainingDistance < 0.5f)
        {
            StartPatrolling();
        }

        if (distanceToTarget <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackTarget();
        }

        if (debugMode)
        {
            detectionDraw.transform.localScale = new Vector3(detectionRange * 2, detectionRange * 2, 1);
            attackDraw.transform.localScale = new Vector3(attackRange * 2, attackRange * 2, 1);
            patrolDraw.transform.localScale = new Vector3(patrolRadius * 2, patrolRadius * 2, 1);
            spawnDraw.transform.position = spawnPos;
        }

        currentPos = transform.position;
    }

    void ChaseTarget()
    {
        agent.SetDestination(target.position);
        isPatrolling = false;
    }

    void StartPatrolling()
    {
        if (!isPatrolling || agent.remainingDistance < 0.5f)
        {
            PickNextPatrolPoint();
        }
    }

    void PickNextPatrolPoint()
    {
        Vector2 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += patrolAreaCenter;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        nextPatrolPoint = hit.position;
        agent.SetDestination(nextPatrolPoint);
        isPatrolling = true;
    }


    void AttackTarget()
    {
        agent.isStopped = true;

        Vector2 direction = (target.position - transform.position).normalized;

        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector3 spawnPosition = transform.position;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        BulletBehavior bulletScript = projectile.GetComponent<BulletBehavior>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(angle, bulletSpeed);
        }

        lastAttackTime = Time.time;
        agent.isStopped = false;
    }





    // Helper method to draw a circle using Gizmos
    void DrawGizmoCircle(Vector3 center, float radius)
    {
        // Draw a flat circle by looping through degrees and calculating points around the center
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

        // To complete the circle
        Gizmos.DrawLine(lastPos, pos);
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
}
