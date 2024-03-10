using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSpawn : MonoBehaviour
{
    private PlayerTrigger playerTrigger;
    private bool spawned, roomComplete;

    [SerializeField] private GameObject enemyPrefab, treasurePrefab; 
    [SerializeField] private float spawnX, spawnY;

    [SerializeField] private float spawnX2, spawnY2;
    [SerializeField] private int spawnNumber;

    [SerializeField] private GameObject doors;
    private TutorialMessage tutorialMessage;

    private Vector2 lastEnemyPosition;


    [SerializeField] private QuestItem notification;

    void Start()
    {
        roomComplete = false;
        spawned = false;
        playerTrigger= GetComponent<PlayerTrigger>();
        tutorialMessage= GetComponent<TutorialMessage>();
        doors.gameObject.SetActive(false);
    }

    void Update()
    {
        CheckForPlayer();
        if (spawned && !roomComplete)
        {
            CheckForEnemies();
        }
    }


    private void CheckForEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            doors.gameObject.SetActive(false);
            Instantiate(treasurePrefab, lastEnemyPosition, Quaternion.identity);
            roomComplete = true;

            if(spawnNumber == 1)
            {
                tutorialMessage.ShowTutorialNotification("Defeating all enemies in a room rewards treasure.");
            }

        }
        else
        {
            lastEnemyPosition = enemies[0].transform.position;
        }
    }


    private void CheckForPlayer()
    {

        if (playerTrigger.triggered && spawned == false)
        {
            if (notification.isOn)
            {
                notification.MinimizeQuest();
            }

            doors.gameObject.SetActive(true);
            Instantiate(enemyPrefab, new Vector2(spawnX, spawnY), Quaternion.identity);


            if (spawnNumber == 2)
            {
                Instantiate(enemyPrefab, new Vector2(spawnX2, spawnY2), Quaternion.identity);
            }
            spawned = true;
        }
    }



    private void OnDrawGizmos()
    {
        if(spawnNumber == 1 || spawnNumber == 2)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector2(spawnX, spawnY), 0.5f);

            if (spawnNumber == 2)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector2(spawnX2, spawnY2), 0.5f);
            }
        }

    }
}
