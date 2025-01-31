using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
public class PlayerManager : MonoBehaviour
{
    [Header("Elemental")]
    public List<GameObject> elementalEffect;
    
    [Header("References")]
    public PlayerData playerData;
    public Animator allyAnimator;
    public PlayerStats playerStats; // อ้างอิงไปยัง PlayerStats component
    public StatsFormula formula;
    public AudioManager audioManager;
    public DamageDisplay damageDisplay;
    public Animator animator;
    public GameObject regenEffect;
    public GameObject hitVFX;
    public Transform spawnVFXPosition;
    public Transform spawnRegenPosition;
    public bool isCritical;
    public CharacterHitEffect hitEffect;
    
    
    
    [Header("HP")]
    public float currentHealth;
    public Slider healthBar;
    private PlayerController _playerController;
    private AIController _aiController;
    private AllyRangedCombat _allyRangedCombat;
    public GameObject gameOverUI;
    //public float regenRate = 1f;
    
    public float regenInterval = 5f;
  

    public bool isPlayerDie;

   
   
    
    private float currentHP;
    private float maxHP;
    private float damage;
    private float defense;
    private float criticalChance;
    private float evasion;
    
    
    
    // ฟังก์ชันเปลี่ยนธาตุ
   
    void Start()
    {
        UpdateHealthBar();
        healthBar.maxValue = playerData.maxHealth;
        healthBar.value = currentHealth;
        StartCoroutine(RegenerateHp());
        _allyRangedCombat = FindObjectOfType<AllyRangedCombat>();
        _playerController = FindObjectOfType<PlayerController>();
       currentHealth = playerData.currentHealth;
        _aiController = FindObjectOfType<AIController>();
      
       
       // ลงทะเบียน callback เมื่อ stats มีการเปลี่ยนแปลง
       playerStats.OnStatsChanged += RecalculateStats;
       RecalculateStats();
    }
    public void RecalculateStats()
    {
        // คำนวณ Max HP
        playerData.maxHealth = formula.baseHP + 
                (playerStats.GetStat(StatType.Vitality) * formula.hpPerVit);
        // คำนวณ Defense
        playerData.defense = formula.baseDefense + 
                             (playerStats.GetStat(StatType.Vitality) * formula.defensePerVit);
        // คำนวณ Regen
        playerData.regenRate = formula.baseRegen + 
                             (playerStats.GetStat(StatType.Vitality) * formula.regenPerVit);
        

        // ถ้า currentHP ยังไม่ถูกกำหนด ให้เท่ากับ maxHP
       // if (currentHP <= 0) currentHP = maxHP;

        // คำนวณ Damage
        playerData.baseDamage = formula.baseDamage + 
                 (playerStats.GetStat(StatType.Strength) * formula.damagePerStr);

        // คำนวณ Critical Chance
        playerData.criticalChance = playerStats.GetStat(StatType.Dexterity) * 
                         formula.criticalChancePerDex;

        playerData.criticalChance = playerData.criticalChance +
                                    playerStats.GetStat(StatType.Agility) * formula.criticalChancePerAgi;
        playerData.armorPenetration = playerStats.GetStat(StatType.Agility) * formula.armorPenatrationPerAgi;

        // คำนวณ Evasion
        //  evasion = playerStats.GetStat(StatType.Agility) * formula.evasionPerAgi;
    }

   
    private void Update()
    {
        maxHP = playerData.maxHealth;
       
        // ตัวอย่างการกดปุ่มเปลี่ยนธาตุ
        if (Input.GetKeyDown(KeyCode.Alpha1)) // กด 1
        {
            ChangeWeaponElement(ElementType.Fire);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // กด 2
        {
            ChangeWeaponElement(ElementType.Water);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) // กด 3
        {
            ChangeWeaponElement(ElementType.Wind);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) // กด 4
        {
            ChangeWeaponElement(ElementType.Earth);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) // กด 5
        {
            ChangeWeaponElement(ElementType.Light);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) // กด 6
        {
            ChangeWeaponElement(ElementType.Dark);
        }
    }

    private IEnumerator RegenerateHp()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);

