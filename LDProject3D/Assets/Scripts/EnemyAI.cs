using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    private GameObject player;
    private Transform playerTransform;
    private PlayerController pc;

    public LayerMask groundMask, playerMask;

    public float health;
    public GameObject projectile;

    public Material matAggro;
    private  MeshRenderer mesh;
    
    //patrol
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //attack
    public float timeBetweenAttack;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("PlayerObject");
        playerTransform = player.transform;
        pc = player.GetComponent<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        if(pc.gameInProgress && !pc.playerDead && !pc.goalReached)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundMask))
            walkPointSet = true;

    }

    private void ChasePlayer()
    {
        mesh.material = matAggro;
        agent.SetDestination(playerTransform.position);
    }

    private void AttackPlayer()
    {
        mesh.material = matAggro;
        agent.SetDestination(transform.position);

        //transform.LookAt(player);

        Vector3 relativePos = playerTransform.position - transform.position;

        transform.rotation = Quaternion.LookRotation(relativePos, new Vector3(0, 1, 0));

        if(!alreadyAttacked)
        {
            ///attack code here
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 5f, ForceMode.Impulse);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttack);
        }    
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }


    public void TakeDamage(float damage)
    {
        pc.audioSourceEnemyHit.Play();
        health -= damage;
        if (health <= 0) KillEnemy();        
    }

    public void KillEnemy()
    {
        pc.audioSourceEnemyKill.Play();
        Destroy(gameObject);
    }
}
