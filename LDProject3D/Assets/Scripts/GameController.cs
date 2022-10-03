using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public HudControl hc;
    public PlayerController pc;

    public AudioSource soundDive;
   
    public bool startGame;
    public bool gameInProgress;
    private static float timeRestart = 3f;


    // Start is called before the first frame update
    void Start()
    {
        startGame = false;
        gameInProgress = false;
        pc.gameInProgress = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startGame && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(GameStart());
            startGame = true;
        }

        if (gameInProgress && pc.goalReached) GameGoalReached();
        else if (!gameInProgress && pc.goalReached)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                int totalLevels = SceneManager.sceneCount;  //get total levels
                if (SceneManager.GetActiveScene().buildIndex + 1 > totalLevels) //if next scene doesnt exist
                    SceneManager.LoadScene(0); //go to main menu
                else //go to next level
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            if (Input.GetKeyDown(KeyCode.R)) //restart
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if (gameInProgress && pc.playerDead)
        {
            gameInProgress = false;
            StartCoroutine(GameRestart());
        }
            
    }

    void GameGoalReached()
    {
        hc.GoalReached();
        gameInProgress = false;
        pc.gameInProgress = false;
    }

    IEnumerator GameStart()
    {
        soundDive.Play();
        yield return new WaitForSeconds(1);
        hc.GameStart();
        gameInProgress = true;
        pc.gameInProgress = true;
    }

    IEnumerator GameRestart()
    {
        yield return new WaitForSeconds(timeRestart);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

