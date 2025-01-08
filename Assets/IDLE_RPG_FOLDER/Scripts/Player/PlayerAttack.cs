using Tiny;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    
    public AllyRangedCombat rangedAllies;  // เพิ่มอาร์เรย์ของพวกพ้องที่โจมตีระยะไกล
    
    private int _comboStep;
    private float _lastAttackTime;
    public float comboCooldown = 1f;
    public bool isAttacking;
    private Transform _vfxPos;
    public GameObject attackVFX;
    [FormerlySerializedAs("_Trail")] public Trail trail;
    
    public float detectionRadius = 5f; // รัศมีในการตรวจจับศัตรู
    public float moveSpeed = 5f; // ความเร็วในการเคลื่อนที่
    public float attackRange = 2f; // ระยะโจมตี
    public float attackRadius = 1f; // รัศมีการโจมตี
    private Transform _nearestEnemy; // เก็บ Transform ของศัตรูที่ใกล้ที่สุด
    private bool _isMovingToEnemy; // เพิ่มตัวแปรเพื่อตรวจสอบว่ากำลังเคลื่อนที่หาศัตรูหรือไม่
    
    private float _currentSpeed; // เพิ่มตัวแปรเก็บความเร็วปัจจุบัน
    
    public Transform attackPoint; // จุดศูนย์กลางของการโจมตี
    public LayerMask enemyLayers; // Layer ของศัตรู
    void Start()
    {
        
       
    }

    void Update()
    {
        if (Time.time - _lastAttackTime > comboCooldown)
        {
            _comboStep = 0;
            isAttacking = false;
            _isMovingToEnemy = false;
        }
        
        if (Input.GetKeyDown(KeyCode.F) && !isAttacking)
        {
            Attack();
        }

        if (_isMovingToEnemy && _nearestEnemy != null)
        {
            MoveTowardsEnemy();
        }
        else
        {
            StopMoving();
        }

        // อัพเดท animator ด้วยค่า Speed ปัจจุบัน
       
    }
    public void Attack()
    {
        if (!isAttacking)
        {
            _lastAttackTime = Time.time;
            isAttacking = true;
            _comboStep++;
            rangedAllies.CallAlliesToAttack();

            FindNearestEnemy();

            if (_nearestEnemy == null)
            {
                PerformAttackAnimation();
            }
            else
            {
                float distanceToEnemy = Vector3.Distance(transform.position, _nearestEnemy.position);
                if (distanceToEnemy <= attackRange)
                {
                    PerformAttackAnimation();
                }
                else
                {
                    _isMovingToEnemy = true;
                    MoveTowardsEnemy();
                }
            }
        }
    }
    private void FindNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        float closestDistance = Mathf.Infinity;
        _nearestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    _nearestEnemy = hitCollider.transform;
                }
            }
        }
    }

    private void MoveTowardsEnemy()
    {
        if (_nearestEnemy != null)
        {
            Vector3 direction = (_nearestEnemy.position - transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(transform.position, _nearestEnemy.position);
            animator.SetFloat("Speed", _currentSpeed);
            if (distanceToEnemy > attackRange)
            {
                transform.position += direction * moveSpeed * Time.deltaTime;
                transform.LookAt(_nearestEnemy);
                
                // เริ่มเล่น animation การเดิน
                _currentSpeed = moveSpeed;
            }
            else
            {
                _isMovingToEnemy = false;
                StopMoving();
                PerformAttackAnimation();
            }
        }
        else
        {
            _isMovingToEnemy = false;
            StopMoving();
        }
    }

    private void StopMoving()
    {
        // หยุดเล่น animation การเดิน
        _currentSpeed = 0f;
    }

    private void PerformAttackAnimation()
    {
        // หยุดเล่น animation การเดินก่อนเริ่ม animation การโจมตี
        StopMoving();

        if (_comboStep == 1)
        {
            animator.SetTrigger("Attack1");
        }
        else if (_comboStep == 2)
        {
            animator.SetTrigger("Attack2");
        }
        else if (_comboStep == 3)    
        {
            animator.SetTrigger("Attack3");
            _comboStep = 0;
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            
        }
    }
    
    public void PerformAttack()
    {
        attackVFX.SetActive(true);
        trail.enabled = true;
        float effectDuration = 0.2f;
        Invoke("StopEffect", effectDuration);

       
       
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius,enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                PlayerManager playerManager = GetComponent<PlayerManager>();
                float attackDamage = playerManager.CalculatePlayerAttackDamage();
                enemyHealth.TakeDamage(attackDamage, playerManager.playerData.armorPenetration);
            }
        }
    }
    
    private void StopEffect()
    {
        attackVFX.SetActive(false);
        trail.enabled = false;
    }
    
    public void EndAttack()
    {
        isAttacking = false;
        _isMovingToEnemy = false;
        _nearestEnemy = null;
        StopMoving();
    }
    
    public void StartAttack()
    {
        isAttacking = true;
    }

   
    
    void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                PlayerManager playerManager = GetComponent<PlayerManager>();
                float attackDamage = playerManager.CalculatePlayerAttackDamage();
                enemyHealth.TakeDamage(attackDamage, playerManager.playerData.armorPenetration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (_nearestEnemy != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _nearestEnemy.position);
        }
    }
    
}
