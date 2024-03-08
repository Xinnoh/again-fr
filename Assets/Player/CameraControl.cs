using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject CameraTarget;
    private Target target;
    private Vector2 nearestEnemy;
    private Vector2 playerPos;
    public float speed = 2.0f; // Base speed at which the camera target moves
    public float maxDistanceFromPlayer = 5.0f; // Maximum distance the camera target can be from the player
    public float slowdownStartDistance = 3.0f; // Distance from player at which the camera target starts to slow down
    [SerializeField] private float cameraXoffset = -1.14f;

    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = transform.position + new Vector3(cameraXoffset, 0, 0);
        nearestEnemy = target.GetNearestEnemyPos();

        if (nearestEnemy == Vector2.zero)
        {
            // If there are no enemies, set the CameraTarget position to the player's position directly
            CameraTarget.transform.position = playerPos;
        }
        else
        {
            // Calculate target position
            Vector2 targetPosition = AverageEnemyPlayerPos(nearestEnemy, playerPos);

            // Calculate dynamic speed and adjust CameraTarget position if enemies are present
            AdjustCameraTargetPosition(targetPosition);
        }
    }

    private void AdjustCameraTargetPosition(Vector2 targetPosition)
    {
        // Adjust target position based on max distance but keep the original logic for finding the position
        Vector2 direction = (targetPosition - playerPos).normalized;
        float distanceToTarget = Vector2.Distance(playerPos, targetPosition);
        if (distanceToTarget > maxDistanceFromPlayer)
        {
            targetPosition = playerPos + direction * maxDistanceFromPlayer;
        }

        // Calculate dynamic speed based on the distance to the slowdown start
        Vector2 currentPosition = CameraTarget.transform.position;
        float currentDistance = Vector2.Distance(currentPosition, playerPos);
        float dynamicSpeed = speed;
        /*
        if (currentDistance > slowdownStartDistance)
        {
            float slowdownFactor = Mathf.Clamp01((maxDistanceFromPlayer - currentDistance) / (maxDistanceFromPlayer - slowdownStartDistance));
            dynamicSpeed *= slowdownFactor;
        }
        */

        // Smoothly move the CameraTarget position over time with dynamic speed
        CameraTarget.transform.position = Vector2.MoveTowards(currentPosition, targetPosition, dynamicSpeed * Time.deltaTime);
    }

    private Vector2 AverageEnemyPlayerPos(Vector2 enemyPos, Vector2 playerPos)
    {
        if (enemyPos == Vector2.zero)
        {
            return playerPos; // If there's no enemy, keep the target on the player.
        }

        Vector2 averagePosition = (enemyPos + playerPos) / 2;
        return averagePosition;
    }
}
