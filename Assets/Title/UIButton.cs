using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public AudioClip hoverSound, clickSound;
    private AudioSource audioSource; // AudioSource component

    void Start()
    {
        // Get the AudioSource component from the GameObject
        audioSource = GetComponent<AudioSource>();
        // If there's no AudioSource component attached to the GameObject, add one and set it up
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false; // Ensure it doesn't play automatically on start
        }
    }

    public void PlayHover()
    {
        // Play the hover noise
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClick()
    {
        // Play the click noise
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
