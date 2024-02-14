using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 5; // Total health
    bool isEnemy = true;

    // This method subtracts damage from the health
    public void TakeDamage(int damage)
    {
        health -= damage;

        // Check if health has dropped below zero and if so, destroy the object
        if (health <= 0)
        {
            Die();
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
