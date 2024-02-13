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

    private float aimOffset = 90f;

    // Start is called before the first frame update
    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
        aimScript = GetComponent<Target>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        enemiesAlive = aimScript.GetAimAngle() != -1 ? true : enemiesAlive;

        if (meleeStateMachine.CurrentState != null)
        {
            playerNotAttacking = meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState);
        }

        if (playerNotAttacking && enemiesAlive)
        {
            RotateHitbox();
        }

        enemiesAlive = false;



        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            hitboxVisual.color = new Color(hitboxVisual.color.r, hitboxVisual.color.g, hitboxVisual.color.b, 1f);

        }
        else
        {
            hitboxVisual.color = new Color(hitboxVisual.color.r, hitboxVisual.color.g, hitboxVisual.color.b, 0.1f);
        }


    }

    private void RotateHitbox()
    {
        Vector3 aimVector = aimScript.GetAimVector();
        float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
        hitboxRotater.transform.rotation = Quaternion.Euler(0f, 0f, angle + aimOffset);
    }

}
