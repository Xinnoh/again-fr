using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gBasic : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        duration = .45f;
    }

}