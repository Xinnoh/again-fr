using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    [SerializeField] private LineRenderer beam;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private ParticleSystem muzzleParticles;
    [SerializeField] private ParticleSystem hitParticles;

    public float baseDamage = .1f;
    public float maxLength;

    private GameObject player;
    private Target aimScript;
    private bool stopAiming;
    private Vector2 aimDirection = Vector2.zero;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aimScript = player.GetComponent<Target>();
        beam.enabled = false;
        stopAiming = false;
    }

    public void EnableLaser()
    {
        stopAiming = false;
        beam.enabled = true;

        muzzleParticles.Play();
        hitParticles.Play();
    }

    public void DisableLaser()
    {
        stopAiming = false;
        beam.enabled = false;
        beam.SetPosition(0, muzzlePoint.position);
        beam.SetPosition(1, muzzlePoint.position);

        hitParticles.transform.position = muzzlePoint.position;
        muzzleParticles.Stop();
        hitParticles.Stop();
    }


    // Update is called once per frame
    void Update()
    {
        if (beam.enabled)
        {
            AimLaser();
        }
    }
    private void AimLaser()
    {
        if(!stopAiming)
        {
            aimDirection = aimScript.GetAimVector();
        }

        Vector2 startPosition = muzzlePoint.position;

        // Combine layer masks for enemies and walls
        int wallLayer = LayerMask.GetMask("Wall");
        int enemyLayer = LayerMask.GetMask("Enemy");
        int combinedLayerMask = wallLayer | enemyLayer;

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPosition, aimDirection, maxLength, combinedLayerMask);
        Vector2 furthestPoint = startPosition + (aimDirection * maxLength); // Default furthest point

        bool wallHitDetected = false;

        foreach (RaycastHit2D hit in hits)
        {
            // Check if hit is a wall and ensure we stop processing hits beyond the first wall
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                furthestPoint = hit.point;
                wallHitDetected = true;
                break; // Stop at the first wall hit
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") && !wallHitDetected)
            {
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    ApplyContinuousDamage();
                    
                }
            }
        }

        // Update the line renderer to stop at the first wall or the last enemy hit if no wall is hit
        beam.SetPosition(0, startPosition);
        beam.SetPosition(1, furthestPoint);
        hitParticles.transform.position = furthestPoint;
    }

    private void ApplyContinuousDamage()
    {
        if (beam.enabled)
        {
            Vector2 startPosition = muzzlePoint.position;
            Vector2 direction = aimScript.GetAimVector();
            int layerMask = LayerMask.GetMask("Enemy"); // Assuming you have an "Enemy" layer

            RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, maxLength, layerMask);

            if (hit.collider != null)
            {
                // Check if the collider is an enemy
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    if(enemyHealth.health <= baseDamage)
                    {
                        stopAiming = true;
                    }
                    float damageAmount = baseDamage * Time.fixedDeltaTime; // baseDamage is your desired damage per second
                    enemyHealth.TakeDamage(damageAmount);
                }
            }
        }
    }
}
