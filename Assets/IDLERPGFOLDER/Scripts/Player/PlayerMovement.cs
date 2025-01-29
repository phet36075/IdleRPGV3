using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
   public LayerMask groundmask;

   public Transform groundcheck;

   public float groundDistance;
   //Essentials
   public Transform cam;
   private CharacterController controller;
   private float turnSmoothTime = .1f;
   private float turnSmoothVelocity;
   private Animator anim;
   
   
   //Movement
   private Vector2 movement;
   public float walkSpeed;
   public float sprintSpeed;
   private bool sprinting;
   private float trueSpeed;
   
   
   //Jump
   public float jumpHeight;
   public float gravity;
   private bool isGrounded;
   private Vector3 velocity;


   private void Start()
   {
      trueSpeed = walkSpeed;
      controller = GetComponent<CharacterController>();
      anim = GetComponentInChildren<Animator>();
   }

   private void Update()
   {
      
      isGrounded = Physics.CheckSphere(groundcheck.position, groundDistance, groundmask);
      
      anim.SetBool("IsGrounded",isGrounded);
      if (isGrounded && velocity.y < 0);
      {
         velocity.y = -2;
      }
      movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
      Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;

      if (Input.GetKeyDown(KeyCode.LeftShift))
      {
         trueSpeed = sprintSpeed;
         sprinting = true;

      }

      if (Input.GetKeyUp(KeyCode.LeftShift))
      {
         trueSpeed = walkSpeed;
         sprinting = false;
      }
      
      if (direction.magnitude >= 0.1f)
      {
         float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
         float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
         transform.rotation = Quaternion.Euler(0f, angle, 0f);
         
         Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
         controller.Move(moveDirection.normalized * trueSpeed * Time.deltaTime);
        
         if (sprinting == true)
         {
            anim.SetFloat("Speed",2);
         }
         else
         {
            anim.SetFloat("Speed",1);
         }
         
         
      }
      else
      {
         anim.SetFloat("Speed",0);
      }
      
      //Jumping
      if (Input.GetButtonDown("Jump") && isGrounded)
      {
         velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
         //velocity.y = jumpHeight;
      }
      velocity.y += gravity * Time.deltaTime;
      controller.Move(velocity * Time.deltaTime);

   /*if (Input.GetButtonDown("Jump") && isGrounded)
   {
      Jump();
   }

   void Jump()
   {
      rb.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
   }*/
   }
   
   
   
   
}
