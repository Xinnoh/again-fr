using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hBasic : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 1;
        animator.SetTrigger("Attack" + attackIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new MeleeEntryState());
            }
            else
            {
                stateMachine.SetNextStateToMain();
            }
        }
    }
}