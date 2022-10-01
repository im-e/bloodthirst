using UnityEngine;
using UnityEngine.UI;

public class HudControl : MonoBehaviour
{
    public GameObject player;
    private PlayerController pc;
    public Image injection;
    public Sprite injEmpty;
    public Sprite inj1;
    public Sprite inj2;
    public Sprite inj3;
    public Sprite inj4;
    public Sprite inj5;
    public Sprite inj6;
    public Sprite injFull;


    private void Start()
    {
        pc = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float vial = pc.bloodVial;

        if (vial <= 0) injection.sprite = injEmpty;
        else if (vial >= 14.2 && vial <= 28.5) injection.sprite = inj1;
        else if (vial >= 28.5 && vial <= 42.75) injection.sprite = inj2;
        else if (vial >= 42.75 && vial <= 57) injection.sprite = inj3;
        else if (vial >= 57 && vial <= 71.25) injection.sprite = inj4;
        else if (vial >= 71.25 && vial <= 85.5) injection.sprite = inj5;
        else if (vial >= 85.5 && vial < 100) injection.sprite = inj6;
        else if (vial >= 100) injection.sprite = injFull;
    }
}
