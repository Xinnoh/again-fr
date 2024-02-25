using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntryState : State
{

    protected Animator animator;
    protected Type stateType;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        animator = GetComponent<Animator>();


        State nextState = (State)Activator.CreateInstance(stateType);
        _stateMachine.SetNextState(nextState);
    }
}
