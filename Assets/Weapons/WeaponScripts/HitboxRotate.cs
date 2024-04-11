using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxRotate : MonoBehaviour
{
    public GameObject hitboxRotater;
    public SpriteRenderer hitboxVisual;


    private bool playerNotAttacking, enemiesAlive;
    private StateMachine meleeStateMachine;
    private Target aimScript;
    protected Animator animator;
    private PlayerMovement playerMovement;

    private Vector2 aimVector;
    private float aimOffset = 90f;

    // Start is called before the first frame update
    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
        aimScript = GetComponent<Target>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVariables();

        // If the player isn't attacking, aim the hitbox either at the enemy, else in the direction they face

        if (playerNotAttacking)
        {
            if(enemiesAlive)
            {
                aimVector = aimScript.GetAimVector();
            }
            else
            {
                aimVector = playerMovement.GetLastMovementDirection();
            }

            AimHitbox(aimVector);
        }

        UpdateHitboxColor();
    }


    private void AimHitbox(Vector2 aimVector)
    {
        float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
        hitboxRotater.transform.rotation = Quaternion.Euler(0f, 0f, angle + aimOffset);
    }


    private void UpdateHitboxColor()
    {
        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            hitboxVisual.color = new Color(hitboxVisual.color.r, hitboxVisual.color.g, hitboxVisual.color.b, 1f);
        }
        else
        {
            hitboxVisual.color = new Color(hitboxVisual.color.r, hitboxVisual.color.g, hitboxVisual.color.b, 0f);
        }
    }


    private void UpdateVariables()
    {
        enemiesAlive = aimScript.GetAimAngle() != -1 ? true : enemiesAlive;

        if (meleeStateMachine.CurrentState != null)
        {
            playerNotAttacking = meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState);
        }
    }
}
