using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gLaser : MeleeBaseState
{
    private LaserScript laserScript;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        // new code
        laserScript = GameObject.FindGameObjectWithTag("Laser").GetComponent<LaserScript>();
        laserScript.maxLength = curWeapon.fireDistance;
        laserScript.baseDamage = curWeapon.damage;
        laserScript.EnableLaser();
        //

        attackIndex = 5;
        animator.SetTrigger("Attack" + attackIndex);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // when attack over
        if (fixedtime >= duration)
        {
            // new code
            laserScript.DisableLaser();
            //

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