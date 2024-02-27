using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gLaser : MeleeBaseState
{
    private LaserScript laserScript;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        laserScript = GameObject.FindGameObjectWithTag("Laser").GetComponent<LaserScript>();
        laserScript.maxLength = curWeapon.fireDistance;
        laserScript.baseDamage = curWeapon.damage;
        laserScript.EnableLaser();
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (fixedtime >= duration)
        {
            laserScript.DisableLaser();
        }
    }
}