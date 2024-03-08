using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class EnemyAnimate : MonoBehaviour
{
    private BasicAI enemyAI;
    private Vector2 playerDirection;
    private Animator animator;

    private Vector2 movement;
    private float speed;
    private bool dying, attacking, retreating;


    private static readonly Dictionary<string, int> animationStates = new Dictionary<string, int>();
    private int currentState;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<BasicAI>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AimPlayer();
        StateMachine();
    }


    private void AimPlayer()
    {
        playerDirection = enemyAI.GetPlayerDirection();

        animator.SetFloat("AimX", playerDirection.x);
        animator.SetFloat("AimY", playerDirection.y);

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


    private void UpdateMovement()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        speed = movement.sqrMagnitude;

        animator.SetFloat("Speed", speed);
    }
}
