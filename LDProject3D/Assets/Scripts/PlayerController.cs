using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Testing")]


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public bool readytoJump;
    public bool grounded;
    public LayerMask ground;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Stats")]
    public float health = 100f;
    public float maxHealth = 200f;
    public float bloodPool = 100f;
    public float bloodVial = 0f;
    public bool rage;
    public bool playerDies;


    GameObject gun;

    public Transform orientation;
    float hozInput;
    float vertInput;

    Vector3 moveDir;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readytoJump = true;
        StartDraining();
    }
    void StartDraining()
    {
        InvokeRepeating("DrainPool", 0f, 1f);
    }
    void DrainPool()
    {
        bloodPool -= 10f;
    }

    public void fillVial()
    {
        bloodVial += 25f;
    }

    private void Update()
    {
        if (bloodPool <= 0) rage = true;
        if (bloodPool <= -100) playerDies = true;


        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, ground);

        MyInput();
        SpeedControl();

        if (grounded) 
            rb.drag = groundDrag;
        else 
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        hozInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        //jumping
        if (Input.GetKey(jumpKey) && readytoJump && grounded)
        {
            readytoJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //inject blood
        if (Input.GetKey(KeyCode.Q) && bloodVial >= 100f)
        {
            bloodVial = 0f;
            bloodPool = 100f;
            if(health < 100f) health = 100f;
        }
    }

    private void MovePlayer()
    {
        moveDir = orientation.forward * vertInput + orientation.right * hozInput;

        if(grounded) rb.AddForce(moveDir.normalized * moveSpeed * 10f, ForceMode.Force);
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

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }
    private void ResetJump()
    {
        readytoJump = true;
    }
}
