using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class EnemyAttack : MonoBehaviour
{
    private EnemySpawner _enemySpawner;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    private float lastAttackTime;
   // public Collider enemySwordCollider;
    
    public Animator animator;
    private NavMeshAgent agent;
    public float chaseRange = 5f;
    private Transform player;
    private PlayerManager _playerManager;
    private bool isAttacking = false;
    public float rotationSpeed = 5f;
    public float damageVariation = 0.2f;
    private EnemyHealth _enemyData;
    // Start is called before the first frame update
    void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
      //  DisableCollision();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerManager = FindObjectOfType<PlayerManager>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _enemyData = FindObjectOfType<EnemyHealth>();
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
                if (enemyHealth.currentHealth > 0)
                {
                    GetComponent<EnemyRoaming>().enabled = false;
                    //agent.isStopped = false;
                    agent.SetDestination(player.position);
                    animator.SetBool("IsWalking",true);
                    animator.SetBool("IsAttacking",false);
                }
            }
            else
            {
                if (_playerManager.isPlayerDie != true)
                {
                    GetComponent<EnemyRoaming>().enabled = false;
                    agent.SetDestination(transform.position);
                    // agent.isStopped = true;
                    animator.SetBool("IsWalking",false);
                    RotateTowardsTarget();
                    AttackPlayer();
                }
               
            }
        }
        else
        {
            GetComponent<EnemyRoaming>().enabled = true;
            
            animator.SetBool("IsWalking",false);
            animator.SetBool("IsAttacking",false);
        }
        
        
    }

    /*public void EnableCollision()
    {
        enemySwordCollider.enabled = true;
        isAttacking = true;
    }*/
  /*  public void DisableCollision()
    {
        enemySwordCollider.enabled = false;
        isAttacking = false;
    }*/
    

    public void AttackPlayer()
    {
        animator.SetBool("IsAttacking",true);
      
        if (Time.time >= lastAttackTime + attackCooldown)
        {
           
            //Debug.Log("Attacking player at time: " + Time.time);
            animator.SetTrigger("Attack");
            animator.SetBool("IsWalking",false);    
            
            lastAttackTime = Time.time;
        }else
        {
           // Debug.Log("In cooldown, next attack at: " + (lastAttackTime + attackCooldown));
        }
       
       /* PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(attackDamage);
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }*/
    }
    
    void RotateTowardsTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        //Debug.Log("Direction: " + direction);  // ตรวจสอบทิศทางการหมุน
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //Debug.Log("Target Rotation: " + lookRotation.eulerAngles);  // ตรวจสอบการหมุนที่ควรจะเป็น
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
   

    void animDealsDamage()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            int finalDamage = CalculateDamage();
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            
            playerManager.TakeDamage(finalDamage,_enemyData.EnemyData.armorPenetration);
        }
    }
    
    int CalculateDamage()
    {
        float randomFactor = Random.Range(1f - damageVariation, 1f + damageVariation);
        int finalDamage = Mathf.RoundToInt(((_enemyData.EnemyData.BaseAttack * _enemySpawner.currentStage) * 1.25f) * randomFactor);
        return finalDamage;
        
    }
    
}
