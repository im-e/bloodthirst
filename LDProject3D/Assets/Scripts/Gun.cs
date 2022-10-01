using UnityEngine;

public class Gun : MonoBehaviour
{
    private float damage = 25f;
    private float range = 1000f;

    private Camera cam;
    private PlayerController pc;
    private ParticleSystem laser;

    private void Start()
    {
        laser = GetComponentInChildren<ParticleSystem>();
        cam = GetComponentInParent<Camera>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1")) Shoot();
        RotateGun();
    }
    
    void Shoot()
    {

        laser.Play();
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            //if it hits an enemy
            if(hit.transform.name == "Enemy AI")
            {
                //get enemy component
                EnemyAI AI = hit.transform.GetComponent<EnemyAI>();
                if(AI != null) //check just incase
                {
                    if((AI.health - damage) <= 0f) //if damage would kill enemy
                    {
                        pc.fillVial(); //fill vial
                    }
                    AI.TakeDamage(damage); //deal damage
                }
            }    
        }

    }

    void RotateGun()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
        {
            Vector3 direction = hit.point - laser.transform.position;
            laser.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
