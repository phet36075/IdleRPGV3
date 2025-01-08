using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skill3 : MonoBehaviour,ISkill
{
    [Header("-------------Damage-------------")]
    public float skill3DmgLongSword = 3f;
    public Animator animator;
    public float firstDamageRadius = 2f;
    public float firstDamageDistance = 2f;
    public float cooldownTime = 5f;
    public GameObject skillEffectPrefab;
    private bool isOnCooldown = false;
    
    private float lastUseTime = -Mathf.Infinity;
    private PlayerController _aiController;
    public int numberOfHits = 5;
    public float timeBetweenHits = 0.2f;
    private float actualCooldownStartTime;
    private float skillDuration;
    public float waitskiil1 = 1f;
    public float waitskill2 = 1f;
    
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
        _aiController.isAIActive = false;
        // เล่น animation
        animator.SetTrigger("LongSwordSkill");
        lastUseTime = Time.time;
        // รอให้ animation เล่นจบ (สมมติว่าใช้เวลา 1 วินาที)
        yield return new WaitForSeconds(waitskiil1);
        
        // คำนวณตำแหน่งและทิศทางสำหรับ effect
        Vector3 effectPosition = transform.position +  transform.forward * 7f;
        Quaternion effectRotation = transform.rotation;
        // แสดง effect
        
       // GameObject effect = Instantiate(skillEffectPrefab, effectPosition, Quaternion.identity, transform);
        GameObject effect = Instantiate(skillEffectPrefab, effectPosition, effectRotation);
        Destroy(effect, 3f); // ลบ effect หลังจาก 2 วินาที
       
        // ปล่อยดาเมจหลายที
        /*for (int i = 0; i < numberOfHits; i++)
        {
            PerformSingleHit();
            yield return new WaitForSeconds(timeBetweenHits);
        }*/
        yield return new WaitForSeconds(waitskill2);
        PerformSingleHit();
        actualCooldownStartTime = Time.time;
        
        _aiController.isAIActive = true;
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
        foreach (var hitCollider in hitColliders)
        {
                EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    PlayerManager playerStats = GetComponent<PlayerManager>();
                   
                
                    //Skill Damage
                    float skillDamage = playerStats.CalculatePlayerAttackDamage(skill3DmgLongSword);
                    enemyHealth.TakeDamage(skillDamage,playerStats.playerData.armorPenetration);
                }
            
        }
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
        //Draw Hit Box Cube last hit
        Gizmos.color = Color.yellow;
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);
        Gizmos.matrix = Matrix4x4.TRS(hitboxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, hitboxSize);
    }
}
