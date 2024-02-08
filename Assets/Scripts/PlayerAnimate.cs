using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public Animator animator;
    Vector2 movement;
    private Target aimScript;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        aimScript= GetComponent<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
            

        if(aimScript.GetAimAngle() != -1)
        {
            animator.SetBool("Enemy", true);

            Vector3 aimVector = aimScript.GetAimVector();

            animator.SetFloat("AimX", aimVector.x);
            animator.SetFloat("AimY", aimVector.y);

        }
        else
        {
            animator.SetBool("Enemy", false);
        }

    }
}
