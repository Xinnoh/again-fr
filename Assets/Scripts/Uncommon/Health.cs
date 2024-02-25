using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 5; // Total health
    public bool isEnemy = true;

    // This method subtracts damage from the health
    public void TakeDamage(float damage)
    {
        health -= damage;

        HitStun(damage);

        if (health <= 0)
        {
            Die();
        }
    }

    void HitStun(float damage)
    {
        if(isEnemy)
        {
            BasicAI basicAI = GetComponent<BasicAI>();
            if(basicAI != null)
            {
                basicAI.HitStun(damage);
            }
        }

        else
        {
            //player
        }

    }

    void Die()
    {
        if(isEnemy)
        {
            GungeonEnemy enemyScript = GetComponent<GungeonEnemy>();
            if (enemyScript != null)
            {
                enemyScript.returnDead();
            }

            Destroy(gameObject);
        }

        else
        {
            // is player
        }
    }
}
