using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gElectroball : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);


    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            // disable
        }
    }
}
