using Edgar.Unity.Examples.Gungeon;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 5; // Total health
    public bool isEnemy = true;
    [HideInInspector] public float maxHealth;


    private HealthBarEnemy healthBar;
    private UiManager playerUI;

    private void Start()
    {
        maxHealth = health;
        healthBar = GetComponentInChildren<HealthBarEnemy>();
    }


    private void Update()
    {
        if(health <= 0)
        {
            Debug.Log(health);
        }
    }

    public void TakeDamage(float damage)
    {
        HitStun(damage);

        if (isEnemy)
        {
            healthBar.UpdateHealth(health, maxHealth);
        }
        else
        {
            playerUI.UpdateHealth();
        }

        if (health <= 0)
        {
            GungeonEnemy enemyScript = GetComponent<GungeonEnemy>();
            if (enemyScript != null)
            {
                enemyScript.returnDead();
            }

            Destroy(gameObject);
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
        if (isEnemy)
        {
            return;
        }

        else
        {
            PlayerManager playerManager = GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.alive = false;
                return;
            }
        }
        //player
    }
}
