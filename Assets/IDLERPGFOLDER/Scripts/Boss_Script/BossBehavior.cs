using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class BossBehavior : MonoBehaviour
{

    public EnemyData _BossData;
    private EnemyHealth _enemyHealth;
    public Animator animator;
   
    
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float chaseRange = 5f;
    private NavMeshAgent agent;
    private Transform player;
    public float fireBallDamage = 20f;
    public float iceSpearDamage = 15f;
    public float thunderStrikeDamage = 25f;
    public float skillCooldown = 5f; // ระยะเวลาคูลดาวน์ระหว่างการใช้สกิล
    private float lastSkillTime;
    private bossSkill1 _bossSkill1;
    private string[] skills = { "FireBall", "IceSpear", "ThunderStrike" };

    public bool IsUsingSkill;
    public float rotationSpeed = 5f;
    
    public float dashDistance = 5f;
    public float dashDuration = 0.5f;

    public GameObject Skill3Prefab;
    public float distanceSkill3 = 2;
    private BossSkill2 _BossSkill2;
    
    public float attackCooldown = 1f;
    private float lastAttackTime;
    
  
    void Start()
    {
        IsUsingSkill = false;
        _bossSkill1 = FindObjectOfType<bossSkill1>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _enemyHealth = FindObjectOfType<EnemyHealth>();
        _BossSkill2 = FindObjectOfType<BossSkill2>();
    }

    void Update()
    {
       
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            if (distanceToPlayer > attackRange)
            {
                if (IsUsingSkill == false)
                {
                    agent.SetDestination(player.position);
                    animator.SetBool("IsWalking",true);
                    animator.SetBool("IsAttacking",false);
                }
                
                
            }
            else
            {
                AttackPlayer();
                animator.SetBool("IsWalking",false);
              
                if (IsUsingSkill == false)
                {
                    RotateTowardsTarget();
                }
               
                agent.SetDestination(transform.position);
            }
            
        }
        else
        {
            animator.SetBool("IsWalking",false);
            animator.SetBool("IsAttacking",false);
        }
        
        
        
        
    }

    public void animPunch()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            
            playerManager.TakeDamage(_BossData.BaseAttack,_BossData.armorPenetration);
        }

    }

    void AttackPlayer()
    {
        // ตรรกะการโจมตีผู้เล่น
        Debug.Log("Boss attacks player for " + attackDamage + " damage!");

        if (IsUsingSkill ==false)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                //Debug.Log("Attacking player at time: " + Time.tie);
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
       
        if (Random.value < 0.01f) // 1% โอกาสต่อเฟรมในการใช้สกิล
        {
            UseRandomSkill();
        }
    }

    void UseRandomSkill()
    {
        if (Time.time - lastSkillTime < skillCooldown)
        {
            return; // ยังอยู่ในช่วงคูลดาวน์
        }
        
        string randomSkill = skills[Random.Range(0, skills.Length)];
        Debug.Log("Boss uses " + randomSkill + "!");
        // เพิ่มตรรกะสำหรับแต่ละสกิลตรงนี้
        
        switch (randomSkill)
        {
            case "FireBall":
                StartCoroutine(UseFireBall1());
                break;
            case "IceSpear":
                StartCoroutine(UseIceSpear());
                break;
            case "ThunderStrike":
                StartCoroutine(UseThunderStrike());
                break;
        }

        lastSkillTime = Time.time;
    }
    
 

 IEnumerator UseFireBall1()
 {
   animator.SetBool("Skill1_Casting",true);
     RotateTowardsTarget();
     IsUsingSkill = true;
     agent.SetDestination(transform.position);
     Debug.Log("Boss casts FireBall for " + fireBallDamage + " damage!");
     _bossSkill1.SpawnObjectInFront();
     
    
    
     yield return new WaitForSeconds(2.7f);
     animator.SetBool("Skill1_Casting",false);
     animator.SetTrigger("Skill1");
     // เริ่มการพุ่ง
     _bossSkill1.AttachObject();
     Vector3 startPosition = transform.position;
     Vector3 endPosition = startPosition + transform.forward * dashDistance;
     float elapsedTime = 0f;
     while (elapsedTime < dashDuration)
     {
         transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / dashDuration);
         elapsedTime += Time.deltaTime;
         yield return null;
     }

     // ตำแหน่งสุดท้าย
     transform.position = endPosition;
     
     _bossSkill1.DestroyAuraThis();
     
     yield return new WaitForSeconds(2);
     IsUsingSkill = false;
     
 }
    IEnumerator UseIceSpear()
    {
        IsUsingSkill = true;
        animator.SetTrigger("BossSkill2");
        Debug.Log("Boss launches IceSpear for " + iceSpearDamage + " damage!");
        
        _BossSkill2.StartAttackSkill2Boss();
        yield return new WaitForSeconds(7f);
        IsUsingSkill = false;
       yield return null;
    }

    IEnumerator UseThunderStrike()
    {
        IsUsingSkill = true;
        animator.SetTrigger("BossSkill3");
        Debug.Log("Boss calls ThunderStrike for " + thunderStrikeDamage + " damage!");
        
        // สร้างวัตถุที่ตำแหน่งด้านหน้าตัวละคร
        Vector3 spawnPosition = transform.position + transform.forward * distanceSkill3;
        
        // สร้างวัตถุและกำหนดให้เป็นลูกของตัวละคร
        // คำนวณตำแหน่งและทิศทางสำหรับ effect
     
        Quaternion effectRotation = transform.rotation;
        GameObject spawnedObject = Instantiate(Skill3Prefab, spawnPosition,effectRotation);
        
        // หันหน้าวัตถุไปทางเดียวกับตัวละคร (ถ้าต้องการ)
        spawnedObject.transform.forward = transform.forward;
        
        yield return new WaitForSeconds(5f);
        IsUsingSkill = false;

    }
    

    void Die()
    {
        Debug.Log("Boss is defeated!");
        // เพิ่มตรรกะเมื่อบอสพ่ายแพ้
    }
    
    
    void RotateTowardsTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        //Debug.Log("Direction: " + direction);  // ตรวจสอบทิศทางการหมุน
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //Debug.Log("Target Rotation: " + lookRotation.eulerAngles);  // ตรวจสอบการหมุนที่ควรจะเป็น
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    
    
    
}
