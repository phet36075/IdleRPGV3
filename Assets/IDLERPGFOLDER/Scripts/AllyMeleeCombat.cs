using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyMeleeCombat : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    public bool AllyisAttacking = false;
    public bool isAutonomous = false;      // เพิ่มตัวแปรควบคุมโหมดอัตโนมัติ
    public float attackRate = 1f;
    public float attackRange = 2f;
    public float followDistance = 3f;
    public float rotationSpeed = 5f;
    public float attackDamage = 20f;
    public float searchRadius = 10f;        // รัศมีการค้นหาศัตรู
    public float updateTargetInterval = 0.5f; // ความถี่ในการอัพเดทเป้าหมาย
    
    private Transform target;
    public Animator animator;
    private float nextAttackTime = 1f;
    private float nextSearchTime = 0f;
    
    
    

    public float defaultSpeed = 1f;
    public float sprintSpeed = 3f;

    public LayerMask enemyLayers; // Layer ของศัตรู

    // เริ่มการทำงานแบบอัตโนมัติ
    public void StartAutonomousCombat()
    {
        isAutonomous = true;
        StartCoroutine(AutomaticBehavior());
    }

    // หยุดการทำงานแบบอัตโนมัติ
    public void StopAutonomousCombat()
    {
        isAutonomous = false;
        StopCoroutine(AutomaticBehavior());
        AllyisAttacking = false;
        agent.ResetPath();
    }

    IEnumerator AutomaticBehavior()
    {
        while (isAutonomous)
        {
            // ตรวจสอบระยะห่างจาก player
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            
            if (distanceToPlayer <= followDistance)
            {
                // ถ้าอยู่ในระยะที่กำหนด ค้นหาและโจมตีศัตรู
                Transform newTarget = FindEnemyInRange();
                if (newTarget != null)
                {
                    target = newTarget;
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);
                    
                    if (distanceToTarget <= attackRange)
                    {
                        // ถ้าอยู่ในระยะโจมตี
                        AllyisAttacking = true;
                        if (Time.time >= nextAttackTime)
                        {
                            PerformMeleeAttack();
                            nextAttackTime = Time.time + 1f/attackRate;
                        }
                    }
                    else
                    {
                        // ถ้าอยู่นอกระยะโจมตี เคลื่อนที่เข้าหาเป้าหมาย
                        MoveTowardsTarget();
                    }
                }
                else
                {
                    // ถ้าไม่พบศัตรู กลับไปหา player
                    MoveTowardsPlayer();
                }
            }
            else
            {
                // ถ้าอยู่ห่างจาก player เกินระยะ ให้กลับไปหา player
                MoveTowardsPlayer();
            }
            
            yield return new WaitForSeconds(updateTargetInterval);
        }
    }

    public void AttackEnemy(Transform enemy)
    {
        target = enemy;
        AllyisAttacking = true;
    }
    
    Transform FindEnemyInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Vector3 enemyCenter = collider.bounds.center;
                float distanceToEnemy = Vector3.Distance(transform.position, enemyCenter);
                if (distanceToEnemy < closestDistance)
                {
                    // ตรวจสอบว่าศัตรูยังมีชีวิตอยู่
                    EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                    if (enemyHealth != null && enemyHealth.currentHealth > 0)
                    {
                        closestDistance = distanceToEnemy;
                        closestEnemy = collider.transform;
                    }
                }
            }
        }

        return closestEnemy;
    }

    void PerformMeleeAttack()
    {
        if (target != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("IsRunning",false);
            animator.SetBool("isWalking",false);
        }
    }

    void AnimDoDamage()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, attackRange, transform.forward, attackRange);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage,0f,WeaponType.Mace);
                }
            }
            
        }
        
      /*  Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange,enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            IDamageable target = enemy.GetComponent<IDamageable>();
            if (target != null)
            {
                PlayerManager playerManager = GetComponent<PlayerManager>();
                float attackDamage = playerManager.CalculatePlayerAttackDamage();
                DamageData damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);
                target.TakeDamage(damageData);
                
            }
        }*/
        
        
        
    }
    public void Die()
    {
        isAutonomous = false;
        StopAllCoroutines();
        animator.SetTrigger("Die");
    }
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        AllyisAttacking = false;
    }

    void Update()
    {
        if (AllyisAttacking && target != null)
        {
            RotateTowardsTarget();
        }
    }
    
    void RotateTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void MoveTowardsTarget()
    {
        if (agent != null && target != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            
            float enemyDistance = Vector3.Distance(target.position, transform.position);
            if (enemyDistance >= followDistance)
            {
                animator.SetBool("IsRunning",true);
                agent.speed = sprintSpeed;
            }
          /*  else
            {
                animator.SetBool("IsRunning",false);
                agent.speed = defaultSpeed;
            }*/

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

    void MoveTowardsPlayer()
    {
        if (agent != null && player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            float distance = Vector3.Distance(player.position, transform.position);
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
            AllyisAttacking = false;
            target = null;
        }
    }

    // เพิ่มฟังก์ชันสำหรับแสดง Gizmos ในหน้า Editor เพื่อดูระยะต่างๆ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, followDistance);
    }
}