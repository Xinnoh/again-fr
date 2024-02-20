using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 1f; // Distance within which the player can interact
    [SerializeField] private bool canInteractMultipleTimes = false; // Whether the object can be interacted with multiple times

    public bool hasInteracted = false; // Tracks if the object has been interacted with
    private GameObject player; // Reference to the player
    public UnityEvent onInteract; // Event triggered upon interaction

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find and assign the player object
    }

    void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= interactionDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasInteracted)
            {
                Interact();
                hasInteracted = true; 
            }
        }
    }

    private void Interact()
    {
        onInteract.Invoke(); // Invoke all methods subscribed to this event
    }
}
