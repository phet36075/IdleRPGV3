using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
public class EnemyHealth : MonoBehaviour
{
    private EnemySpawner _enemySpawner;
    public bool IsthisBoss = false;
    //public GameObject CongratulationUI;
    [Header("-----------Health----------")]
    public EnemyData EnemyData;
    public Transform spawnVFXPosition;
    public float maxHealth;
    public float currentHealth;
    public float defense;
    public Slider healthBar;
    public bool isDead = false;

    public GameObject hitVFX;
    [Header("----------Animator----------")]
    public Animator animator;
   
    [Header("----------Enemy Collider----------")]
    
    public Collider enemyCollider;
    private EnemySpawner spawner;
    
    [Header("----------Stun duration----------")]
    public float staggerDuration = 0.5f;  // ระยะเวลาที่ศัตรูหยุดชะงัก
    private float lastTimeStagger = 0;
    public float cooldownStagger = 4;
   private bool isHurt;
   public CharacterHitEffect CharacterHitEffect;
   private PlayerManager _playerManager;
   public DamageDisplay _damageDisplay;
   public AudioManager _audioManager;
   void Start()
   {
      
       _enemySpawner = FindObjectOfType<EnemySpawner>();
       maxHealth = (int)Math.Round((EnemyData.maxhealth * _enemySpawner.currentStage) * 1.25f);
       currentHealth = maxHealth;
       healthBar.maxValue = maxHealth;
       healthBar.value = currentHealth;
       
       defense =(( EnemyData.defense * _enemySpawner.currentStage) * 1.1f);
       spawner = FindObjectOfType<EnemySpawner>();
       _playerManager = FindObjectOfType<PlayerManager>();
      // EnemyData.armorPenetration = (EnemyData.armorPenetration * _enemySpawner.currentStage) * 1.25f;
      // EnemyData.defense = (EnemyData.defense * _enemySpawner.currentStage) * 1.25f;
      
       //  _damageDisplay = FindObjectOfType<DamageDisplay>();
       //  _audioManager = FindObjectOfType<AudioManager>();
   }

   public void TakeDamage(float incomingDamage)
   {
       if (!isHurt)
       {
           currentHealth -= incomingDamage;
           currentHealth = Mathf.Max(currentHealth, 0f); // Ensure health doesn't go below 0
           
           CharacterHitEffect.StartHitEffect();
           animator.SetBool("isHurt", true);
           isHurt = true;
           Invoke("ResetHurt", 0.5f);
       }
           
       UpdateHealthBar();

       if (_playerManager.isCritical == true)
       {
           _damageDisplay.DisplayDamageCritical(incomingDamage);
           _playerManager.isCritical = false;
       }
       else
       {
           _damageDisplay.DisplayDamage(incomingDamage);
       }
       
       if (currentHealth > 0)
       {
           Stagger();
       }
       if (currentHealth <= 0)
       {
           Die();
       }
   }

   public void TakeDamage(float incomingDamage, float attackerArmorPenetration)
    {
        // Apply armor penetration
        float effectiveDefense = Mathf.Max(0,defense - attackerArmorPenetration);

        // Calculate damage reduction
        float damageReduction = effectiveDefense / (effectiveDefense + 100f);

        // Calculate final damage
        float finalDamage = incomingDamage * (1f - damageReduction);
        
       if (!isHurt)
       {
          
           currentHealth -= finalDamage;
           currentHealth = Mathf.Max(currentHealth, 0f); // Ensure health doesn't go below 0
           
           CharacterHitEffect.StartHitEffect();
           animator.SetBool("isHurt", true);
           isHurt = true;
           Invoke("ResetHurt", 0.5f);
       }
           
       
       
       GameObject effect = Instantiate(hitVFX, spawnVFXPosition.position, spawnVFXPosition.rotation);
       Destroy(effect, 1f);
       UpdateHealthBar();

            if (_playerManager.isCritical == true)
            {
                _damageDisplay.DisplayDamageCritical(finalDamage);
                _playerManager.isCritical = false;
                _audioManager.PlayHitCritSound();
            }
            else
            {
                _audioManager.PlayHitSound();
                _damageDisplay.DisplayDamage(finalDamage);
            }
            
            
            if (currentHealth > 0)
            {
                    Stagger();
            }
            if (currentHealth <= 0)
            {
                Die();
            }
            
            
            
    }
   
    private void Die()
    {
        if (IsthisBoss)
        {
           // CongratulationUI.gameObject.SetActive(true);
        }
        
        _audioManager.PlayDieSound();
        CurrencyManager.Instance.AddMoney( Mathf.RoundToInt((EnemyData.moneyDrop * _enemySpawner.currentStage) *1.25f));
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<NavMeshAgent>().enabled = false;
        Destroy(gameObject,3f);
        enemyCollider.enabled = false;
        animator.SetBool("IsWalking",false);
        animator.SetBool("IsAttacking",false);
        currentHealth = 0;
    }

   
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return EnemyData.maxhealth;
    }
    
    
    public void ResetHurt()
    {
        animator.SetBool("isHurt", false);
        isHurt = false;
    }
    public void OnHurtAnimationEnd()
    {
        ResetHurt();
    }
    void Stagger()
    {
       
        if (animator != null && Time.time > lastTimeStagger  )
        {
            lastTimeStagger = Time.time + cooldownStagger;
            animator.SetTrigger("Hit");
        }
        
        // หยุดการเคลื่อนไหวของศัตรูชั่วคราว
//        GetComponent<EnemyAttack>().enabled = false;  // ปิดการเคลื่อนไหว
        Invoke("RecoverFromStagger", staggerDuration);  // กำหนดเวลาหยุดชะงัก
        
    }
    void RecoverFromStagger()
    {
//        GetComponent<EnemyAttack>().enabled = true;  // เปิดการเคลื่อนไหวกลับมา
    }
    
    void UpdateHealthBar()
    {
        StartCoroutine(SmoothHealthBar());
    }

    IEnumerator SmoothHealthBar()
    {
        float elapsedTime = 0f;
        float duration = 0.2f; // ระยะเวลาที่ต้องการให้การลดลงของแถบลื่นไหล
        float startValue = healthBar.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, currentHealth, elapsedTime / duration);
            yield return null;
        }

        healthBar.value = currentHealth;
    }
    
    
    
    void Update()
    {
        
    }
}
