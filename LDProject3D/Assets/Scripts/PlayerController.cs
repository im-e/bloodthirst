using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Keybinds")]
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode sprintKey = KeyCode.LeftShift;
    private KeyCode crouchKey = KeyCode.LeftControl;
    private KeyCode injectionKey = KeyCode.E;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    [Header("Attached Objects")]
    public Gun gun;
    public Spike spike;
    public Transform orientation;
    private Rigidbody rb;

    [Header("Movement")]
    //moving
    private float moveSpeed; //movepseed
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;    //drag on ground
    public float testCurrentVel;
    private Vector3 moveDir;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Ground Check")]
    public bool readytoJump;
    public bool grounded;
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    [Header("Stats")]
    public float gunAmmo = 100f;
    private static float maxAmmo = 100f;

    public float bloodPool = 100f;
    private static float maxPool = 100f;

    public float bloodInjector = 0f;
    private static float maxInjector = 100f;

    

    [Header("Scoring")]
    public int scoreKillCount;
    public int scoreShotsFired;
    public int scoreShotsHit;
    public int scoreSpikeKills;
    public float scoreTimeTaken;
    public int scoreTimesInjected;

    public bool isRage;
    private bool rageReset;
    public bool playerDead;
    public bool goalReached;

    //blood draining values
    private static float drainTime = 0.1f;
    private static float drainAmount = 1f;
    private float lastDrainTime;

    //height value
    private float playerHeight;

    [Header("Audio")]
    public AudioSource audioSourceJump;
    public AudioSource audioSourceInject;
    public AudioSource audioSourceRage;
    public AudioSource audioSourceEnemyHit;
    public AudioSource audioSourceEnemyKill;
    public AudioSource audioSourcePoolTick;
    public AudioSource audioSourceInjectorReduced;
    public AudioSource audioSourceInjFilled;

    private float poolTickPitch;
    private static float poolTickTime = 1f;
    private float lastPoolTickTime;

    //input variables
    private float hozInput;
    private float vertInput;

    public bool gameInProgress;

    private void Start()
    {
        scoreTimeTaken = 0f;

        //rigidbody controller setup
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Player starts with no jump cooldown
        readytoJump = true;

        //Start rage as already being reset
        rageReset = true;

        goalReached = false;

        //get player height
        startYScale = transform.localScale.y;
        playerHeight = startYScale;
    }

    private void Update()
    {
        if (gameInProgress && !playerDead && !goalReached)
        {
            scoreTimeTaken += Time.deltaTime;

            //blood pool control
            if (bloodPool <= 0) Rage();
            if (bloodPool <= -100) PlayerDies();

            //check if grounded
            grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            DrainPool();
            StateHandler();
            MyInput();
            SpeedControl();
            Drag();
        }
    }

    private void FixedUpdate()
    {
        if (gameInProgress && !playerDead && !goalReached) MovePlayer();
    }

    private void StateHandler()
    {
        //crouched
        if (Input.GetKey(crouchKey) && grounded)
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        //sprinting
        if (Input.GetKey(sprintKey) && grounded)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        // Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // in air
        else
        {
            state = MovementState.air;
        }

    }
    private void MyInput()
    {
        //Mouse Movement
        hozInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        //crouching
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            playerHeight = crouchYScale;
            if (grounded) rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            playerHeight = startYScale;
        }

        //jumping
        if (Input.GetKey(jumpKey) && readytoJump && grounded)
        {
            readytoJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //inject blood
        if (Input.GetKey(injectionKey) && bloodInjector >= maxInjector)
        {
            InjectBlood();
        }
    }

    private void MovePlayer()
    {
        //Get new move dir
        moveDir = orientation.forward * vertInput + orientation.right * hozInput;

        //on slope
        if (OnSlope())
        {
            rb.AddForce(GetSlopeMoveDir() * moveSpeed * 10f, ForceMode.Force);
        }
        //on ground
        if (grounded) rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        //in air
        else if (!grounded) rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Rage()
    {
        isRage = enabled;
        if (rageReset)
        {
            rageReset = false;
        }

    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        audioSourceJump.Play();

    }

    void PlayerDies()
    {
        playerDead = true;
    }

    void InjectBlood()
    {
        scoreTimesInjected += 1;
        bloodInjector = 0f; //empty injector
        bloodPool = maxPool; //fill pool
        gunAmmo = maxAmmo; //fill gun health
        isRage = false; //disable rage
        rageReset = true; //toggle rage as reset    
        audioSourceInject.Play(); //play injection sound
    }
    void DrainPool()
    {
        lastDrainTime += Time.deltaTime;
        lastPoolTickTime += Time.deltaTime;

        if (lastDrainTime >= drainTime)
        {
            bloodPool -= drainAmount;
            lastDrainTime = 0f;
        }

        if (lastPoolTickTime >= poolTickTime)
        {
            poolTickPitch = (bloodPool / 100f) + 1.1f;
            audioSourcePoolTick.pitch = poolTickPitch;
            audioSourcePoolTick.Play();
            if (isRage) audioSourceRage.Play();
            lastPoolTickTime = 0f;
        }
    }

    public void FillInjector(float amount)
    {
        bloodInjector += amount;
        if (bloodInjector >= maxInjector) Invoke("InjectorFilled", .5f);
    }

    void InjectorFilled() { audioSourceInjFilled.Play(); }

    public void ReduceInjector(float amount)
    {
        audioSourceInjectorReduced.Play();
        bloodInjector -= amount;
        if (bloodInjector <= 0f) bloodInjector = 0f;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDir()
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    private void ResetJump()
    {
        readytoJump = true;
    }

    private void Drag()
    {
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if(other.name == "EndGoal")
        {
            goalReached = true;
        }
    }

}
