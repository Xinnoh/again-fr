using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gBasic : MeleeBaseState
{

    // Currently the default attack animation

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 2;
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