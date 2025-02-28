using System.Collections;
using Tiny;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    private AllyManager _allyManager;
    private WeaponSystem _weaponSystem;
    
    public Animator animator;
    private PlayerMovement playerMovement;
    public AllyRangedCombat rangedAllies;
    public NavMeshAgent agent;
    
    public int _comboStep;
    private float _lastAttackTime;
    public float comboCooldown = 0.8f;
    public bool isAttacking;
    public GameObject attackVFX;
   
    public float attackRadius = 1f;
    public Transform attackPoint;
    public LayerMask enemyLayers;
    
    // เพิ่มตัวแปรควบคุมการทำงาน
    private bool _waitingForAnimationToStart = false;
    private bool _isInTransition = false;
    private float _animationDelayTimer = 0f;
    private float _animationStartDelay = 0.1f; // ระยะเวลาที่ยอมรับว่าแอนิเมชันกำลังเริ่ม
    
    // ตัวแปรเก็บสถานะแอนิมเมเตอร์
    private int _attack1Hash;
    private int _attack2Hash;
    private int _attack3Hash;
    private int _isAttackingHash;
    public bool WaitForCombo3 = false;
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
    public GameObject slashEffect;
    void Start()
    {
        _allyManager = GetComponent<AllyManager>();
        _weaponSystem = GetComponent<WeaponSystem>();
        playerMovement = GetComponent<PlayerMovement>();
        
        // แคชค่า hash ของ animation parameters
        _attack1Hash = Animator.StringToHash("Attack1");
        _attack2Hash = Animator.StringToHash("Attack2");
        _attack3Hash = Animator.StringToHash("Attack3");
        _isAttackingHash = Animator.StringToHash("IsAttacking"); // ต้องเพิ่ม IsAttacking เป็น bool parameter ใน Animator
    }

    void Update()
    {
        if (WaitForCombo3)
        {
            _comboStep = 0;
            animator.ResetTrigger(_attack1Hash);
            animator.ResetTrigger(_attack2Hash);
            animator.ResetTrigger(_attack3Hash);
        }
            
        // ตรวจสอบการเปลี่ยนแปลงแอนิเมชัน
        CheckAnimationTransition();
        
        // ตรวจสอบว่าแอนิเมชันเริ่มหรือยัง ถ้ารอนานเกินไปให้รีเซ็ต
        if (_waitingForAnimationToStart)
        {
            _animationDelayTimer += Time.deltaTime;
            if (_animationDelayTimer > _animationStartDelay)
            {
                // ถ้าแอนิเมชันยังไม่เริ่มหลังจากรอแล้ว ให้รีเซ็ตสถานะ
                _waitingForAnimationToStart = false;
                _animationDelayTimer = 0f;
            }
        }
        
        // ตรวจสอบระยะเวลาระหว่างการโจมตี
        float timeSinceLastAttack = Time.time - _lastAttackTime;
        
        // รีเซ็ตคอมโบหากเกินเวลาที่กำหนด
        if (timeSinceLastAttack > comboCooldown)
        {
            ResetCombo();
        }

        // รีเซ็ตการโจมตี
        if (timeSinceLastAttack > 0.8f && !_isInTransition && !_waitingForAnimationToStart)
        {
            playerMovement.isTakingAction = false;
            isAttacking = false;
            animator.SetBool(_isAttackingHash, false);
        }

        // ตรวจสอบการกดปุ่ม
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryAttack();
        }
       
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                TryAttack();
            }
        }
    }
    
    // ตรวจสอบการเปลี่ยนแปลงแอนิเมชัน
    private void CheckAnimationTransition()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorTransitionInfo transitionInfo = animator.GetAnimatorTransitionInfo(0);
        
        // ตรวจสอบว่ากำลังอยู่ในช่วง transition หรือไม่
        _isInTransition = animator.IsInTransition(0);
        
        // ถ้าอยู่ในสถานะรอแอนิเมชันเริ่ม และแอนิเมชันเริ่มแล้ว
        if (_waitingForAnimationToStart && stateInfo.IsName("Attack1") || stateInfo.IsName("Attack2") || stateInfo.IsName("Attack3"))
        {
            _waitingForAnimationToStart = false;
            _animationDelayTimer = 0f;
        }
    }
    
    // แยกเป็นฟังก์ชันใหม่เพื่อตรวจสอบก่อนการโจมตี
    public void TryAttack()
    {
        // ไม่อนุญาตให้โจมตีถ้ากำลังรอแอนิเมชันเริ่ม หรืออยู่ในช่วง transition
        if (_waitingForAnimationToStart || _isInTransition)
        {
            return;
        }
        
        if (!isAttacking && _weaponSystem.GetIsDrawn == true && !WaitForCombo3)
        {
            Attack();
        }
    }
    
    public void Attack()
    {
        
            _weaponSystem.ResetIdleTimer();
            playerMovement.isTakingAction = true;
            _lastAttackTime = Time.time;
            isAttacking = true;
            animator.SetBool(_isAttackingHash, true);
        
            // ตั้งค่าว่ากำลังรอแอนิเมชันเริ่ม
            _waitingForAnimationToStart = true;
            _animationDelayTimer = 0f;
        
            // เพิ่มขั้นตอนคอมโบ
            _comboStep++;
            
            if (_comboStep > 3)
            {
                _comboStep = 1; // รีเซ็ตเป็น 1 เมื่อเกิน 3
            }

            _allyManager.CallAllAllies();
            PerformAttackAnimation();
        
        
    }
    
    private void PerformAttackAnimation()
    {
        // ล้าง triggers ก่อนเพื่อป้องกันการทับซ้อน
        animator.ResetTrigger(_attack1Hash);
        animator.ResetTrigger(_attack2Hash);
        animator.ResetTrigger(_attack3Hash);
        
        // เก็บข้อมูลเพื่อ Debug
      
        
        // ตั้ง trigger ตาม combo step
        switch (_comboStep)
        {
            case 1:
                animator.SetTrigger(_attack1Hash);
                break;
            case 2:
                animator.SetTrigger(_attack2Hash);
                break;
            case 3:
                animator.SetTrigger(_attack3Hash);
                break;
        }
    }
    
    // ฟังก์ชันรีเซ็ตคอมโบ
    private void ResetCombo()
    {
        if (_comboStep != 0)
        {
           
        }
        
        _comboStep = 0;
        animator.ResetTrigger(_attack1Hash);
        animator.ResetTrigger(_attack2Hash);
        animator.ResetTrigger(_attack3Hash);
    }
    
    // ฟังก์ชันที่ถูกเรียกจาก Animation Event
    public void PerformAttack()
    {
        attackVFX.SetActive(true);
        float effectDuration = 0.2f;
        Invoke("StopEffect", effectDuration);

        if (_comboStep == 3)
        {
            Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
            GameObject spawnedEffect =  Instantiate(slashEffect, spawnPosition, transform.rotation);
            Destroy(spawnedEffect,1f);
        }
        
        
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayers);
        foreach (Collider enemy in hitEnemies)
        {
            IDamageable target = enemy.GetComponent<IDamageable>();
            if (target != null)
            {
                PlayerManager playerManager = GetComponent<PlayerManager>();
                float attackDamage = playerManager.CalculatePlayerAttackDamage();
                DamageData damageData = new DamageData(attackDamage, playerManager.playerProperty.armorPenetration, playerManager.playerProperty.elementType);
                target.TakeDamage(damageData);
            }
        }
    }
    
    private void StopEffect()
    {
        attackVFX.SetActive(false);
    }
    
    // เรียกจาก Animation Event เมื่อแอนิเมชันการโจมตีจบ
    public void EndAttack()
    {
       
        // ถ้าเป็นท่าสุดท้าย (3) ให้รีเซ็ตคอมโบ
        if (_comboStep == 0 || _comboStep == 3)
        {
            _comboStep = 0;
        }

       
        if(agent.enabled)
            agent.isStopped = false;
        
        isAttacking = false;
        _waitingForAnimationToStart = false;
        _animationDelayTimer = 0f;
        
        
    }

    public void EnableMovement()
    {
        
    }

    public void EndAttack3()
    {
            StartCoroutine(WaitCombo3());
            isAttacking = false;
            _waitingForAnimationToStart = false;
            _animationDelayTimer = 0f;
        
            if(agent.enabled)
                agent.isStopped = false;
    }
    IEnumerator WaitCombo3()
    {
        WaitForCombo3 = true;
        yield return new WaitForSeconds(1);
        WaitForCombo3 = false;
        
    }
    
    // เรียกจาก Animation Event เมื่อแอนิเมชันการโจมตีเริ่ม
    public void StartAttack()
    {
       
        
        if(agent.enabled)
            agent.isStopped = true;
        
        isAttacking = true;
        _waitingForAnimationToStart = false;
        _animationDelayTimer = 0f;
    }
    
   
}