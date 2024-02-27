using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public WeaponManager weaponManager;
    private int wavesCleared;
    private int dungeonDepth;

    public bool alive = true;
    public bool playerActive = true;
    public bool playerStunned, playerInvuln;
    private float stunTimer, invulnTimer;
    [SerializeField] private float stunInvulnTime = 1f;

    public GameObject treasurePrefab;
    void Start()
    {
        alive = true;
        weaponManager = GetComponent<WeaponManager>();
        wavesCleared = 0;
    }

    void Update()
    {

        if(playerActive)
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
        }

    }


    public int WavesCleared
    {
        get { return wavesCleared; }
        set { wavesCleared = value; }
    }

    public void IncrementWavesCleared()
    {
        wavesCleared++;
    }

    public void SpawnTreasure(Vector2 treasurePos)
    {
        Instantiate(treasurePrefab, treasurePos, Quaternion.identity);
    }


    public void HitStun()
    {
        playerStunned = true;
        playerInvuln = true;
        invulnTimer = stunInvulnTime;
        
    }

}
