using UnityEngine;

public class Target : MonoBehaviour
{
    private PlayerMovement playerMovement;

    // a bunch of public methods to call for the player

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public float GetAimAngle()
    {
        Vector3 aimDirection = AimAtNearestEnemy();
        
        if (aimDirection.Equals(Vector3.zero) && playerMovement != null)
        {
            Vector2 lastDirection = playerMovement.GetLastMovementDirection();
            if (lastDirection != Vector2.zero)
            {
                aimDirection = new Vector3(lastDirection.x, lastDirection.y, 0);
            }
        }

        if (aimDirection.Equals(Vector3.zero)) return -1;

        return Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    }

    public Vector3 GetAimVector()
    {
        Vector3 aimVector = AimAtNearestEnemy();

        if (aimVector.Equals(Vector3.zero) && playerMovement != null)
        {
            Vector2 lastDirection = playerMovement.GetLastMovementDirection();
            if (lastDirection != Vector2.zero)
            {
                aimVector = new Vector3(lastDirection.x, lastDirection.y, 0);
            }
        }

        return aimVector;
    }

    private Vector3 AimAtNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return Vector3.zero;

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

        if (nearestEnemy != null)
        {
            return (nearestEnemy.transform.position - currentPosition).normalized;
        }

        return Vector3.zero;
    }
}
