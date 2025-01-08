using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;
    public float chaseRange = 5f;
    public Transform player;
    public float attackRange = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            if (distanceToPlayer > attackRange)
            {
                EnemyHealth enemyHealth = this.GetComponent<EnemyHealth>();
                if (enemyHealth.currentHealth <= 0)
                {
                    Debug.Log("Enemy IS DIE FR");
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                    animator.SetBool("IsWalking",true);
                    animator.SetBool("IsAttacking",false);
                }
                
            }
            else
            {
                agent.isStopped = true;
                animator.SetBool("IsWalking",false);
                EnemyAttack enemyAttack = this.GetComponent<EnemyAttack>();
                
                enemyAttack.AttackPlayer();
            }
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("IsWalking",false);
            animator.SetBool("IsAttacking",false);
        }

    }
}
