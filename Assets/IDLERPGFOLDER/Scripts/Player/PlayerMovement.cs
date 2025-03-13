using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundDistance;

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public Transform cam;
    public bool isTakingAction =false;
    public bool isUsingSkill = false;
    [Header("Jump Settings")]
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    [Header("Roll Settings")]
    public float rollSpeed = 15f;
    public float rollDuration = 0.5f;
    public float rollCooldown = 1f;

    public Collider playerCollider;
    // Private Components
    private CharacterController controller;
    private Animator animator;
    
    // Movement Variables
    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float currentSpeed;
    private Vector2 movementInput;
    private bool isSprinting;
    
    // Jump Variables
    private bool isGrounded;
    private Vector3 verticalVelocity;

    // Roll Variables
    public bool isRolling;
    private float rollTimeRemaining;
    private float rollCooldownRemaining;
    private Vector3 rollDirection;
    private void Start()
    {
        // Initialize components
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        currentSpeed = walkSpeed;
    }

    private void Update()
    {
       
        CheckGrounded();
        if (!isRolling)
        {
            if (!isTakingAction)
            {
               
                HandleSprintInput();
                if (!isUsingSkill)
                {
                    HandleMovementInput();
                    HandleMovement();
                }
               
                HandleJump();
                HandleRollInput();
            }
        }else
        {
            UpdateRoll();
        }

        UpdateRollCooldown();
        
        
        
        ApplyGravity();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        animator.SetBool("IsGrounded", isGrounded);

        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }
    }

    private void HandleMovementInput()
    {
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void HandleSprintInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // Toggle sprint state
            isSprinting = !isSprinting;
        
            // Update speed based on sprint state
            currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        }
    }

    private void HandleMovement()
    {
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate rotation
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, 
                targetAngle, 
                ref turnSmoothVelocity, 
                turnSmoothTime
            );
        
            // Apply rotation
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

            // Calculate and apply movement
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);

            // Update animation
            animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
        }
    }

    private bool isJumping = false;
    private float jumpCooldown = 2.0f; // Cooldown time in seconds
    private float jumpCooldownTimer = 0f; // Timer to track cooldown

    private void HandleJump()
    {
        // Decrease cooldown timer if it's active
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping && jumpCooldownTimer <= 0)
        {
            isJumping = true;
            animator.SetTrigger("JumpStart");
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        
            // Start the cooldown
            jumpCooldownTimer = jumpCooldown;
        }
    }
    public void OnJumpAnimationComplete()
    {
        isJumping = false;
        // Note: The cooldown will continue independently of the animation
    }

    private void ApplyGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
        if (controller.enabled)
        {
            controller.Move(verticalVelocity * Time.deltaTime);
        }
        
    }
    
    private void HandleRollInput()
    {
        // Check if can roll (grounded, not in cooldown, and moving)
        if (Input.GetKey(KeyCode.LeftControl) && 
            isGrounded && 
            rollCooldownRemaining <= 0 && 
            movementInput.magnitude > 0.1f)
        {
            StartRoll();
        }
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimeRemaining = rollDuration;
        playerCollider.enabled = false;
        // Store current movement direction for the roll
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        rollDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        // Play roll animation
        animator.SetTrigger("Roll");
    }

    private void UpdateRoll()
    {
        if (rollTimeRemaining > 0)
        {
            // Move in roll direction
            controller.Move(rollDirection * rollSpeed * Time.deltaTime);
            rollTimeRemaining -= Time.deltaTime;
        }
        else
        {
            // End roll
            isRolling = false;
            playerCollider.enabled = true;
            rollCooldownRemaining = rollCooldown;
        }
    }

    private void UpdateRollCooldown()
    {
        if (rollCooldownRemaining > 0)
        {
            rollCooldownRemaining -= Time.deltaTime;
        }
    }
}
