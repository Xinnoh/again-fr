using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    public static WeaponList Instance { get; private set; } // Static singleton instance

    public Weapon[] lightList, heavyList, rangedList, starterList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep alive across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Weapon GetRandomWeapon(InventorySlot slotType)
    {
        Weapon[] sourceArray = null;

        // Ensure there's an instance of WeaponList available
        if (Instance == null)
        {
            Debug.LogError("WeaponList instance is not set.");
            return null;
        }

        // Select the correct weapon list based on slotType
        switch (slotType)
        {
            case InventorySlot.Light:
                sourceArray = Instance.lightList;
                break;
            case InventorySlot.Heavy:
                sourceArray = Instance.heavyList;
                break;
            case InventorySlot.Ranged:
                sourceArray = Instance.rangedList;
                break;
                // Add more cases if necessary
        }

        if (sourceArray != null && sourceArray.Length > 0)
        {
            int randomIndex = Random.Range(0, sourceArray.Length);
            return sourceArray[randomIndex]; // Return the randomly selected weapon
        }
        else
        {
            Debug.LogWarning($"The weapon list for {slotType} is empty or not found.");
            return null; // Return null if no weapon is found or list is empty
        }
    }
}

public enum GunTypes
{
    Punch,
    Kick,
    HeavyHammer,
    HeavySlam,
    GunFast,
    GunSlow
}

public enum InventorySlot
{
    Light,
    Heavy,
    Ranged
}
