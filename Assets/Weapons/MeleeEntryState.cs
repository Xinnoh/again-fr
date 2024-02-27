using System;
using UnityEngine;

public class MeleeEntryState : State
{
    // Called when the player hits attack, this picks which attack to use
    private WeaponManager weaponManager;
    private Weapon curWeapon;
    protected Animator animator;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        animator = GetComponent<Animator>();

        #region Bug checks

        weaponManager = GetComponent<WeaponManager>();
        // Early return for error conditions
        if (weaponManager.GetAttackingWeapon() == null)
        {
            Debug.LogError("Current weapon is null.");
            return;
        }
        #endregion

        curWeapon = weaponManager.GetAttackingWeapon();

        Type stateType = Type.GetType(curWeapon.weaponState, throwOnError: false, ignoreCase: true);

        #region bug checks
        if (string.IsNullOrEmpty(curWeapon.weaponState))
        {
            Debug.Log("Weapon state is not defined.");
            stateMachine.SetNextStateToMain();
            return;
        }

        if (stateType == null)
        {
            Debug.LogError($"State transition failed. No state class matches the name: {curWeapon.weaponState}");
            stateMachine.SetNextStateToMain();
            return;
        }

        if (!typeof(State).IsAssignableFrom(stateType))
        {
            Debug.LogError($"Provided type {curWeapon.weaponState} is not a valid state.");
            stateMachine.SetNextStateToMain();
            return;
        }

        if (animator == null)
        {
            Debug.LogError($"Animator is missing from MeleeEntry.");
            return;
        }

        #endregion


        State nextState = (State)Activator.CreateInstance(stateType);
        _stateMachine.SetNextState(nextState);
    }
}
