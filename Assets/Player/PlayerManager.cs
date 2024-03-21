using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public WeaponManager weaponManager;
    private int wavesCleared;
    private int dungeonDepth;

    public bool gameOver;
    public bool playerActive = true;
    public bool playerStunned, playerInvuln;
    private float stunTimer, invulnTimer;
    [SerializeField] private float stunTime = .5f;
    [SerializeField] private float invulnTime = 1f;

    public GameObject treasurePrefab;

    private Animator animator;
    public PlayerInputActions playerControls;
    private InputAction button1, button2, button3;


    private void Awake()
    {
        playerControls = new PlayerInputActions();
        animator = GetComponent<Animator>();
    }


    private void OnEnable()
    {
        button1 = playerControls.Player.Button1;
        button1.Enable();
        button1.performed += Button1;
        button2 = playerControls.Player.Button2;
        button2.Enable();
        button2.performed += Button2;
        button3 = playerControls.Player.Button3;
        button3.Enable();
        button3.performed += Button3;
    }

    private void OnDisable()
    {
        button1.Disable();
        button1.performed -= Button1;
        button2.Disable();
        button2.performed -= Button2;
        button3.Disable();
        button3.performed -= Button3;
    }

    private void Button1(InputAction.CallbackContext context)
    {
        if (playerActive)
        {
            weaponManager.FireWeapon(1);
        }
    }
    private void Button2(InputAction.CallbackContext context)
    {
        if (playerActive)
        {
            weaponManager.FireWeapon(2);
        }
    }
    private void Button3(InputAction.CallbackContext context)
    {
        if (playerActive)
        {
            weaponManager.FireWeapon(3);
        }
    }


    void Start()
    {
        gameOver = false;
        weaponManager = GetComponent<WeaponManager>();
        wavesCleared = 0;
    }
    void Update()
    {
        UpdateInvulnStunTimer();
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
        invulnTimer = invulnTime;
        stunTimer = stunTime;
    }

    private void UpdateInvulnStunTimer()
    {
        if (playerInvuln)
        {
            invulnTimer -= Time.deltaTime;
            if (invulnTimer <= 0)
            {
                playerInvuln = false;
                invulnTimer = 0; // Reset timer for safety
            }
        }

        if (playerStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0)
            {
                playerStunned = false;
                stunTimer = 0; // Reset timer for safety
            }
        }
    }

}
