using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// unused, will probably delete

public class ComboCharacter : MonoBehaviour
{

    private StateMachine meleeStateMachine;

    [SerializeField] public Collider2D hitbox;
    [SerializeField] public GameObject Hiteffect;

    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
    }

    void Update()
    {
        // 



        // If mousedown and in idle state, attack
        if (Input.GetMouseButton(0) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new MeleeEntryState());
        }
    }
}