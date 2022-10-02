using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HudControl : MonoBehaviour
{
    public GameObject player;
    private PlayerController pc;

    [Header("UI Groups")]
    public GameObject uiGame;
    public GameObject uiDeath;
    public GameObject uiStart;

    [Header("Injector")]
    public Image injection;
    public Sprite injEmpty;
    public Sprite inj1;
    public Sprite inj2;
    public Sprite inj3;
    public Sprite inj4;
    public Sprite inj5;
    public Sprite inj6;
    public Sprite injFull;

    [Header("Bloodthirst")]
    public Image bloodthirstOverlay;
    public GameObject rageOverlay;
    public Sprite thirst0;
    public Sprite thirst1;
    public Sprite thirst2;
    public Sprite thirst3;
    public Sprite thirst4;
    public Sprite thirst5;

    public TMP_Text poolText;

    static private float injThreshold0 = 0f;
    static private float injThreshold1 = 14.2f;
    static private float injThreshold2 = 28.5f;
    static private float injThreshold3 = 42.75f;
    static private float injThreshold4 = 57f;
    static private float injThreshold5 = 71.25f;
    static private float injThreshold6 = 85.5f;
    static private float injThreshold7 = 100f;

    private void Start()
    {
        pc = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pc.playerDead) GameUIRefresh();
    }

    private void Update()
    {
        if (pc.playerDead) GameEnd();
    }

    public void GameStart()
    {
        uiDeath.SetActive(false);
        uiStart.SetActive(false);
        uiGame.SetActive(true);
    }

    void GameEnd()
    {
        uiStart.SetActive(false);
        uiGame.SetActive(false);
        uiDeath.SetActive(true);
    }

    void GameUIRefresh()
    {
        float vial = pc.bloodInjector;
        float pool = pc.bloodPool;

        poolText.text = pc.bloodPool.ToString("0");

        if (pc.isRage) rageOverlay.SetActive(true);
        else rageOverlay.SetActive(false);

        if (vial <= injThreshold0) injection.sprite = injEmpty;
        else if (vial >= injThreshold1 && vial <= injThreshold2) injection.sprite = inj1;
        else if (vial >= injThreshold2 && vial <= injThreshold3) injection.sprite = inj2;
        else if (vial >= injThreshold3 && vial <= injThreshold4) injection.sprite = inj3;
        else if (vial >= injThreshold4 && vial <= injThreshold5) injection.sprite = inj4;
        else if (vial >= injThreshold5 && vial <= injThreshold6) injection.sprite = inj5;
        else if (vial >= injThreshold6 && vial < injThreshold7) injection.sprite = inj6;
        else if (vial >= injThreshold7) injection.sprite = injFull;


        if (pool >= 100)
        {
            bloodthirstOverlay.sprite = thirst0;
        }
        else if (pool >= 80 && pool <= 100) bloodthirstOverlay.sprite = thirst1;
        else if (pool >= 60 && pool <= 80) bloodthirstOverlay.sprite = thirst2;
        else if (pool >= 40 && pool <= 60) bloodthirstOverlay.sprite = thirst3;
        else if (pool >= 20 && pool <= 40) bloodthirstOverlay.sprite = thirst4;
        else if (pool >= 0 && pool <= 20) bloodthirstOverlay.sprite = thirst5;
        else if (pool <= 0) bloodthirstOverlay.sprite = thirst5;
    }
}
