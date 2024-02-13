using System;
using UnityEngine;

public class MeleeEntryState : State
{
    // Called when the player hits attack, this picks which attack to use
    private WeaponManager weaponManager;
    private Weapon curWeapon;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found with tag 'Player'.");
            return;
        }

        weaponManager = player.GetComponent<WeaponManager>();
        // Early return for error conditions
        if (weaponManager.currentWeapon == null)
        {
            Debug.LogError("Current weapon is null.");
            return;
        }

        curWeapon = weaponManager.currentWeapon;


        if (string.IsNullOrEmpty(curWeapon.weaponState))
        {
            Debug.Log("Weapon state is not defined.");
            stateMachine.SetNextStateToMain();
            return;
        }

        Type stateType = Type.GetType(curWeapon.weaponState, throwOnError: false, ignoreCase: true);
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

        // If no errors, proceed with state transition
        State nextState = (State)Activator.CreateInstance(stateType);
        _stateMachine.SetNextState(nextState);
    }
}
