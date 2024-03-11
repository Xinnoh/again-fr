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

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponentInParent<BasicAI>();
        animator = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>(); // Access the NavMeshAgent from the parent

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

        animator.SetFloat("AimX", playerDirection.x);
        animator.SetFloat("AimY", playerDirection.y);
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
    }


    private void StateMachine()
    {
        if (dying)
        {
            // die

            return;
        }

        if(attacking)
        {
            //attack

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


    public void AttackAnimation(Vector2 direction)
    {
        animator.SetFloat("AttackX", direction.x);
        animator.SetFloat("AttackY", direction.y);
        ChangeAnimationState(Attack);
        animator.speed = 1f;


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
        Debug.Log(currentState);
        if(currentState == Movement)
        {
            float speed = agent.velocity.magnitude;
            float animationSpeed = Mathf.Clamp(speed, 0.1f, 1f);
            animator.speed = animationSpeed;
        }
    }
}
