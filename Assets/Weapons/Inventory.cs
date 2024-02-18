using UnityEngine;

public class Inventory : MonoBehaviour
{
    // manages what items we have in our inventory

    public WeaponList weaponListPrefab; 

    public Weapon[] weapons1, weapons2, weapons3;

    public Weapon heldWeaponLight, heldWeaponHeavy, heldWeaponRanged;
    private void Start()
    {
        AddFirstWeapon(InventorySlot.Light);
        AddFirstWeapon(InventorySlot.Heavy);
        AddFirstWeapon(InventorySlot.Ranged);
    }

    private void Update()
    {
        KeyboardInput();
    }

    private void AddRandomWeapon(InventorySlot slotType)
    {
        Weapon randomWeapon = WeaponList.GetRandomWeapon(slotType);
        if (randomWeapon != null)
        {
            switch (slotType)
            {
                case InventorySlot.Light:
                    weapons1 = AddWeaponToArray(weapons1, randomWeapon);
                    heldWeaponLight = randomWeapon;
                    break;
                case InventorySlot.Heavy:
                    weapons2 = AddWeaponToArray(weapons2, randomWeapon);
                    heldWeaponLight = randomWeapon;
                    break;
                case InventorySlot.Ranged:
                    weapons3 = AddWeaponToArray(weapons3, randomWeapon);
                    heldWeaponLight = randomWeapon;
                    break;
            }
            Debug.Log($"Added {randomWeapon.name} to inventory.");
        }
        else
        {
            Debug.LogWarning("Failed to add random weapon: No weapon found.");
        }
    }
    private Weapon[] AddWeaponToArray(Weapon[] weaponArray, Weapon weaponToAdd)
    {
        int length = weaponArray != null ? weaponArray.Length : 0;
        Weapon[] newArray = new Weapon[length + 1];
        if (weaponArray != null && weaponArray.Length > 0)
        {
            weaponArray.CopyTo(newArray, 0);
        }
        // Instantiate a new copy of the weaponToAdd ScriptableObject and add it to the array
        newArray[length] = ScriptableObject.Instantiate(weaponToAdd);
        return newArray;
    }



    // Cycles player inventory to next weapon
    public void RotateWeapons(Weapon[] weapons)
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

    // use keyboard to add random weapons to the inventory for testing
    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { AddRandomWeapon(InventorySlot.Light); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { AddRandomWeapon(InventorySlot.Heavy); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { AddRandomWeapon(InventorySlot.Ranged); }
    }

    // adds the first weapon in each weaponlist to the player inventory as starter weapons
    private void AddFirstWeapon(InventorySlot slotType)
    {
        Weapon[] sourceArray = GetSourceArray(slotType);
        if (sourceArray != null && sourceArray.Length > 0)
        {
            Weapon firstWeaponInstance = Instantiate(sourceArray[0]); 

            switch (slotType)
            {
                case InventorySlot.Light:
                    weapons1 = AddWeaponToArray(weapons1, firstWeaponInstance);
                    heldWeaponLight = firstWeaponInstance; 
                    break;
                case InventorySlot.Heavy:
                    weapons2 = AddWeaponToArray(weapons2, firstWeaponInstance);
                    heldWeaponHeavy = firstWeaponInstance;
                    break;
                case InventorySlot.Ranged:
                    weapons3 = AddWeaponToArray(weapons3, firstWeaponInstance);
                    heldWeaponRanged = firstWeaponInstance; 
                    break;
            }
        }
    }

    private Weapon[] GetSourceArray(InventorySlot slotType)
    {
        switch (slotType)
        {
            case InventorySlot.Light: return weaponListPrefab.lightList;
            case InventorySlot.Heavy: return weaponListPrefab.heavyList;
            case InventorySlot.Ranged: return weaponListPrefab.rangedList;
            default: return null;
        }
    }

}
