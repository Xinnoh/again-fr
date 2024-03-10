using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMessage : MonoBehaviour
{
    private PlayerTrigger playerTrigger;

    [SerializeField] private QuestItem notification;
    [SerializeField] private string tutorialMessage;

    private bool playOnce;


    void Start()
    {
        playerTrigger = GetComponent<PlayerTrigger>();

    }

    void Update()
    {
        if (playerTrigger.triggered && playOnce == false)
        {
            ShowTutorialNotification(tutorialMessage);
            playOnce = true;
        }
    }


    public void ShowTutorialNotification(string message)
    {
        if (message == "")
            return;

        if (notification != null)
        {
            if (notification.isOn)
            {
                notification.MinimizeQuest();
            }

            StartCoroutine(WaitForNotificationToEnd(message));
        }
    }

    private IEnumerator WaitForNotificationToEnd(string message)
    {
        while (notification.isOn)
        {
            yield return null;
        }

        notification.questText = message;
        notification.ExpandQuest();
    }

}
