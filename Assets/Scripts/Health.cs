using Edgar.Unity.Examples.Gungeon;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 5; // Total health
    public bool isEnemy = true;
    [HideInInspector] public float maxHealth;

    private PlayerManager playerManager;
    private HealthBarEnemy healthBar;
    private UiManager playerUI;

    [SerializeField] private float playerIFrames;

    private void Start()
    {
        maxHealth = health;
        healthBar = GetComponentInChildren<HealthBarEnemy>();
        playerUI = GetComponent<UiManager>();

        playerManager = GetComponent<PlayerManager>();
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
            if (isEnemy)
            {
                Destroy(gameObject);
            }

            else
            {
                playerManager.gameOver = true;
            }
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

        
        if(playerManager != null)
        {
            if (playerManager.playerInvuln)
            {
                return;
            }

            Debug.Log(damage + "taken");

            health -= damage;
            playerManager.HitStun();
            return;
        }


    }

    IEnumerator InvulnerabilityPeriod()
    {
        playerManager.playerInvuln = true;
        yield return new WaitForSeconds(playerIFrames);
        playerManager.playerInvuln = false;
    }

    void Die()
    {
        if (isEnemy)
        {
            return;
        }

        else
        {

        }
        //player
    }
}
