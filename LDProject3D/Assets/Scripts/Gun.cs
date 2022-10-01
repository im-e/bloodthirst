using UnityEngine;

public class Gun : MonoBehaviour
{
    private float damage;
    private float range = 1000f;

    private Camera cam;
    private PlayerController pc;
    private ParticleSystem laser;

    private AudioSource audioSource;

    private float shootCooldown = 0.25f;
    private float lastShotTime = 0f;

    public LayerMask enemyLayerMask;
    private int enemyLayer;

    private void Start()
    {
        laser = GetComponentInChildren<ParticleSystem>();
        cam = GetComponentInParent<Camera>();
        pc = GameObject.Find("PlayerObject").GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
        enemyLayer = enemyLayerMask.value;
    }

    // Update is called once per frame
    void Update()
    {
        lastShotTime += Time.deltaTime;
        if (Input.GetButtonDown("Fire1"))
        {
            if (lastShotTime >= shootCooldown) Shoot();
        }
        RotateGun();

        if (pc.isRage) damage = 100f;
        else damage = 50f;
    }
    
    void Shoot()
    {
        //gun has been fired
        laser.Play();   //play laser particle effect
        audioSource.Play(); //play sound effect
        
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, enemyLayer))
        {
            Debug.Log(hit.transform.name);
            //if it hits an enemy
          
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

        //reset shot cooldown
        lastShotTime = 0f;
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
