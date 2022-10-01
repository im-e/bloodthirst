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
    private KeyCode meleeKey = KeyCode.Q;

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
    public float health = 100f;
    public float maxHealth = 200f;
    public float bloodPool = 100f;
    public float bloodInjector = 0f;
    public bool isRage;
    private bool rageReset;
    public bool playerDies;

    //static values
    private static float maxVial = 100f;
    private static float maxPool = 100f;
    private static float maxGunHealth = 100f;
    private static float vialOnKill = 25f;
    private static float startingHealth = 100f;

    //blood training values
    private static float drainTime = 0.1f;
    private static float drainAmount = 1f;
    private float lastDrainTime;

    //height value
    private float playerHeight;

    [Header("Audio")]
    public AudioSource audioSourceJump;
    public AudioSource audioSourceInject;
    public AudioSource audioSourceRage;

    //input variables
    private float hozInput;
    private float vertInput;



    private void Start()
    {
        //rigidbody controller setup
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Player starts with no jump cooldown
        readytoJump = true;

        //Start rage as already being reset
        rageReset = true;

        //get player height
        startYScale = transform.localScale.y;
        playerHeight = startYScale;
    }

    private void Update()
    {
        //blood pool control
        if (bloodPool <= 0) Rage();
        if (bloodPool <= -100) playerDies = true;

        //check if grounded
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        DrainPool();
        StateHandler();
        MyInput();
        SpeedControl();
        Drag();
      

        //testing
        testCurrentVel = rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void StateHandler()
    {

        //crouched
        if(Input.GetKey(crouchKey) && grounded)
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
        if(Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            playerHeight = crouchYScale;
            if(grounded)rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if(Input.GetKeyUp(crouchKey))
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
        if (Input.GetKey(injectionKey) && bloodInjector >= 100f)
        {
            InjectBlood();
        }
    }

    private void MovePlayer()
    {
        //Get new move dir
        moveDir = orientation.forward * vertInput + orientation.right * hozInput;

        //on slope
        if(OnSlope())
        {
            rb.AddForce(GetSlopeMoveDir() * moveSpeed * 10f, ForceMode.Force);
        }
        //on ground
        if(grounded) rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
        //in air
        else if(!grounded) rb.AddForce(moveDir.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Rage()
    {
        isRage = enabled;
        if(rageReset)
        {
            audioSourceRage.Play();
            rageReset = false;
        }
       
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        audioSourceJump.Play();

    }
    private void Drag()
    {
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }
    private void ResetJump()
    {
        readytoJump = true;
    }

    void InjectBlood()
    {
        bloodInjector = 0f; //empty injector
        bloodPool = maxPool; //fill pool
        gun.gunHealth = 100f; //fill gun health
        isRage = false; //disable rage
        rageReset = true; //toggle rage as reset    
        if (health < startingHealth) health = startingHealth;
        audioSourceInject.Play(); //play injection sound
    }
    void DrainPool()
    {
        lastDrainTime += Time.deltaTime;
        if (lastDrainTime >= drainTime)
        {
            bloodPool -= drainAmount;
            lastDrainTime = 0f;
        }
    }

    public void fillVial()
    {
        bloodInjector += vialOnKill;
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
}