            if (currentHealth < playerData.maxHealth)
            {
                Quaternion rotation = Quaternion.Euler(-90f, 0, 0f);
                GameObject regenEfx = Instantiate(regenEffect, spawnRegenPosition.position,rotation );
                Destroy(regenEfx, 1f);
                
                currentHealth += playerData.regenRate;
                currentHealth = Mathf.Min(currentHealth, playerData.maxHealth);
                Debug.Log("Current HP: " + currentHealth);
                UpdateHealthBar();
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    
    public float CalculatePlayerAttackDamage(float skillDamageMultiplier = 1f)
    {
        // Start with base damage
        float attackDamage = (playerData.baseDamage + playerData.weaponDamage) * skillDamageMultiplier;
        
        // Apply damage variation
        float variationMultiplier = Random.Range(1 - playerData.damageVariation, 1 + playerData.damageVariation);
        attackDamage *= variationMultiplier;

        // Apply critical hit
        isCritical = Random.value < playerData.criticalChance;
        if (isCritical)
        {
            attackDamage *= playerData.criticalDamage; // Double damage for critical hit
            Debug.Log("Critical Hit!");
        }
        
        Debug.Log($"Attack Damage: {attackDamage} (Base: {playerData.baseDamage}, Skill Multiplier: {skillDamageMultiplier}, Critical: {isCritical})");
        return attackDamage;
    }
    public void TakeDamage(float incomingDamage , float attackerArmorPenetration )
    {
        hitEffect.StartHitEffect();
        
       // Apply armor penetration
       float effectiveDefense = Mathf.Max(0, playerData.defense - attackerArmorPenetration);

       // Calculate damage reduction
       float damageReduction = effectiveDefense / (effectiveDefense + 100f);

       // Calculate final damage
       float finalDamage = incomingDamage * (1f - damageReduction);
       
        currentHealth -= finalDamage;
        currentHealth = Mathf.Max(currentHealth, 0f); // Ensure health doesn't go below 0

        Debug.Log($"Player took {finalDamage} damage. Current health: {currentHealth}");
        
        audioManager.PlayHitSound();
        damageDisplay.DisplayDamage(finalDamage);
           Quaternion rotation = Quaternion.Euler(-90f, 0, 0f);
            GameObject effect = Instantiate(hitVFX, spawnVFXPosition.position,rotation );
            Destroy(effect, 1f);
            
        if (currentHealth <= 0)
        {
           Die();
        }
        UpdateHealthBar();

    }

    public void Heal(float amount)
    {
        currentHealth +=amount;
        if (currentHealth >= playerData.maxHealth)
        {
            currentHealth = playerData.maxHealth;
        }
    }

    public void ChangeWeapon(float weaponDmg)
    {
        playerData.weaponDamage = weaponDmg;
    }
    
    public void ChangeWeaponElement(ElementType newElement) 
    {
        // เปลี่ยนธาตุใน PlayerData
        playerData.elementType = newElement;
        
        // เปลี่ยนเอฟเฟกต์ตามธาตุ
        UpdateWeaponEffects(newElement);
    }
    
    private void UpdateWeaponEffects(ElementType elementType)
    {
        // หยุดเอฟเฟกต์เก่า
     /*   if (weaponElementalEffect != null)
        {
            weaponElementalEffect.Stop();
        }*/

     foreach (GameObject elementalEffectList in elementalEffect)
         
     {
         elementalEffectList.SetActive(false);
     }
        // ตั้งค่าเอฟเฟกต์ใหม่ตามธาตุ
        switch (elementType)
        {
            case ElementType.Fire:
                SetWeaponAppearance(elementalEffect[0]);
                break;
            case ElementType.Water:
                SetWeaponAppearance(elementalEffect[1]);
                break;
            case ElementType.Wind:
                SetWeaponAppearance(elementalEffect[2]);
                break;
            case ElementType.Earth:
                SetWeaponAppearance(elementalEffect[3]);
                break;
            case ElementType.Light:
                SetWeaponAppearance(elementalEffect[4]);
                break;
            case ElementType.Dark:
                SetWeaponAppearance(elementalEffect[5]);
                break;
        }
    }

    void SetWeaponAppearance(GameObject weaponEffect)
    {
        weaponEffect.SetActive(true);
    }
    
    
    
    void Die()
    {
        isPlayerDie = true;
       // currentHealth = PlayerData.maxHealth;
       animator.SetTrigger("Die");
       _allyRangedCombat.Die();
       _aiController.enabled = false;
       GetComponent<CapsuleCollider>().enabled = false;
       GetComponent<CharacterController>().enabled = false;
       _playerController.isAIActive = false;
       _playerController.enabled = false;
       gameOverUI.gameObject.SetActive(true);
       
    }

    public void ResetDie()
    {
        StartCoroutine(WaitLoading());
    }

    public IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("Reset");
        allyAnimator.SetTrigger("Reset");
        yield return new WaitForSeconds(0.5f);
        isPlayerDie = false;
       // GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<CharacterController>().enabled = false;
        _playerController.isAIActive = true;
        _playerController.isAIEnabled = true;
        _playerController.enabled = true;
    }
    
    public void UpdateHealthBar()
    {
        healthBar.maxValue = playerData.maxHealth;
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
    // Getters
    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => maxHP;
    public float GetDamage() => damage;
    public float GetDefense() => defense;
    public float GetCriticalChance() => criticalChance;
    public float GetEvasion() => evasion;
    
    
    
}
