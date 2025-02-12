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
    [Header("Jump Settings")]
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

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
        if (!isTakingAction)
        {
            HandleMovementInput();
            HandleSprintInput();
            HandleMovement();
        }
        
        
        HandleJump();
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
            currentSpeed = sprintSpeed;
            isSprinting = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = walkSpeed;
            isSprinting = false;
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
            animator.SetFloat("Speed", isSprinting ? 2f : 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }
}
