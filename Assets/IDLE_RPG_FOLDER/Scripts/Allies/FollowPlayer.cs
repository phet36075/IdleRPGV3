using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    private NavMeshAgent agent;

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
        float distance = Vector3.Distance(player.position, transform.position);
        AllyRangedCombat ally = this.GetComponent<AllyRangedCombat>();
        if (ally.AllyisAttacking == true)
        {
            agent.isStopped = true;
            agent.SetDestination(transform.position);
        }
        else if(ally.AllyisAttacking == false)
        {
            agent.isStopped = false;
            
            agent.SetDestination(player.position);
            
            if (distance >= followDistance)
            {
                animator.SetBool("IsRunning",true);
                agent.speed = sprintSpeed;
            }
            else
            {
                animator.SetBool("IsRunning",false);
                agent.speed = defaultSpeed;
            }

            if (agent.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }

        }
       

      
        


    }
}


