using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    private WeaponManager weaponManager;
    private Health healthScript;
    private PlayerMovement playerMovement;
    private Inventory inventory;

    // Health
    [SerializeField] private RectTransform healthMask;
    private float currHealthWidth, maxHealthWidth;
    private float currHealthPercent, maxHealth;
    
    // Energy
    [SerializeField] private RectTransform energyMask;
    private float currEnergyWidth, maxEnergyWidth;
    private float currEnergyPercent, maxEnergy;
    [SerializeField] private GameObject reloading;
    private bool reloadBool;

    // Dashes
    [SerializeField] private RectTransform dashMask;
    private float currDashWidth, maxDashWidth;
    private float currDashPercent, maxDash;

    [Header("Inventory")]

    // Inventory
    [SerializeField] private TMP_Text lightCurrentWeapon;
    [SerializeField] private TMP_Text heavyCurrentWeapon, rangedCurrentWeapon, lightInventory, heavyInventory, rangedInventory;

    void Start()
    {
        weaponManager= GetComponent<WeaponManager>();
        healthScript = GetComponent<Health>();
        playerMovement = GetComponent<PlayerMovement>();
        inventory = GetComponent<Inventory>();

        maxEnergyWidth = energyMask.sizeDelta.x;
        currEnergyWidth = maxEnergyWidth;
        maxEnergy = weaponManager.maxEnergy;

        maxHealthWidth = healthMask.sizeDelta.x;
        currHealthWidth = maxHealthWidth;
        maxHealth = healthScript.health;

        maxDashWidth = dashMask.sizeDelta.x;
        currDashWidth = maxDashWidth;
        maxDash = playerMovement.maxDashes;

        // Set reloading to false by default
        reloading.SetActive(false);
    }

    void Update()
    {
        UpdateEnergy();
        UpdateDash();
        UpdateReloading();
    }

    public void UpdateWeapons(InventorySlot slotType)
    {
        switch (slotType)
        {
            case InventorySlot.Light:
                lightInventory.text = inventory.GetWeaponNames(InventorySlot.Light);
                lightCurrentWeapon.text = inventory.weapons1.Length > 0 ? inventory.weapons1[0].name : "No Weapon";
                break;
            case InventorySlot.Heavy:
                heavyInventory.text = inventory.GetWeaponNames(InventorySlot.Heavy);
                heavyCurrentWeapon.text = inventory.weapons2.Length > 0 ? inventory.weapons2[0].name : "No Weapon";
                break;
            case InventorySlot.Ranged:
                rangedInventory.text = inventory.GetWeaponNames(InventorySlot.Ranged);
                rangedCurrentWeapon.text = inventory.weapons3.Length > 0 ? inventory.weapons3[0].name : "No Weapon";
                break;
        }
    }

    // Only called when player health changes
    public void UpdateHealth()
    {
        // Only run the code if the value has changed
        if(currHealthPercent == healthScript.health / maxHealth)
        { return; }

        currHealthPercent = healthScript.health / maxHealth;
        currHealthWidth = currHealthPercent * maxHealthWidth;
        healthMask.sizeDelta = new Vector2(currHealthWidth, healthMask.sizeDelta.y);
    }

    private void UpdateEnergy()
    {
        if(currEnergyPercent == weaponManager.currentEnergy / maxEnergy)
        { return; }

        currEnergyPercent = weaponManager.currentEnergy / maxEnergy;
        currEnergyWidth = currEnergyPercent * maxEnergyWidth;
        energyMask.sizeDelta = new Vector2(currEnergyWidth, energyMask.sizeDelta.y);
    }


    private void UpdateDash()
    {

        if(currDashPercent == playerMovement.currentDashes / maxDash)
        { return; }

        currDashPercent = playerMovement.currentDashes / maxDash;
        currDashWidth= currDashPercent * maxDashWidth;
        dashMask.sizeDelta = new Vector2(currDashWidth, dashMask.sizeDelta.y);
    }

    private void UpdateReloading()
    {
        if (weaponManager.reloading == reloadBool)
        { return; }

        reloadBool = weaponManager.reloading;

        if (reloadBool)
        {
            reloading.SetActive(true);
        }
        else
        {
            reloading.SetActive(false);
        }

    }

}
