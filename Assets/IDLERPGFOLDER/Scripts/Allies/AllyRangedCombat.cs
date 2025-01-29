using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AllyRangedCombat : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform player;
    public Transform firePoint;
    private NavMeshAgent agent;
    public bool AllyisAttacking = false;
    public float fireRate = 1f;
    public float followDistance =3f; // ระยะที่ไม่อยากให้ player ห่างเกินไป
    public float rotationSpeed = 5f;
    
    private Transform target;
    public Animator animator;
    private float nextFireTime =1f;

    public void AttackEnemy(Transform enemy)
    {
        target = enemy;
        InvokeRepeating("ContinueAttacking", 0f, fireRate);
        
        if (Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + 1f/fireRate; // กำหนดเวลาการยิงครั้งถัดไป
        }
    }
    public void CallAlliesToAttack()
    {
        Transform enemy = FindEnemyInRange();  // ฟังก์ชันเพื่อค้นหาศัตรูในระยะใกล้
        if (enemy != null)
        {
            AttackEnemy(enemy);
        }
    }
    
    Transform FindEnemyInRange()
    {
        float detectionRadius = 10f; // ระยะการตรวจจับศัตรู
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Enemy")) // ตรวจสอบว่าเป็นศัตรูหรือไม่
            {
                // หาจุดกึ่งกลางของ Collider
                Vector3 enemyCenter = collider.bounds.center;
            
                float distanceToEnemy = Vector3.Distance(transform.position, enemyCenter);
                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = collider.transform;
                }
            }
        }

        return closestEnemy; // คืนค่า Transform ของศัตรูที่อยู่ใกล้ที่สุด
    }
    
    
    
    void ContinueAttacking()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (target != null && distance <= followDistance)
        {
            EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
            if (enemyHealth != null && enemyHealth.currentHealth > 0)
            {
                if (Time.time >= nextFireTime)
                {
                    FireProjectile();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
            else
            {
                AllyisAttacking = false;
                CancelInvoke("ContinueAttacking");  // หยุดโจมตีเมื่อศัตรูตาย
            }
        }
        else
        {
            AllyisAttacking = false;
            CancelInvoke("ContinueAttacking");  // หยุดโจมตีถ้าไม่มีเป้าหมาย
        }
    }


    void FireProjectile()
    {
        if (target != null)
        {
            if (animator != null)
            {
                AllyisAttacking = true;
                animator.SetTrigger("Attack");
            }
        }
    }

   public void Die()
    {
        animator.SetTrigger("Die");
    }

    void FireProjectileAnim()
    { 
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ProjectileMovement projectileMovement = projectile.GetComponent<ProjectileMovement>();
        if (projectileMovement != null)
        {
            projectileMovement.SetTarget(target);
            projectileMovement.SetExplosionRadius(3f);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        AllyisAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (AllyisAttacking)
        {
                RotateTowardsTarget();
        }
    }
    
    void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        //Debug.Log("Direction: " + direction);  // ตรวจสอบทิศทางการหมุน
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //Debug.Log("Target Rotation: " + lookRotation.eulerAngles);  // ตรวจสอบการหมุนที่ควรจะเป็น
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    
}
