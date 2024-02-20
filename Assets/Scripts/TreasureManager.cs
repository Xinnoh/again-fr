using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public TMP_Text reward1, reward2, reward3;
    private Weapon[] selectedWeapons = new Weapon[3];

    [SerializeField] private float interactionDistance = 1f;

    public GameObject treasureUI;
    private PlayerManager playerManager;
    private GameObject player; // Reference to the player
    public bool hasInteracted = false; // Tracks if the object has been interacted with

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find and assign the player object
        playerManager = player.GetComponent<PlayerManager>();

        List<string> weaponNames = new List<string>(); // Temporary list to hold weapon names

        Weapon newWeapon = GetRandomWeaponFromAnyCategory();

        for (int i = 0; i < 3; i++)
        {
            while (IsDuplicate(newWeapon))
            {
                newWeapon = GetRandomWeaponFromAnyCategory();
            }

            selectedWeapons[i] = newWeapon;
            weaponNames.Add(newWeapon.name); // Add the weapon's name to the list
        }
        DisplayWeaponNames(weaponNames);
        if (treasureUI != null) treasureUI.SetActive(false);

    }

    private void Update()
    {

        if (Vector2.Distance(player.transform.position, transform.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasInteracted)
            {
                OnTreasureInteracted();
            }
        }
    }


    private Weapon GetRandomWeaponFromAnyCategory()
    {
        // Randomly select a category
        InventorySlot[] slots = { InventorySlot.Light, InventorySlot.Heavy, InventorySlot.Ranged };
        InventorySlot selectedSlot = slots[Random.Range(0, slots.Length)];

        // Get a random weapon from the selected category
        return WeaponList.GetRandomWeapon(selectedSlot);
    }

    private bool IsDuplicate(Weapon weapon)
    {
        // Check if the weapon parameter is null to avoid comparing null values
        if (weapon == null) return false;

        for (int i = 0; i < selectedWeapons.Length; i++)
        {
            // Check for null to avoid comparing with unassigned array slots
            if (selectedWeapons[i] != null && selectedWeapons[i].name == weapon.name)
            {
                return true;
            }
        }
        return false;
    }

    private void DisplayWeaponNames(List<string> names)
    {
        if (reward1 == null || reward2 == null || reward3 == null)
        {
            return; // WHY DOES THE CODE TRY TO DO IT TWICE I HAVE NO IDEA
        }

        if (names.Count >= 3)
        {
            reward1.text = names[0] ?? "Name Missing";
            reward2.text = names[1] ?? "Name Missing";
            reward3.text = names[2] ?? "Name Missing";
        }
    }


    public void OnTreasureInteracted()
    {
        hasInteracted = true;
        if (treasureUI != null) treasureUI.SetActive(true);
        playerManager.PlayerActive = false;
    }

    public void ButtonInput(int button)
    {
        Inventory inventory = FindObjectOfType<Inventory>(); // Find the player's inventory script in the scene
        if (inventory != null && button >= 1 && button <= 3)
        {
            inventory.AddWeaponToInventory(selectedWeapons[button - 1]);
            playerManager.PlayerActive = true;
            Destroy(gameObject);

        }
    }
}
