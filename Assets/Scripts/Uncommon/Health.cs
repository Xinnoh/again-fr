using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 5; // Total health
    public bool isEnemy = true;

    // This method subtracts damage from the health
    public void TakeDamage(float damage)
    {
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
            health -= damage;

            BasicAI basicAI = GetComponent<BasicAI>();
            if(basicAI != null)
            {
                basicAI.HitStun(damage);
            }
            return;
        }

        
        PlayerManager playerManager = GetComponent<PlayerManager>();
        if(playerManager != null)
        {
            if (playerManager.playerInvuln)
            {
                return;
            }

            health -= damage;
            playerManager.HitStun();
            return;
        }

        Debug.Log("Health Null");

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
            return;
        }

        PlayerManager playerManager = GetComponent<PlayerManager>();
        if (playerManager != null)
        {
            playerManager.alive = false;
            return;
        }
        //player
    }
}
