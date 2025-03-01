using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    [Header("Ground Check Settings")]
    public LayerMask groundMask;
    public Transform groundCheck;
    public float groundDistance;
    private NavMeshAgent agent;
    private bool isGrounded;
    private Animator animator;

    public float defaultSpeed = 1f;
    public float sprintSpeed = 3f;

    public float followDistance = 5f; //For Sprint

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        CheckGrounded();
        HandleJump();
        
        
        float distance = Vector3.Distance(player.position, transform.position);
        AllyRangedCombat ally = this.GetComponent<AllyRangedCombat>();
        if (ally.AllyisAttacking)
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
            
        }
        else if(ally.AllyisAttacking == false)
        {
            //agent.isStopped = false;
            animator.SetFloat("Speed",agent.velocity.magnitude);
            agent.SetDestination(player.position);
            
            if (distance >= followDistance)
            {
                //animator.SetBool("IsRunning",true);
                agent.speed = sprintSpeed;
            }
            else
            {
               // animator.SetBool("IsRunning",false);
                agent.speed = defaultSpeed;
            }

            if (agent.velocity.magnitude > 0.1f)
            {
                //animator.SetBool("isWalking", true);
            }
            else
            {
               // animator.SetBool("isWalking", false);
            }

        }
       

        


    }
    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        animator.SetBool("IsGrounded", isGrounded);

        // if (isGrounded && verticalVelocity.y < 0)
        // {
        //    verticalVelocity.y = -2f;
        // }
    }
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            animator.SetTrigger("JumpStart");
            //verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}


