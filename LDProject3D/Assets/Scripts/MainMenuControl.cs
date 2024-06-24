using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject LoreScreen;
    public GameObject ExplainScreen;
    public GameObject ControlsScreen;

    private int sceneIndex;


    // Start is called before the first frame update
    void Start()
    {
        sceneIndex = 0;
        TitleScreen.SetActive(true);
        LoreScreen.SetActive(false);
        ExplainScreen.SetActive(false);
        ControlsScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            sceneIndex += 1;
        }

        switch(sceneIndex)
        {
            case 0:
                {
                    TitleScreen.SetActive(true);
                    LoreScreen.SetActive(false);
                    ExplainScreen.SetActive(false);
                    ControlsScreen.SetActive(false);
                    break;
                }
            case 1:
                {
                    TitleScreen.SetActive(false);
                    LoreScreen.SetActive(true);
                    ExplainScreen.SetActive(false);
                    ControlsScreen.SetActive(false);
                    break;
                }
            case 2:
                {
                    TitleScreen.SetActive(false);
                    LoreScreen.SetActive(false);
                    ExplainScreen.SetActive(true);
                    ControlsScreen.SetActive(false);
                    break;
                }
            case 3:
                {
                    TitleScreen.SetActive(false);
                    LoreScreen.SetActive(false);
                    ExplainScreen.SetActive(false);
                    ControlsScreen.SetActive(true);
                    break;
                }
            case 4:
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                }

        }
    }
}
