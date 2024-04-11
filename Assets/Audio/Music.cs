using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip[] songs; 
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (songs.Length > 0)
        {
            PlayRandomSong();
        }
    }

    void PlayRandomSong()
    {
        int randomIndex = Random.Range(0, songs.Length);

        audioSource.clip = songs[randomIndex];

        audioSource.Play();
    }
}
