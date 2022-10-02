using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    private float damage;
    private static float normalDamage = 50f;
    private static float rageDamage = 100f;
    private static float shotCost = 10f;
    private static float range = 1000f;
    private static float vialFillAmount = 25f;


    private Camera cam; 
    private PlayerController pc;
    private ParticleSystem laser;

    [Header("Audio")]
    public AudioSource gunShoot; //gun shoot sound effect
    public AudioSource gunEmpty; //gun empty sound effecrt

    public GameObject healthDisplay; //display for gun health

    private float shootCooldown = 0.5f; //time before you can shoot again
    private float lastShotTime; //timer for how long has passed from last shot

    public LayerMask enemyLayerMask;    //layer mask for enemy
    private int enemyLayer; // int of the enemy layer mask

    private void Start()
    {
        //obtain objects
        laser = GetComponentInChildren<ParticleSystem>();
        cam = GetComponentInParent<Camera>();
        pc = GameObject.Find("PlayerObject").GetComponent<PlayerController>();
        gunShoot = GetComponent<AudioSource>();
        enemyLayer = enemyLayerMask.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.isRage) damage = rageDamage; //set gun damage
        else damage = normalDamage;

        //rotate gun towards crosshair for the particle laser
        //RotateGun();
        //update gun health position
        GunHealthPositon();

        //add time since the last time you shot
        lastShotTime += Time.deltaTime;
        //if shoot has been pressed
        if (Input.GetButtonDown("Fire1") && pc.health > 0)
        {
            //if player hasnt shot before the cooldown
            if (lastShotTime >= shootCooldown) Shoot();
        }
        else if(Input.GetButtonDown("Fire1") && pc.health <= 0)
        {
            gunEmpty.Play();
        }
    }
    
    void Shoot()
    {
        //gun has been fired
        laser.Play();   //play laser particle effect
        gunShoot.Play(); //play sound effect
        
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
                    pc.fillVial(vialFillAmount); //fill vial
                }
                AI.TakeDamage(damage); //deal damage
            }
             
        }

        //take away health/ammo
        pc.health -= shotCost;

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

    void GunHealthPositon()
    {
        float zPos =  (pc.health / 100) + 0.1f - 1f;
        healthDisplay.transform.localPosition = new Vector3(healthDisplay.transform.localPosition.x, healthDisplay.transform.localPosition.y, zPos);
    }
}
