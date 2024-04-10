using Edgar.Unity.Examples.Gungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitObject : MonoBehaviour
{
    private PlayerManager playerManager;
    private GameObject player;

    [SerializeField] private float interactionDistance = 1f;
    [SerializeField] private GameObject highlightField;

    private GameObject gameManager;
    private MyRoomManager myRoomManager;
    private GungeonGameManager gungeonGameManager;

    [SerializeField] private Image fadeImage;

    [SerializeField] private bool tutorialMode;
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

            if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.L))
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
        StartCoroutine(LoadAsyncScene());
    }


    IEnumerator LoadAsyncScene()
    {
        yield return StartCoroutine(FadeToBlack());

        if(tutorialMode)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Dungeon");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        else
        {
            myRoomManager.wavesCleared += 1;
            gungeonGameManager.LoadNextLevel();
        }
    }

    private IEnumerator FadeToBlack()
    {
        float duration = .7f;
        float currentTime = 0f;
        Color color = fadeImage.color;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, currentTime / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }
}
