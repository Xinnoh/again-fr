using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Shoot shoot;
    public WeaponManager weaponManager;
    public float reloadTime = 0.5f; // Time in seconds between shots
    private float reloadTimer = 0f; // Timer to track time since last shot

    void Start()
    {
        shoot = GetComponent<Shoot>();
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {


        if (Input.GetKeyDown(KeyCode.I))
        {
            weaponManager.FireWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            weaponManager.FireWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            weaponManager.FireWeapon(3);
        }


        // Update the reload timer
        if (reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
        }
    }

    private void TryShoot()
    {
        // Check if the reload timer allows for shooting
        if (reloadTimer <= 0)
        {
            shoot.ShootBullet(); // Call the shooting function
            reloadTimer = reloadTime; // Reset the reload timer
        }
        else
        {
        }
    }
}
