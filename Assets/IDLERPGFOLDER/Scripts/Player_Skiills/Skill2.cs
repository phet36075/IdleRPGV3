using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skill2 : ISkill
{
    [Header("-------------Damage-------------")]
    public float skill2DmgDashThrough = 0.6f;
    public float skill2DmgAfterDashThrough = 1.5f;
        
    public float cooldownTime = 10f;
    public float dashDistance = 5f;
    public float dashDuration = 0.5f;
    public float damageRadius = 2f;
    public float damageAmount = 20f;
    public GameObject dashEffectPrefab;
    public string dashAnimationTrigger = "DashAttack";
    public float damageVariation = 0.2f;
    private Animator animator;
    private bool isOnCooldown = false;
    private float lastUseTime = -Mathf.Infinity;
    private PlayerWeapon PlayerWeapon;
    private PlayerController _aiController;
    public int numberOfHits = 5;
    public float timeBetweenHits = 0.1f;
    private float actualCooldownStartTime;
    private float skillDuration;
    public GameObject dashTrailEffect;
    private void Start()
    {
        animator = GetComponent<Animator>();
        PlayerWeapon = FindObjectOfType<PlayerWeapon>();
        _aiController = FindObjectOfType<PlayerController>();
        skillDuration = 3; 
    }

    public override void UseSkill()
    {
        if (!isOnCooldown)
        {
            ActivateCooldown();
            lastUseTime = Time.time;

            // เล่นอนิเมชั่น
            animator.SetTrigger(dashAnimationTrigger);
          //  StartCoroutine(DashAttackCoroutine());
        }
    }

    public void PerformDashAttack()
    {
        animator.speed = 2;
        _aiController.isAIActive = false;
        GameObject dashTraileffect = Instantiate(dashTrailEffect, transform.position, transform.rotation);
        Destroy(dashTraileffect, 2f);
        
        StartCoroutine(DashAttackCoroutine());
    }
    
  
    
    private IEnumerator DashAttackCoroutine()
    {
       
        // เริ่มการพุ่ง
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
        
        //   animator.applyRootMotion = true;
        
        animator.speed = 1;
        
        GameObject effect = Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        Destroy(effect, 2f); // ลบ effect หลังจาก 2 วินาที
        for (int i = 0; i < numberOfHits; i++)
        {
            PerformSingleHit();
            yield return new WaitForSeconds(timeBetweenHits);
        }
        
        // แสดง effect
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++)
        {
            PerformSkillAfterDashhHit();
            yield return new WaitForSeconds(timeBetweenHits);
        } 
        
        // _aiController.GetComponent<NavMeshAgent>().enabled = true;
        _aiController.isAIActive = true;
        // รอให้ cooldown หมด
        actualCooldownStartTime = Time.time;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
        
       
    }

    void PerformSingleHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hitCollider in hitColliders)
        {
            EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                PlayerManager playerStats = GetComponent<PlayerManager>();
                
                float skillDamage = playerStats.CalculatePlayerAttackDamage(skill2DmgDashThrough);
                
                enemyHealth.TakeDamage(skillDamage,playerStats.playerData.armorPenetration);
               // damageable.TakeDamage(finalDamage);
            }
        }
        
    }

    void PerformSkillAfterDashhHit()
    {
        Collider[] hitColliders2 = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hitCollider in hitColliders2)
        {
            EnemyHealth enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                PlayerManager playerStats = GetComponent<PlayerManager>();
                   
                //Skill Damage
                float skillDamage = playerStats.CalculatePlayerAttackDamage(skill2DmgAfterDashThrough);
                enemyHealth.TakeDamage(skillDamage,playerStats.playerData.armorPenetration);
            } 
        }
    }

   
    
   
    
    private void OnDrawGizmos()
    {
       
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawSphere(transform.position, damageRadius);
        
    }
    
    
}



