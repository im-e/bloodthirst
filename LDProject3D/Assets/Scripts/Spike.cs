using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private static float damage = 100f;
    private static float range = 4f;

    private static float vialFillAmount;
    private static float vialRageFillAmount = 15f;
    private static float vialNormalFillAmount = 15f;

    public float stabCooldown = 0.5f; //time before you can shoot again
    public float lastStabTime; //timer for how long has passed from last shot

    private Camera cam;
    private PlayerController pc;
    private Animator animator;

    public LayerMask enemyMask;

    public AudioSource spikeStab; //stab sound event


    private void Start()
    {
        //obtain objects
        pc = GameObject.Find("PlayerObject").GetComponent<PlayerController>();
        cam = GetComponentInParent<Camera>();
        spikeStab = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (pc.gameInProgress && !pc.playerDead && !pc.goalReached)
        {
            if (pc.isRage) vialFillAmount = vialRageFillAmount;
            else vialFillAmount = vialNormalFillAmount;

            //add time since the last time you shot
            lastStabTime += Time.deltaTime;
            //if shoot has been pressed
            if (Input.GetButtonDown("Fire2")) //q or left alt
            {
                //if player hasnt shot before the cooldown
                if (lastStabTime >= stabCooldown) Stab();
            }
        }
    }

    void Stab()
    {
        
        spikeStab.Play(); //play sound effect
        animator.Play("Stab");

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, enemyMask))
        {

            EnemyAI AI = hit.transform.GetComponent<EnemyAI>();
            if (AI != null) //check just incase
            {
                if ((AI.health - damage) <= 0f) //if damage would kill enemy
                {
                    pc.scoreKillCount += 1;
                    pc.scoreSpikeKills += 1;
                    pc.FillInjector(vialFillAmount); //fill vial
                }
                AI.TakeDamage(damage); //deal damage
            }
        }

        Debug.DrawRay(cam.transform.position, cam.transform.forward * range, Color.green, 2f);

        //reset shot cooldown
        lastStabTime = 0f;

    }


}
