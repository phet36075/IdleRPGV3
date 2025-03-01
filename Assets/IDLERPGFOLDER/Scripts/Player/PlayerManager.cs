using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [Header("Elemental")]
    public List<GameObject> elementalEffect;
    
    [Header("References")]
    public PlayerProperty playerProperty;
    
    public Animator allyAnimator;
    public PlayerStats playerStats;
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
    
    [Header("Status")]
    public float currentHealth;
    public float currentMana;
    public float regenInterval = 5f;
    public bool isPlayerDie;
    public Slider healthBar;
    public GameObject gameOverUI;

    private PlayerController _playerController;
    private AIController _aiController;
    private AllyRangedCombat _allyRangedCombat;
    
    public event System.Action<float> OnManaChanged;

    private void Start()
    {
        
        InitializeComponents();
        InitializeStats();
        StartCoroutine(RegenerateHp());
       

    }

    private void InitializeComponents()
    {
        _allyRangedCombat = FindObjectOfType<AllyRangedCombat>();
        _playerController = FindObjectOfType<PlayerController>();
        _aiController = FindObjectOfType<AIController>();
        
        currentMana = playerProperty.maxMana;
        currentHealth = playerProperty.maxHealth;
        
        InitializeHealthBar();
    }

    private void InitializeStats()
    {
        playerStats.OnStatsChanged += RecalculateStats;
        RecalculateStats();
        UpdateWeaponEffects(playerProperty.elementType);
    }

    private void InitializeHealthBar()
    {
        healthBar.maxValue = playerProperty.maxHealth;
        healthBar.value = currentHealth;
        UpdateHealthBar();
    }

    private void Update()
    {
        HandleElementalInputs();
    }

    private void HandleElementalInputs()
    {
        // if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeaponElement(ElementType.Fire);
        // else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeaponElement(ElementType.Water);
        // else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeaponElement(ElementType.Wind);
        // else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeWeaponElement(ElementType.Earth);
        // else if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeWeaponElement(ElementType.Light);
        // else if (Input.GetKeyDown(KeyCode.Alpha6)) ChangeWeaponElement(ElementType.Dark);
        
        if (Input.GetKeyDown(KeyCode.Alpha7)) RestoreMana(50f);
        if (Input.GetKeyDown(KeyCode.Alpha8)) Heal(99999999f);
    }

    public void RecalculateStats()
    {
        CalculateHealthStats();
        CalculateCombatStats();
        CalculateManaStats();
        
        OnManaChanged?.Invoke(currentMana);
        UpdateHealthBar();
    }
    public float healthBonus = 0f;      // โบนัสเลือด
    public float defenseBonus = 0f;     // โบนัสป้องกัน
    public float regenRateBonus = 0f;   // โบนัสฟื้นฟูเลือด
    public float baseDamageBonus = 0f;  // โบนัสพลังโจมตี
    public float criticalChanceBonus = 0f; // โบนัสโอกาสคริติคอล
    public float criticalDamageBonus = 0f;
    public float armorPenetrationBonus = 0f; // โบนัสเจาะเกราะ
    public float maxManaBonus = 0f;     // โบนัสมานา
    public float manaRegenRateBonus = 0f; // โบนัสฟื้นฟูมานา
    private void CalculateHealthStats()
    {
        float vitality = playerStats.GetStat(StatType.Vitality);
        playerProperty.maxHealth = formula.baseHP + (vitality * formula.hpPerVit) + healthBonus;
        playerProperty.defense = formula.baseDefense + (vitality * formula.defensePerVit) + defenseBonus;
        playerProperty.regenRate = formula.baseRegen + (vitality * formula.regenPerVit) + regenRateBonus;
    }

    private void CalculateCombatStats()
    {
        float strength = playerStats.GetStat(StatType.Strength);
        float dexterity = playerStats.GetStat(StatType.Dexterity);
        float agility = playerStats.GetStat(StatType.Agility);

        playerProperty.baseDamage = formula.baseDamage + (strength * formula.damagePerStr) + baseDamageBonus;
        playerProperty.criticalChance = (dexterity * formula.criticalChancePerDex) + 
                                        (agility * formula.criticalChancePerAgi) + criticalChanceBonus;
        playerProperty.armorPenetration = (agility * formula.armorPenatrationPerAgi) + armorPenetrationBonus;
     //   playerProperty.criticalDamage = agility * formula.criticalMultiplier + criticalDamageBonus;
    }

    private void CalculateManaStats()
    {
        float intelligence = playerStats.GetStat(StatType.Intelligence);
        playerProperty.maxMana = (formula.baseMana * (1 + (intelligence * formula.manaPerInt))) + maxManaBonus;
        playerProperty.manaRegenRate = formula.baseManaRegen + (intelligence * formula.manaRegenPerInt) + manaRegenRateBonus;
    }
    public void ResetAllBonuses() {
        healthBonus = 0f;
        defenseBonus = 0f;
        regenRateBonus = 0f;
        baseDamageBonus = 0f;
        criticalChanceBonus = 0f;
        armorPenetrationBonus = 0f;
        maxManaBonus = 0f;
        manaRegenRateBonus = 0f;
    
        // คำนวณค่าพลังใหม่ทั้งหมด
        CalculateHealthStats();
        CalculateCombatStats();
        CalculateManaStats();
    }
    private IEnumerator RegenerateHp()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);

            if (currentHealth < playerProperty.maxHealth && !isPlayerDie)
            {
                SpawnRegenEffect();
                audioManager.PlayHealSound();
               
                RegenerateHealth();
            }
        }
    }

    private void SpawnRegenEffect()
    {
        Quaternion rotation = Quaternion.Euler(-90f, 0, 0f);
        GameObject regenEfx = Instantiate(regenEffect, spawnRegenPosition.position, rotation);
        Destroy(regenEfx, 1f);
    }

    private void RegenerateHealth()
    {
        currentHealth = Mathf.Min(currentHealth + playerProperty.regenRate, playerProperty.maxHealth);
        damageDisplay.DisplayHealing(playerProperty.regenRate);
        UpdateHealthBar();
    }

    public float CalculatePlayerAttackDamage(float skillDamageMultiplier = 1f)
    {
        float attackDamage = (playerProperty.baseDamage + playerProperty.weaponDamage) * skillDamageMultiplier;
        attackDamage *= Random.Range(1 - playerProperty.damageVariation, 1 + playerProperty.damageVariation);

        isCritical = Random.value < (playerProperty.criticalChance / 100f);
        if (isCritical)
        {
            attackDamage *= playerProperty.criticalDamage;
        }

        return attackDamage;
    }

    public void TakeDamage(float incomingDamage, float attackerArmorPenetration)
    {
        ApplyDamage(new DamageData 
        { 
            damage = incomingDamage, 
            armorPenetration = attackerArmorPenetration 
        });
    }
    public void TakeDamage(float incomingDamage, float attackerArmorPenetration, WeaponType weaponType)
    {
        ApplyDamage(new DamageData 
        { 
            damage = incomingDamage, 
            armorPenetration = attackerArmorPenetration 
        });
    }
    public void TakeDamage(DamageData damageData)
    {
        ApplyDamage(damageData);
    }

    private void ApplyDamage(DamageData damageData)
    {
        hitEffect.StartHitEffect();

        float effectiveDefense = Mathf.Max(0, playerProperty.defense - damageData.armorPenetration);
        float damageReduction = effectiveDefense / (effectiveDefense + 100f);
        float finalDamage = damageData.damage * (1f - damageReduction);

        currentHealth = Mathf.Max(currentHealth - finalDamage, 0f);

        HandleDamageEffects(finalDamage);

        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateHealthBar();
    }

    private void HandleDamageEffects(float damage)
    {
        audioManager.PlayHitSound();
        damageDisplay.DisplayDamage(damage);
        
        Quaternion rotation = Quaternion.Euler(-90f, 0, 0f);
        GameObject effect = Instantiate(hitVFX, spawnVFXPosition.position, rotation);
        Destroy(effect, 1f);
    }

    public void ChangeWeaponElement(ElementType newElement)
    {
        playerProperty.elementType = newElement;
        UpdateWeaponEffects(newElement);
    }

    private void UpdateWeaponEffects(ElementType elementType)
    {
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

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, playerProperty.maxHealth);
        audioManager.PlayHealSound();
        UpdateHealthBar();
        SpawnRegenEffect();
        damageDisplay.DisplayHealing(amount);
    }

    public void UseMana(float amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
        OnManaChanged?.Invoke(currentMana);
    }

    public void RestoreMana(float amount)
    {
        currentMana = Mathf.Min(playerProperty.maxMana, currentMana + amount);
        OnManaChanged?.Invoke(currentMana);
    }

    private void Die()
    {
        if (isPlayerDie) return; // Prevent multiple death triggers
        isPlayerDie = true;
        animator.SetTrigger("Die");
        animator.SetBool("Died", true);
        DisableComponents();
        gameOverUI.SetActive(true);
    }

    private void DisableComponents()
    {
        _allyRangedCombat.Die();
        _aiController.enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        _playerController.isAIActive = false;
        _playerController.enabled = false;
    }

    public void ResetDie()
    {
        StartCoroutine(WaitLoading());
    }

    private IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("Reset");
        animator.SetBool("Died", false);
        allyAnimator.SetTrigger("Reset");
        
        yield return new WaitForSeconds(0.5f);
        ResetPlayerState();
    }

    private void ResetPlayerState()
    {
        isPlayerDie = false;
        GetComponent<CharacterController>().enabled = false;
        _playerController.isAIActive = true;
        _playerController.isAIEnabled = true;
        _playerController.enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
    }

    public void UpdateHealthBar()
    {
        healthBar.maxValue = playerProperty.maxHealth;
        StartCoroutine(SmoothHealthBar());
    }

    private IEnumerator SmoothHealthBar()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;
        float startValue = healthBar.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, currentHealth, elapsedTime / duration);
            yield return null;
        }

        healthBar.value = currentHealth;
    }

    // Public getters
    public float GetMaxMana() => playerProperty.maxMana;
    public float GetCurrentMana() => currentMana;
    public float GetMaxHealth() => playerProperty.maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetDefense() => playerProperty.defense;
    public float GetDamage() => playerProperty.baseDamage + playerProperty.weaponDamage;
    public bool HasEnoughMana(float manaCost) => currentMana >= manaCost;
}