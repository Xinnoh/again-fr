using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private GameObject player;
    private PlayerManager playerManager;
    private bool paused;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                ReturnToTitle();
            }
        }
    }




    void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.gameObject.GetComponent<PlayerManager>();
    }

    public void StopPlayer()
    {
        if(player == null)
        {
            FindPlayer();
        }
        paused = true;

        playerManager.playerActive = false;
    }

    public void ResumePlayer()
    {
        paused = false;
        playerManager.playerActive = true;
    }

    public void ReturnToTitle()
    {
        playerManager.UIRestart();

        SceneManager.LoadScene("TitleScreen");
    }

}
