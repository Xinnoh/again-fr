using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private GameObject player;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null) 
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        transform.position = player.transform.position;
        
    }
}
