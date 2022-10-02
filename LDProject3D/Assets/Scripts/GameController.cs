using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public HudControl hc;
    public PlayerController pc;
    private bool startGame;

    private static float timeRestart = 3f;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startGame = true;
        }

        if (startGame) GameStart();

        if (pc.playerDead) Invoke("GameRestart", timeRestart);
    }

    void GameStart()
    {
        hc.GameStart();
        Time.timeScale = 1.0f;
    }

    void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

