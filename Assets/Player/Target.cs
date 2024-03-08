using UnityEngine;
using UnityEngine.UIElements;

public class Target : MonoBehaviour
{
    private PlayerMovement playerMovement;

    // a bunch of public methods to call for the player

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }


    // Used for rotating objects
    public float GetAimAngle()
    {
        Vector3 aimDirection = GetAimVector();
        
        if (aimDirection.Equals(Vector3.zero)) return -1;

        return Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    }

    // Used for movement of objects
    public Vector3 GetAimVector()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (EnemiesAlive(enemies))
        {
            return AimAtEnemy();
        }
        else
        {
            return AimAtFeet();
        }
    }

    private Vector3 AimAtFeet()
    {
        Vector3 lastDirection = playerMovement.GetLastMovementDirection();

        return lastDirection;
    }


    private Vector3 AimAtEnemy()
    {
        Vector3 nearestEnemyPosition = GetNearestEnemyPos();

        if (nearestEnemyPosition == Vector3.zero)
        {
            return Vector3.zero;
        }

        Vector3 currentPosition = transform.position;

        if (nearestEnemyPosition != null)
        {
            return (nearestEnemyPosition - currentPosition).normalized;
        }

        return Vector3.zero;
    }


    public Vector3 GetNearestEnemyPos()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");


        if (EnemiesAlive(enemies))
        {
            GameObject nearestEnemy = null;
            float minDistance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(enemy.transform.position, currentPosition);
                if (distance < minDistance)
                {
                    nearestEnemy = enemy;
                    minDistance = distance;
                }
            }
            return nearestEnemy.transform.position;
        }
        return Vector3.zero;
    }

    private bool EnemiesAlive(GameObject[] enemies)
    {
        if (enemies.Length == 0)
        {
            return false;
        }

        return true;
    }
}
