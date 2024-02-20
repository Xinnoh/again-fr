using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Shoot shoot;
    public WeaponManager weaponManager;
    private int wavesCleared;
    private int dungeonDepth;

    public bool PlayerActive = true;



    public GameObject treasurePrefab;
    void Start()
    {
        shoot = GetComponent<Shoot>();
        weaponManager = GetComponent<WeaponManager>();
        wavesCleared = 0;

    }

    void Update()
    {

        if(PlayerActive)
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

}
