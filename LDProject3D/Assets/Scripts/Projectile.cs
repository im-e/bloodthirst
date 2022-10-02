using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private static float vialDamage = 10f;
    
    private float timeSinceExist;
    private static float timeToExit = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceExist += Time.deltaTime;
        if(timeSinceExist >= timeToExit)
        {
            Destroy(gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "PlayerObject")
        {
            PlayerController pc = collision.transform.GetComponent<PlayerController>();
            if(pc != null)
            {
                pc.ReduceInjector(vialDamage);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("PC not found");
            }
        }
    }
}
