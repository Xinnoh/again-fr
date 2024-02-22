using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class lMove : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        //Attack
        attackIndex = 4;
        animator.SetTrigger("Attack" + attackIndex);

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            playerMovement.ResetSpeedMultiplier();

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

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        MoveToTarget();
    }


    private void MoveToTarget()
    {


        if (curWeapon.momentum != 0)  // If we are using weapon with momentum
        {
            if (fixedtime >= curWeapon.momentumDelay)   // how long to delay before attacking
            {
                if (!targetAcquired) // ensures we only target one enemy throughout attack
                {
                    FindEnemy();
                }

                Vector2 moveDirection = enemyDirection.normalized; 
                Vector2 movementStep = moveDirection * curWeapon.momentum * Time.deltaTime;
                playerMovement.transform.position += new Vector3(movementStep.x, movementStep.y, 0);


            }
        }

    }
}