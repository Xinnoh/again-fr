using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    public bool triggered { get; private set; }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = true;


        }
    }
    
}
