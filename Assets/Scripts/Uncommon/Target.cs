using UnityEngine;
using UnityEngine.UIElements;

public class Target : MonoBehaviour
{


    public float GetAimAngle()
    {
        Vector3 aimDirection = AimAtNearestEnemy();
        if (aimDirection.Equals(Vector3.zero)) return -1; // Return an invalid angle if no enemies

        return Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
    }

    public Vector3 GetAimVector()
    {
        Vector3 aimVector = AimAtNearestEnemy();

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
