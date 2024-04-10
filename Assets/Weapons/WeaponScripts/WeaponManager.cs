using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    #region Variables

    private PlayerMovement playerMovement;
    private PlayerManager playerManager;
    private Target aimscript;
    private Inventory inventory;
    private UiManager uiManager;

    public Canvas canvas;



    [HideInInspector] public float currentEnergy;
    public int maxEnergy = 40;
    public int startingEnergy = 40;

    public float reloadRate = 1f;
    public float reloadCooldown = 2f;


    private float timeSinceLastAttack = 0f;   // How long since shot, calculate reload time
    private bool canShoot = true;

    [HideInInspector] public bool reloading;
    private bool hasEnergy, playerNotAttacking;

    private Weapon currentWeapon;
    private Weapon attackingWeapon;

    public bool nearTreasure = false;

    private StateMachine playerStateMachine;

    [SerializeField] public Collider2D hitbox;
    [SerializeField] public GameObject Hiteffect;

    public bool uiEnable;

    #endregion

    private void Start()
    {
        currentEnergy = maxEnergy;
        aimscript = GetComponent<Target>();
        playerMovement = GetComponent<PlayerMovement>();
        playerManager = GetComponent<PlayerManager>();
        inventory = GetComponent<Inventory>();
        uiManager = GetComponent<UiManager>();

        inventory.ResetWeaponExhaustion();

        playerStateMachine = GetComponent<StateMachine>();
    }

    private void Update()
    {
        UpdateEnergy();
        UpdateCanShoot();
    }

    private void UpdateEnergy()
    {
        // timeSinceFire is set to 0 whenever we attack

        if (currentEnergy < maxEnergy)
        {
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack >= reloadCooldown)
            {
                // This ensures shooting is disabled only after starting the reload process
                canShoot = false;
                reloading = true;
                // Decrease currentEnergy over time
                currentEnergy += reloadRate * Time.deltaTime;
                // Clamp currentEnergy to ensure it does not go below 0
                currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            }
            return;
        }

        if (timeSinceLastAttack != 0 && currentEnergy >= maxEnergy)
        {
            reloading = false;
            canShoot = true;
            inventory.ResetWeaponExhaustion();
            timeSinceLastAttack = 0;
        }
    }

    private void UpdateCanShoot()
    {

        playerNotAttacking = playerStateMachine.CurrentState.GetType() == typeof(IdleCombatState);
        hasEnergy = currentEnergy > 0;

        if (playerNotAttacking && hasEnergy && !reloading)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }



    public void FireWeapon(int val)
    {
        // Find what weaopn to use
        Weapon[] curWeapons = null;
        switch (val)
        {
            case 1: curWeapons = inventory.weapons1; break;
            case 2: curWeapons = inventory.weapons2; break;
            case 3: curWeapons = inventory.weapons3; break;
        }
        if (curWeapons == null) { return; }
        currentWeapon = curWeapons[0];


        // Use the weapon if we can
        if (hasEnergy && playerNotAttacking && currentWeapon.exhaust == false)
        {
            if(nearTreasure == false)
            {
                FireProcess(currentWeapon, curWeapons);
            }
        }
    }

    public Weapon GetAttackingWeapon()
    {
        return attackingWeapon;
    }


    // Confirmed Fire
    private void FireProcess(Weapon weaponToFire, Weapon[] curWeapons)
    {
        reloading = false;
        canShoot = false;
        attackingWeapon = currentWeapon;

        currentEnergy -= weaponToFire.cost;
        if (currentEnergy < 0) currentEnergy = 0;
        
        if (weaponToFire.isProjectile)
        {
            GameObject hitbox = Instantiate(weaponToFire.hitbox, MoveHitboxStart(), Quaternion.identity);
            BulletBehavior hitboxBehaviour = hitbox.GetComponent<BulletBehavior>();

            float angle = aimscript.GetAimAngle();
            if (angle != -1) 
            {
                hitbox.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); 
            }
            else
            {
                // No enemy, use direction we are facing
                Debug.Log("No enemy");
            }

            if (hitboxBehaviour != null)
            {
                hitboxBehaviour.SetDirection(angle, weaponToFire.hitboxSpeed);
                hitboxBehaviour.SetDuration(weaponToFire.duration);
            }
        }


        playerStateMachine.SetNextState(new MeleeEntryState());

        timeSinceLastAttack = 0;
        weaponToFire.exhaust = true;
        inventory.RotateWeapons(curWeapons); // moves to next weapon

        uiManager.UpdateWeapons(weaponToFire.inventorySlot);

    }

    private Vector3 MoveHitboxStart()
    {
        // This needs to be set dependent on the direction the player is facing

        Vector3 raisedPosition = transform.position;
        raisedPosition.y += .575f;
        return raisedPosition;
    }

}
