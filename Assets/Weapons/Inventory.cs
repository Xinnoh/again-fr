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

        AddWeaponToInventory(randomWeapon);
    }

    public void AddWeaponToInventory(Weapon weaponToAdd)
    {
        Weapon[] targetArray = null;

        // Determine the correct array to update based on the weapon's inventory slot
        switch (weaponToAdd.inventorySlot)
        {
            case InventorySlot.Light:
                targetArray = AddWeaponToArray(weapons1, weaponToAdd);
                weapons1 = targetArray;
                break;
            case InventorySlot.Heavy:
                targetArray = AddWeaponToArray(weapons2, weaponToAdd);
                weapons2 = targetArray;
                break;
            case InventorySlot.Ranged:
                targetArray = AddWeaponToArray(weapons3, weaponToAdd);
                weapons3 = targetArray;
                break;
            default:
                Debug.LogError("Unknown slot type: " + weaponToAdd.inventorySlot);
                return;
        }

        Debug.Log($"Added {weaponToAdd.name} to inventory.");
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
            AddWeaponToInventory(firstWeaponInstance);
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
