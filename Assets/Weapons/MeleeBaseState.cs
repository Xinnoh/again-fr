using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;
// Used as the base script for all weapons.

public class MeleeBaseState : State
{
    // How long this state should be active for before moving on
    public float duration;
    // Cached animator component
    protected Animator animator;
    // bool to check whether or not the next attack in the sequence should be played or not
    protected bool shouldCombo;
    // The attack index in the sequence of attacks
    protected int attackIndex;

    // What the next attack should be
    protected string nextAttack;

    protected float movementModifier;

    // The cached hit collider component of this attack
    protected Collider2D hitCollider;
    // Cached already struck objects of said attack to avoid overlapping attacks on same target
    private List<Collider2D> collidersDamaged;
    // The Hit Effect to Spawn on the afflicted Enemy
    private GameObject HitEffectPrefab;
    //Current weapon
    protected Weapon curWeapon;
    //Directionality
    protected Target aimScript;
    protected PlayerMovement playerMovement;
    protected GameObject player;

    // closest enemy direction and distance
    protected Vector2 enemyDirection;
    protected float enemyDistance;

    // have we picked a target (used for attacks that attack over time, prevents switchign)
    protected bool targetAcquired;

    // Input buffer Timer
    private float AttackPressedTimer = 0;

    // Allow attack cancel if we hit enemy
    protected bool hasHit;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator = GetComponent<Animator>();
        collidersDamaged = new List<Collider2D>();
        hitCollider = GetComponent<WeaponManager>().hitbox;
        HitEffectPrefab = GetComponent<WeaponManager>().Hiteffect;
        curWeapon = GetComponent<WeaponManager>().attackingWeapon;
        aimScript = GetComponent<Target>();
        playerMovement = GetComponent<PlayerMovement>();

        player = GameObject.FindGameObjectWithTag("Player");

        targetAcquired = false;
        hasHit = false;
        playerMovement.SetSpeedModifier(curWeapon.speedMultiplier);

        duration = curWeapon.recoverTime;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;

        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }


        if (Input.GetMouseButtonDown(0))
        {
            AttackPressedTimer = .3f;
        }

        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
        {
            shouldCombo = true;
        }
    }

    public override void OnExit()
    {
        playerMovement.ResetSpeedMultiplier();
        base.OnExit();
    }

    protected void Attack()
    {
        Collider2D[] collidersToDamage = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {

            if (!collidersDamaged.Contains(collidersToDamage[i]))
            {
                TeamComponent hitTeamComponent = collidersToDamage[i].GetComponentInChildren<TeamComponent>();

                // Only check colliders with a valid Team Componnent attached
                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    GameObject.Instantiate(HitEffectPrefab, collidersToDamage[i].transform);

                    // Deal damage to enemy

                    Health health = collidersToDamage[i].gameObject.GetComponent<Health>();
                    if (health != null)
                    {
                        health.TakeDamage(1);
                    }

                    hasHit = true;

                    Debug.Log("Enemy Has Taken:" + attackIndex + "Damage");
                    collidersDamaged.Add(collidersToDamage[i]);
                }
            }
        }
    }

    protected void FindEnemy()
    {
        enemyDirection = aimScript.GetAimVector();

        targetAcquired = true;
    }


}