using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
// Stores the data used to create each weapon

public class Weapon : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("The name of the weapon.")]
    public new string name;

    [Tooltip("Description of the weapon.")]
    public string description;

    [Tooltip("Artwork representing the weapon.")]
    public Sprite artwork;

    [Tooltip("State to enter upon using this weapon.")]
    public string weaponState;

    [Tooltip("Tells us what weapon it is")]
    public int ID; // You might choose to hide this if it's set programmatically

    [Tooltip("The inventory slot this weapon occupies.")]
    public InventorySlot inventorySlot;

    [Tooltip("Where should this weapon spawn?")]
    public int rarity;


    [Header("Combat Stats")]
    [Tooltip("Cost to fire the weapon.")]
    public int cost;

    [Tooltip("Damage dealt by the weapon.")]
    public int damage;

    [Tooltip("Whether the weapon has been used in this combo.")]
    public bool exhaust;

    [Tooltip("How long this stuns an enemy.")]
    public float hitStun;




    [Header("Ranged Weapons")]
    [Tooltip("Does this weapon spawn projectiles.")]
    public bool isProjectile;

    [Tooltip("Prefab of the hitbox/attack effect.")]
    public GameObject hitbox;

    [Range(0, 20)]
    [Tooltip("Speed of the hitbox, 0 for melees.")]
    public int hitboxSpeed;

    [Tooltip("How long the hitbox lasts.")]
    public float duration;

    [Tooltip("Delay before the weapon spawns its hitbox.")]
    public float spawnDelay;

    [Tooltip("How many bullets.")]
    public int bulletCount = 1;

    [Tooltip("Accuracy of the projectile.")]
    public float bulletSpread = 0;

    [Tooltip("Time between shots for rapid fire weapons. (eg smg 0.1)")]
    public float fireInterval = 0;


    [Header("Movement Modifier")]
    [Tooltip("How long it takes before the weapon can be used again.")]
    public float recoverTime;

    [Range(0f, 2f)]
    [Tooltip("How fast the player moves while attacking.")]
    public float speedMultiplier;


    [Header("Momentum Effects")]
    [Tooltip("Direction the player is sent upon using the weapon (eg recoil). Positive values for forward, negative for backward.")]
    public float momentum;

    [Tooltip("Delay before momentum effect takes place.")]
    public float momentumDelay;

    [Tooltip("How far away the object should be spawned from the player.")]
    public float distanceOffset;
}
