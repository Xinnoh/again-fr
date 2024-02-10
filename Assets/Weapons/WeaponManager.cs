using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    public Canvas canvas;

    public TMP_Text energyDisplay;
    public TMP_Text weapon1Display, weapon2Display, weapon3Display;
    public TMP_Text canShootDisplay;

    public Weapon[] weapons1, weapons2, weapons3;

    private int index1, index2, index3;

    public float currentEnergy;
    public int maxEnergy = 51;
    public int startingEnergy = 51;

    public float reloadRate = 1f;
    public float reloadCooldown = 2f;

    private PlayerMovement playerMovement;
    private Target aimscript;

    private float timeSinceFire = 0f;
    private bool canShoot = true;

    private float timeSinceLastShot = 0f;
    private float lastWeaponRecoverTime = 0f;


    public bool uiEnable;
    private void Start()
    {
        currentEnergy = 0;
        UpdateUI(); // Initialize UI
        aimscript = GetComponent<Target>();
        playerMovement = GetComponent<PlayerMovement>();
        ResetWeaponExhaustion();
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
            case 1: curWeapons = weapons1; break;
            case 2: curWeapons = weapons2; break;
            case 3: curWeapons = weapons3; break;
        }

        if (canShoot && curWeapons != null && currentEnergy < maxEnergy && !curWeapons[0].exhaust)
        {
            FireProcess(curWeapons[0], curWeapons); 
        }
    }

    // Confirmed Fire
    private void FireProcess(Weapon weaponToFire, Weapon[] curWeapons)
    {
        canShoot = false;
        lastWeaponRecoverTime = weaponToFire.recoverTime;
        timeSinceLastShot = 0f;

        currentEnergy += weaponToFire.cost;
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
            Debug.Log("Miss");
        }

        if (hitboxBehaviour != null)
        {
            hitboxBehaviour.SetDirection(angle, weaponToFire.hitboxSpeed);
            hitboxBehaviour.SetDuration(weaponToFire.duration);
        }

        // Apply movement reduction if needed
        if (playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(weaponToFire.speedMultiplier, weaponToFire.recoverTime);
        }


        timeSinceFire = 0;
        weaponToFire.exhaust = true;
        RotateWeapons(curWeapons); // Assuming RotateWeapons is a method that modifies the order or state of weapons
    }




    private void UpdateCanShoot()
    {
        // Update the time since the last shot
        timeSinceLastShot += Time.deltaTime;

        // Check if enough time has passed since the last shot based on the last weapon's recoverTime
        if (timeSinceLastShot >= lastWeaponRecoverTime && currentEnergy < maxEnergy)
        {
            canShoot = true;
        }
        else
        {
            canShoot = false;
        }
    }


    // After firing, get a new weapon
    private void RotateWeapons(Weapon[] weapons)
    {
        if (weapons.Length > 1)
        {
            Weapon temp = weapons[0];
            for (int i = 1; i < weapons.Length; i++)
            {
                weapons[i - 1] = weapons[i];
            }
            weapons[weapons.Length - 1] = temp;
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
            ResetWeaponExhaustion();
        }

    }

    public void ResetWeaponExhaustion()
    {
        foreach (Weapon weapon in weapons1)
        {
            if (weapon != null)
            {
                weapon.exhaust = false;
            }
        }

        foreach (Weapon weapon in weapons2)
        {
            if (weapon != null)
            {
                weapon.exhaust = false;
            }
        }

        foreach (Weapon weapon in weapons3)
        {
            if (weapon != null)
            {
                weapon.exhaust = false;
            }
        }
    }

    private void UpdateUI()
    {
        if (uiEnable)
        {
            energyDisplay.text = $"Energy: {Mathf.Round(currentEnergy)} / {maxEnergy}";
            canShootDisplay.text = $"Can Shoot: {canShoot}";

            weapon1Display.text = weapons1.Length > 0 ? $"{weapons1[0].name}" : "Weapon 1: N/A";
            weapon2Display.text = weapons2.Length > 0 ? $"{weapons2[0].name}" : "Weapon 2: N/A";
            weapon3Display.text = weapons3.Length > 0 ? $"{weapons3[0].name}" : "Weapon 3: N/A";
        }
    }
}
