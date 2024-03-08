using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UIElements;

public class Shoot : MonoBehaviour
{

    private Transform aimTransform;
    public GameObject Rocket;
    private float angle;
    private Vector3 aimDirection;
    public float rocketSpeed = 20f;

    public float spawnOffset = 1.0f; 

    public float explosionForce = 10f;
    public float explosionRadius = 5f;
    private Target aimScript;


    public void ShootBullet()
    {
        float angle = aimScript.GetAimAngle();
        if (angle == -1) return; // No enemy to aim at

        Vector3 spawnPosition = transform.position + transform.up * spawnOffset;
        GameObject bullet = Instantiate(Rocket, spawnPosition, Quaternion.Euler(new Vector3(0, 0, angle)));
        BulletBehavior bulletScript = bullet.GetComponent<BulletBehavior>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(angle, rocketSpeed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 gizmoPosition = transform.position + transform.up * spawnOffset;
        // Gizmos.DrawWireSphere(gizmoPosition, 0.2f);
    }
}
