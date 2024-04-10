using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class hHurricane : MeleeBaseState
{
    private Vector2 originalSize;
    private BoxCollider2D hitbox; // Reference to the BoxCollider2D


    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        duration = 1.4f;
        hitbox = hitCollider.GetComponent<BoxCollider2D>();

        originalSize = hitbox.size;
        hitbox.size *= 2;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        Attack();

        Collider2D[] colliders = Physics2D.OverlapBoxAll(hitbox.bounds.center, hitbox.bounds.size, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("WallTiles"))
            {
                stateMachine.SetNextStateToMain();
                return;
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        hitbox.size = originalSize;

    }

}