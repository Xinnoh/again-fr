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

    private float timeSinceFire = 0f;   // How long since shot, calculate reload time
    private bool canShoot = true;

    public Weapon currentWeapon;


    private StateMachine meleeStateMachine;

    [SerializeField] public Collider2D hitbox;
    [SerializeField] public GameObject Hiteffect;

    public bool uiEnable;


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


    public void FireWeapon(int val)
    {
        Weapon[] curWeapons = null;
        switch (val)
        {
            case 1: curWeapons = inventory.weapons1; break;
            case 2: curWeapons = inventory.weapons2; break;
            case 3: curWeapons = inventory.weapons3; break;
        }

        if (curWeapons == null){ return; }

        currentWeapon = curWeapons[0];

        bool hasEnoughEnergy = currentEnergy < maxEnergy;
        bool weaponIsNotExhausted = !currentWeapon.exhaust;
        bool isCurrentlyIdle = meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState);


        if (hasEnoughEnergy && weaponIsNotExhausted && isCurrentlyIdle)
        {
            FireProcess(currentWeapon, curWeapons);
        }
    }

    // Confirmed Fire
    private void FireProcess(Weapon weaponToFire, Weapon[] curWeapons)
    {
        canShoot = false;

        currentEnergy += weaponToFire.cost;

        if (weaponToFire.isProjectile)
        {
            GameObject hitbox = Instantiate(weaponToFire.hitbox, transform.position, Quaternion.identity);
            BulletBehavior hitboxBehaviour = hitbox.GetComponent<BulletBehavior>();

            // Aim at the nearest enemy
            float angle = aimscript.GetAimAngle();
            if (angle != -1) // Valid angle
            {
                hitbox.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Directly apply rotation
            }
            else
            {
                Debug.Log("No enemy?");
            }


            // Move to MeleeBaseState

            if (hitboxBehaviour != null)
            {
                hitboxBehaviour.SetDirection(angle, weaponToFire.hitboxSpeed);
                hitboxBehaviour.SetDuration(weaponToFire.duration);
            }
        }
        


        meleeStateMachine.SetNextState(new MeleeEntryState());

        timeSinceFire = 0;
        weaponToFire.exhaust = true;
        inventory.RotateWeapons(curWeapons); // moves to next weapon
    }

    private void UpdateCanShoot()
    {

        bool isCurrentlyIdle = meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState);

        if (isCurrentlyIdle && currentEnergy < maxEnergy)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }


    private void UpdateEnergy()
    {

        // If energy has been used
        if (currentEnergy > 0)
        {
            timeSinceFire += Time.deltaTime;
            // Once the reload cooldown time has passed, start reducing currentEnergy
            if (timeSinceFire >= reloadCooldown)
            {
                // This ensures shooting is disabled only after starting the reload process
                canShoot = false;
                // Decrease currentEnergy over time
                currentEnergy -= reloadRate * Time.deltaTime;
                // Clamp currentEnergy to ensure it does not go below 0
                currentEnergy = Mathf.Max(currentEnergy, 0);
            }
        }

        // If currentEnergy has reached 0, enable shooting and reset weapon exhaustion
        if (currentEnergy <= 0)
        {
            // Enable shooting
            canShoot = true;
            // Reset time since last fire to prevent immediate energy reduction on next fire
            timeSinceFire = 0;
            // Reset the exhaustion of all weapons to allow them to be used again
            inventory.ResetWeaponExhaustion();
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
}
