using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    public static WeaponList Instance { get; private set; } 

    public Weapon[] lightList, heavyList, rangedList, savedLight, savedHeavy, savedRanged;

    private GameObject player;
    private Inventory inventory;

    [HideInInspector] public bool weaponsSaved;

    void Awake()
    {
        if (Instance == null)
        {
            // Unity will tell you this isn't needed. Unity is wrong.
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearSavedWeapons()
    {
        savedLight = new Weapon[0];
        savedHeavy = new Weapon[0];
        savedRanged = new Weapon[0];
        weaponsSaved = false;
    }


    public void LoadWeapons()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();

        inventory.weapons1 = savedLight;
        inventory.weapons2 = savedHeavy;
        inventory.weapons3 = savedRanged;
    }

    public void SaveWeapons(Weapon[] light, Weapon[] heavy, Weapon[] ranged)
    {
        savedLight = light;
        savedHeavy = heavy;
        savedRanged = ranged;

        weaponsSaved = true;
    }








    public static Weapon GetRandomWeapon(InventorySlot slotType)
    {
        Weapon[] sourceArray = null;

        if (Instance == null)
        {
            Debug.LogError("WeaponList instance is not set.");
            return null;
        }

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
        }

        if (sourceArray != null && sourceArray.Length > 0)
        {
            int randomIndex = Random.Range(0, sourceArray.Length);
            return sourceArray[randomIndex]; 
        }
        else
        {
            Debug.LogWarning($"The weapon list for {slotType} is empty or not found.");
            return null; 
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
