using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Skill1 : MonoBehaviour,ISkill
{
    [Header("--------------------Damage--------------------")]
    public float skillDmgFirst2Hit = 1.5f;

    public float skillDmgLastHitAttack = 0.6f;
    public Animator animator;
    public float firstDamageRadius = 2f;
    public float firstDamageDistance = 2f;
    public float cooldownTime = 5f;
    public GameObject skillEffectPrefab;
    public GameObject skillEffectPrefabSlash;
   
  
    private bool isOnCooldown = false;
    private float lastUseTime = -Mathf.Infinity;
    private PlayerController _aiController;
    public int numberOfHits = 5;
    public float timeBetweenHits = 0.2f;
    private float actualCooldownStartTime;
    private float skillDuration;
    
    
    public Vector3 hitboxSize = new Vector3(1f, 1f, 2f); // ขนาดของ hitbox
    public float hitboxDistance = 1f; // ระยะห่างจากตัวละคร
    public LayerMask hitboxLayer; // Layer ที่ต้องการตรวจสอบการชน
    
    private void Start()
    {
        _aiController = FindObjectOfType<PlayerController>();
        skillDuration = 3; 
    }

    public void UseSkill()
    {
        if (!isOnCooldown)
        {
            isOnCooldown = true;
            StartCoroutine(SkillSequence());
        }
    }
    
    IEnumerator SkillSequence()
    {
        // เล่น animation
        animator.SetTrigger("UseSkill");
        lastUseTime = Time.time;
        // รอให้ animation เล่นจบ (สมมติว่าใช้เวลา 1 วินาที)
        yield return new WaitForSeconds(2.8f);
        
        // คำนวณตำแหน่งและทิศทางสำหรับ effect
        Vector3 effectPosition = transform.position + transform.forward * 2f;
        Quaternion effectRotation = transform.rotation;
        // แสดง effect
        GameObject effect = Instantiate(skillEffectPrefab, effectPosition, effectRotation);
        Destroy(effect, 2f); // ลบ effect หลังจาก 2 วินาที
       
        // ปล่อยดาเมจหลายที
        for (int i = 0; i < numberOfHits; i++)
        {
            PerformSingleHit();
            yield return new WaitForSeconds(timeBetweenHits);
        }
        actualCooldownStartTime = Time.time;
        // เริ่มคูลดาวน์
        StartCoroutine(Cooldown());
    }
    private IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    void PerformSingleHit()
    {
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);

        // ตรวจสอบการชนกัน
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter, hitboxSize / 2f, transform.rotation, hitboxLayer);
       // Vector3 skillCenter = transform.position + transform.forward * skillRange;
     //   Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 2f, damageRadius);
        foreach (var hitCollider in hitColliders)
        {
                EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    
                    PlayerManager playerStats = GetComponent<PlayerManager>();
                    float enemyDefense = 10f;
                
                    //Skill Damage
                    float skillDamage = playerStats.CalculatePlayerAttackDamage(skillDmgLastHitAttack);
                    enemyHealth.TakeDamage(skillDamage,playerStats.playerData.armorPenetration);
                }
            
        }
    }
    public void ShowEffect1()
    {
        Vector3 effectPosition = transform.position + transform.forward * 2f; // ระยะห่างจาก GameObject ไปทางด้านหน้า 
        Quaternion effectRotation = transform.rotation;
        GameObject effect = Instantiate(skillEffectPrefabSlash, effectPosition, effectRotation);
        Destroy(effect, 0.2f);
    }

    public void DoAnimDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * firstDamageDistance, firstDamageRadius); // ระยะห่างจาก GameObject ไปทางด้านหน้า และ รัศมีของวงกลม
        foreach (var hitCollider in hitColliders)
        {
           
            EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                PlayerManager playerStats = GetComponent<PlayerManager>();
                float enemyDefense = 10f;
                
                //Skill Damage
                float skillDamage = playerStats.CalculatePlayerAttackDamage(skillDmgFirst2Hit);
                enemyHealth.TakeDamage(skillDamage,playerStats.playerData.armorPenetration);
            }
            
        }
    }

    public void EnableNav()
    {
        //_aiController.GetComponent<NavMeshAgent>().enabled = true;
        _aiController.isAIActive = true;
    }

    public void DisableNav()
    {
        //_aiController.GetComponent<NavMeshAgent>().enabled = false;
        _aiController.isAIActive = false;
    }
    public float GetCooldownPercentage()
    {
        if (!isOnCooldown)
        {
            return 0f;
        }

        float totalCooldownTime = skillDuration + cooldownTime;
        float elapsedTime = Time.time - lastUseTime;
        return 1f - Mathf.Clamp01(elapsedTime / totalCooldownTime);
    }
    public float GetRemainingCooldownTime()
    {
        if (!isOnCooldown)
        {
            return 0f;
        }

        float timeSinceUse = Time.time - lastUseTime;
        if (timeSinceUse < skillDuration)
        {
            // สกิลยังทำงานอยู่
            return cooldownTime + (skillDuration - timeSinceUse);
        }
        else
        {
            // สกิลจบแล้ว กำลังอยู่ใน cooldown จริงๆ
            return cooldownTime - (Time.time - actualCooldownStartTime);
        }
    }
   
    
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public float GetCooldownTime()
    {
        return cooldownTime;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Vector3 sphereCenter = transform.position + transform.forward * firstDamageDistance;
        Gizmos.DrawSphere(sphereCenter, firstDamageRadius);
        
        
        //Draw Hit Box Cube last hit
        Gizmos.color = Color.red;
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);
        Gizmos.matrix = Matrix4x4.TRS(hitboxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, hitboxSize);
    }
}

