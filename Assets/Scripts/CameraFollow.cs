using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform; // Reference to the player's transform
    private Vector3 offset; // The initial offset from the player

    private GameObject player;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f; // Adjust this for smoother camera movement

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }


    public void LateUpdate()
    {
        if (player == null)
        {
        }

        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        }
    }
}
