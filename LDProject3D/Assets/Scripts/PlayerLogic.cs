using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 200f;

    public float pool = 100f;



    // Start is called before the first frame update
    void Start()
    {
        StartDraining();
    }

    void StartDraining()
    {
        InvokeRepeating("DrainPool", 0f, 1f);
    }

    // Update is called once per frame
    void Update()
    {


    }


    void DrainPool()
    {
        pool -= 10f;
    }

    void RestoreHealth()
    {
        health = 100f;
    }

}
