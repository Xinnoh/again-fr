using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using TMPro;
using Unity.VisualScripting;

public class WeaponManager : MonoBehaviour
{
    #region Variables
    public Canvas canvas;

    public TMP_Text energyDisplay;
    public TMP_Text weapon1Display, weapon2Display, weapon3Display;
    public TMP_Text canShootDisplay;


    public float currentEnergy;
    public int maxEnergy = 51;
    public int startingEnergy = 51;

    public float reloadRate = 1f;
    public float reloadCooldown = 2f;

    private PlayerMovement playerMovement;
    private Target aimscript;
    private Inventory inventory;

    private float timeSinceLastAttack = 0f;   // How long since shot, calculate reload time
    private bool canShoot = true;

    private bool hasEnergy, playerNotAttacking, reloading;

    private Weapon currentWeapon;
    public Weapon attackingWeapon;


    private StateMachine meleeStateMachine;

    [SerializeField] public Collider2D hitbox;
    [SerializeField] public GameObject Hiteffect;

    public bool uiEnable;

    #endregion

    private void Start()
    {
        currentEnergy = 0;
        aimscript = GetComponent<Target>();
        playerMovement = GetComponent<PlayerMovement>();
        inventory = GetComponent<Inventory>();
        inventory.ResetWeaponExhaustion();

        meleeStateMachine = GetComponent<StateMachine>();
        UpdateUI(); 
    }

    private void Update()
    {
        UpdateEnergy();
        UpdateCanShoot();
        UpdateUI(); 
    }

    private void UpdateEnergy()
    {
        // timeSinceFire is set to 0 whenever we attack

        if (currentEnergy > 0)
        {
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack >= reloadCooldown)
            {
                // This ensures shooting is disabled only after starting the reload process
                canShoot = false;
                reloading = true;
                // Decrease currentEnergy over time
                currentEnergy -= reloadRate * Time.deltaTime;
                // Clamp currentEnergy to ensure it does not go below 0
                currentEnergy = Mathf.Max(currentEnergy, 0);
            }
            return;
        }

        if (timeSinceLastAttack != 0 && currentEnergy <= 0)
        {
            reloading = false;
            canShoot = true;
            inventory.ResetWeaponExhaustion();
            timeSinceLastAttack = 0;
        }
    }

    private void UpdateCanShoot()
    {
        playerNotAttacking = meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState);
        hasEnergy = currentEnergy < maxEnergy;

        if (playerNotAttacking && hasEnergy && !reloading)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }


    private void UpdateUI()
    {
        if (uiEnable)
        {
            energyDisplay.text = $"Energy: {Mathf.Round(currentEnergy)} / {maxEnergy}";
            canShootDisplay.text = $"Can Shoot: {canShoot}";

            weapon1Display.text = inventory.weapons1.Length > 0 ? $"{inventory.weapons1[0].name}" : "Weapon 1: N/A";
            weapon2Display.text = inventory.weapons2.Length > 0 ? $"{inventory.weapons2[0].name}" : "Weapon 2: N/A";
            weapon3Display.text = inventory.weapons3.Length > 0 ? $"{inventory.weapons3[0].name}" : "Weapon 3: N/A";
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
        if (hasEnergy && playerNotAttacking && currentWeapon.exhaust == false && !reloading)
        {
            FireProcess(currentWeapon, curWeapons);
        }
    }


    // Confirmed Fire
    private void FireProcess(Weapon weaponToFire, Weapon[] curWeapons)
    {
        canShoot = false;
        attackingWeapon = currentWeapon;

        currentEnergy += weaponToFire.cost;

        if (weaponToFire.isProjectile)
        {
            GameObject hitbox = Instantiate(weaponToFire.hitbox, transform.position, Quaternion.identity);
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


        meleeStateMachine.SetNextState(new MeleeEntryState());

        timeSinceLastAttack = 0;
        weaponToFire.exhaust = true;
        inventory.RotateWeapons(curWeapons); // moves to next weapon
    }
}
