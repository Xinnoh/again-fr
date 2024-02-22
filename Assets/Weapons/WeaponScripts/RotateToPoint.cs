using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RotateToPoint : MonoBehaviour
{

    // Script that orientates an object on the player to a target point


    [SerializeField] private Transform targetPos;
    private GameObject player;
    private Target aimScript;
    private float aimAngle;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aimScript = player.GetComponent<Target>();
    }


    void Update()
    {
        aimAngle = aimScript.GetAimAngle();
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, aimAngle));
    }
}
