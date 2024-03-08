using Edgar.Unity.Examples.Gungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitObject : MonoBehaviour
{
    private PlayerManager playerManager;
    private GameObject player;

    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private GameObject highlightField;

    private GameObject gameManager;
    private MyRoomManager myRoomManager;
    private GungeonGameManager gungeonGameManager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();

        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManager == null)
        {
            Debug.Log("GameManager Missing");
            return;
        }

        myRoomManager = gameManager.GetComponent<MyRoomManager>();
        gungeonGameManager = gameManager.GetComponent<GungeonGameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistancePlayer();

    }

    private void CheckDistancePlayer()
    {
        if (Vector2.Distance(player.transform.position, transform.position) <= interactionDistance)
        {
            if (highlightField != null) highlightField.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitInteraction();
            }
        }
        else
        {
            if (highlightField != null)
                highlightField.SetActive(false);
        }
    }

    private void ExitInteraction()
    {
        myRoomManager.wavesCleared += 1;
        gungeonGameManager.LoadNextLevel();
    }
}
