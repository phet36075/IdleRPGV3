using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour,IDamageable
{
    public float baseAttack= 1.0f;
    public float baseAttackMultiplier = 1.0f;
    public float baseSpeed = 3.0f;
    public float baseAttackSpeed = 1.0f;
    public float baseCriticalChance = 0.05f; // 5% Critical Chance
    public float speedMultiplier = 1.0f;
    public float attackSpeedMultiplier = 1.0f;
    public float criticalChanceMultiplier = 1.0f;
    public bool bypassArmor = false;
    
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
  
  //isDead
   // public bool isDead => currentHealth > 0;
    
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
   
   [Header("----------Elemental----------")]
   [SerializeField]
   private ElementType enemyElementType = ElementType.None;
        
   [SerializeField]
   private List<ElementalResistance> elementalResistances = new List<ElementalResistance>();
   
   void Start()
   {
       ApplyModifierEffects();
       addModifier();
           
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

/*   public void TakeDamage(float incomingDamage)
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
   }*/

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

    public void TakeDamage(DamageData damageData)
    {
        float elementalMultiplier = CalculateElementalMultiplier(damageData.elementType);
        
        // Apply armor penetration
        float effectiveDefense = Mathf.Max(0,defense - damageData.armorPenetration);

        // Calculate damage reduction
        float damageReduction = effectiveDefense / (effectiveDefense + 100f);

        // Calculate final damage
        float finalDamage = damageData.damage * elementalMultiplier * (1f - damageReduction);
        
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
   
    private float CalculateElementalMultiplier(ElementType attackElementType)
    {
        // เช็คว่าแพ้ชนะธาตุกันไหม
        float baseMultiplier = GetElementalAdvantage(attackElementType, enemyElementType);
        
        // เช็คค่าต้านทานธาตุ
        ElementalResistance resistance = elementalResistances.Find(r => r.elementType == attackElementType);
        if (resistance != null)
        {
            // ถ้ามีค่าต้านทาน +50 จะรับดาเมจแค่ 50%, -50 จะรับดาเมจ 150%
            float resistanceMultiplier = (100 - resistance.resistance) / 100f;
            return baseMultiplier * resistanceMultiplier;
        }
        
        return baseMultiplier;
    }
    
    private float GetElementalAdvantage(ElementType attackElement, ElementType defenderElement)
    {
        // ตัวอย่างความสัมพันธ์ของธาตุ
        if (attackElement == ElementType.Fire && defenderElement == ElementType.Earth) return 1.5f;
        if (attackElement == ElementType.Water && defenderElement == ElementType.Fire) return 1.5f;
        if (attackElement == ElementType.Earth && defenderElement == ElementType.Wind) return 1.5f;
        if (attackElement == ElementType.Wind && defenderElement == ElementType.Water) return 1.5f;
        
        // ถ้าเป็นธาตุที่แพ้
        if (attackElement == ElementType.Earth && defenderElement == ElementType.Fire) return 0.5f;
        if (attackElement == ElementType.Fire && defenderElement == ElementType.Water) return 0.5f;
        if (attackElement == ElementType.Wind && defenderElement == ElementType.Earth) return 0.5f;
        if (attackElement == ElementType.Water && defenderElement == ElementType.Wind) return 0.5f;
        
        return 1f; // ธาตุไม่มีผลต่อกัน
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

    private enum Modifier
    {
        AttackBoost,       // เพิ่มความแรง
        AttackSpeedBoost, // เพิ่มความเร็วการโจมตี
        CriticalHit100,   // ติดคริ 100%
        ArmorBreaker,     // ศัตรูพังเกราะ
        None              // ไม่มี Modifier
    }
    private Modifier currentModifier;
    public void addModifier()
    {
        int randomValue = Random.Range(0, 5); // สุ่มค่า 0 ถึง 4
        currentModifier = (Modifier)randomValue;
        Debug.Log($"Assigned Modifier: {currentModifier}");
    }
    
    private void ApplyModifierEffects()
    {
        switch (currentModifier)
        {
            case Modifier.AttackBoost:
                baseAttack = 1.5f;// เพิ่มความแรง 50%
                break;

            case Modifier.AttackSpeedBoost:
                attackSpeedMultiplier = 1.5f; // เพิ่มความเร็วการโจมตี 50%
                break;

            case Modifier.CriticalHit100:
                criticalChanceMultiplier = 20.0f; // ติดคริ 100%
                break;
            case Modifier.ArmorBreaker:
                bypassArmor = true; // ทะลุเกราะ
                break;
            case Modifier.None:
                // ไม่มีการเปลี่ยนแปลง
                break;
        }

        // อัปเดตค่าสถานะ
        UpdateStats();
    }
    private void UpdateStats()
    {
        float modifiedAttack = baseAttack * baseAttackMultiplier;
        float modifiedAttackSpeed = baseAttackSpeed * attackSpeedMultiplier;
        float modifiedCriticalChance = baseCriticalChance * criticalChanceMultiplier;

        Debug.Log($"Modified Stats -> Speed: {modifiedAttack}, Attack Speed: {modifiedAttackSpeed}, Critical Chance: {modifiedCriticalChance * 100}%");
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
    
    
}
