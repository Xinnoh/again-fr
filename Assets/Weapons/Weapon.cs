using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]

public class Weapon : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite artwork;
    public int ID;

    public int cost; // cost to fire

    public int damage; // how much it does
    public int hitboxSpeed = 0; // 0 for melees

    public float duration = .5f; // how long the hitbox lasts for

    public int slot; // which slot it goes

    public bool exhaust; // has it been used in this combo

    public GameObject hitbox; // what does it attack with

    public float recoverTime; // how long the reload time
    public float speedMultiplier = 1f;  // How much the player speed changes after firing

    public float spawnDelay; // how long to delay before hitbox

    public float distanceOffset; // how far away the object should be spawned


    public float momentum; // what direction is the player sent by the weapon, forward or back
    public float momentumDelay; // how long until momentum takes place


}
