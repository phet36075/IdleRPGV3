using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIController : MonoBehaviour
{
    public Transform target;  // ศัตรูเป้าหมาย
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private NavMeshAgent agent;
    private Animator animator;
    private float nextAttackTime = 0f;
    public float rotationSpeed = 5f;
    public GameObject indicatorPrefab; // Prefab ของ Indicator
    private GameObject currentIndicator;
    private Transform nextLevelTarget; 
    private bool moveToNextLevel = false;
    public AllyRangedCombat _AllyRangedCombat;
    private SkillManager skillManager;

    
    
    private float lastSkillUseTime;
    public float skillUseInterval = 2f; // ระยะเวลาระหว่างการใช้สกิล
    
    
    
    
    
    private void OnDisable()
    {
        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
        }
    }
    
    void Start()
    {
        skillManager = GetComponent<SkillManager>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
   public void FindNearestEnemy()
   {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            EnemyHealth enemyScript = enemy.GetComponent<EnemyHealth>();

            if (enemyScript != null && !enemyScript.isDead)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
            
        }

        if (nearestEnemy != null)
        {
            target = nearestEnemy.transform;
           if (currentIndicator != null)
           {
               Destroy(currentIndicator);
           }
           // show indicator
           currentIndicator = Instantiate(indicatorPrefab, target.transform.position, Quaternion.identity);
        }
        else
        {
            target = null; // ไม่มีศัตรูในระยะ
            Destroy(currentIndicator);
        }
    }

   public void FindNearestBoss()
   {
       GameObject[] enemies = GameObject.FindGameObjectsWithTag("Boss");
       float shortestDistance = Mathf.Infinity;
       GameObject nearestEnemy = null;

       foreach (GameObject enemy in enemies)
       {
           EnemyHealth enemyScript = enemy.GetComponent<EnemyHealth>();

           if (enemyScript != null && !enemyScript.isDead)
           {
               float distance = Vector3.Distance(transform.position, enemy.transform.position);
               if (distance < shortestDistance)
               {
                   shortestDistance = distance;
                   nearestEnemy = enemy;
               }
           }
            
       }

       if (nearestEnemy != null)
       {
           target = nearestEnemy.transform;
           if (currentIndicator != null)
           {
               Destroy(currentIndicator);
           }
           // show indicator
           currentIndicator = Instantiate(indicatorPrefab, target.transform.position, Quaternion.identity);
       }
       else
       {
           target = null; // ไม่มีศัตรูในระยะ
           Destroy(currentIndicator);
       }
       
       
   }
    void Update()
    {
        if (currentIndicator != null)
        {
            currentIndicator.transform.position = new Vector3(target.transform.position.x,currentIndicator.transform.position.y,target.transform.position.z);
        }
        FindEnemy();
     
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    void Attack()
    {
        // เรียกใช้อนิเมชันการโจมตี
        
        animator.SetTrigger("Attack1");

        // คุณสามารถเพิ่มโค้ดสำหรับคำนวณความเสียหายและอื่น ๆ ที่นี่
    }
    
    void RotateTowardsTarget()
    {
       Vector3 direction = (target.position - transform.position).normalized;
        //Debug.Log("Direction: " + direction);  // ตรวจสอบทิศทางการหมุน
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //Debug.Log("Target Rotation: " + lookRotation.eulerAngles);  // ตรวจสอบการหมุนที่ควรจะเป็น
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    private void TryUseSkill()
    {
        if (Time.time >= lastSkillUseTime + skillUseInterval)
        {
            if (skillManager.UseNextAvailableSkill())
            {
                lastSkillUseTime = Time.time;
            }
        }
    }

    void FindEnemy()
    {
        if (target != null)
        {
            EnemyHealth targetEnemy = target.GetComponent<EnemyHealth>();
            if (targetEnemy != null && targetEnemy.isDead)
            {
                // หากศัตรูตายแล้ว หาศัตรูใหม่
                FindNearestEnemy();
                _AllyRangedCombat.CallAlliesToAttack();
            }
            else
            {
                // ทำงานตามปกติ
                if (moveToNextLevel == false)
                {
                    agent.SetDestination(target.position);
                }
               
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (distanceToTarget <= attackRange)
                {
                    //agent.isStopped = true;
                    TryUseSkill();
                    agent.SetDestination(transform.position);
                    RotateTowardsTarget();
                    if (Time.time >= nextAttackTime)
                    {
                        PlayerAttack playerAttack = this.GetComponent<PlayerAttack>();
                        playerAttack.Attack();
                        nextAttackTime = Time.time + attackCooldown;
                    }
                }
                else
                {
                    // agent.isStopped = false;
                    agent.SetDestination(target.position);
                }

                animator.SetFloat("Speed", agent.velocity.magnitude);
            }
        }
        else
        {
            // หาศัตรูใหม่เมื่อไม่มีเป้าหมาย
            if (moveToNextLevel == false)
            {
                FindNearestEnemy();
                _AllyRangedCombat.CallAlliesToAttack();
                Destroy(currentIndicator);
            }
        }
    }
}
