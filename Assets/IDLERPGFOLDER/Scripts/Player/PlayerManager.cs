using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IDamageable
{
    [Header("Elemental")]
    public List<GameObject> elementalEffect;
    
    [Header("References")]
    public PlayerData playerData;
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
        
        currentMana = playerData.maxMana;
        currentHealth = playerData.currentHealth;
        
        InitializeHealthBar();
    }

    private void InitializeStats()
    {
        playerStats.OnStatsChanged += RecalculateStats;
        RecalculateStats();
        UpdateWeaponEffects(playerData.elementType);
    }

    private void InitializeHealthBar()
    {
        healthBar.maxValue = playerData.maxHealth;
        healthBar.value = currentHealth;
        UpdateHealthBar();
    }

    private void Update()
    {
        HandleElementalInputs();
    }

    private void HandleElementalInputs()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeWeaponElement(ElementType.Fire);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeWeaponElement(ElementType.Water);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeWeaponElement(ElementType.Wind);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeWeaponElement(ElementType.Earth);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeWeaponElement(ElementType.Light);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) ChangeWeaponElement(ElementType.Dark);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) RestoreMana(50f);
    }

    public void RecalculateStats()
    {
        CalculateHealthStats();
        CalculateCombatStats();
        CalculateManaStats();
        
        OnManaChanged?.Invoke(currentMana);
        UpdateHealthBar();
    }

    private void CalculateHealthStats()
    {
        float vitality = playerStats.GetStat(StatType.Vitality);
        playerData.maxHealth = formula.baseHP + (vitality * formula.hpPerVit);
        playerData.defense = formula.baseDefense + (vitality * formula.defensePerVit);
        playerData.regenRate = formula.baseRegen + (vitality * formula.regenPerVit);
    }

    private void CalculateCombatStats()
    {
        float strength = playerStats.GetStat(StatType.Strength);
        float dexterity = playerStats.GetStat(StatType.Dexterity);
        float agility = playerStats.GetStat(StatType.Agility);

        playerData.baseDamage = formula.baseDamage + (strength * formula.damagePerStr);
        playerData.criticalChance = (dexterity * formula.criticalChancePerDex) + 
                                  (agility * formula.criticalChancePerAgi);
        playerData.armorPenetration = agility * formula.armorPenatrationPerAgi;
    }

    private void CalculateManaStats()
    {
        float intelligence = playerStats.GetStat(StatType.Intelligence);
        playerData.maxMana = formula.baseMana * (1 + (intelligence * formula.manaPerInt));
        playerData.manaRegenRate = formula.baseManaRegen + (intelligence * formula.manaRegenPerInt);
    }

    private IEnumerator RegenerateHp()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenInterval);

            if (currentHealth < playerData.maxHealth && !isPlayerDie)
            {
                SpawnRegenEffect();
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
        currentHealth = Mathf.Min(currentHealth + playerData.regenRate, playerData.maxHealth);
        UpdateHealthBar();
    }

    public float CalculatePlayerAttackDamage(float skillDamageMultiplier = 1f)
    {
        float attackDamage = (playerData.baseDamage + playerData.weaponDamage) * skillDamageMultiplier;
        attackDamage *= Random.Range(1 - playerData.damageVariation, 1 + playerData.damageVariation);

        isCritical = Random.value < playerData.criticalChance;
        if (isCritical)
        {
            attackDamage *= playerData.criticalDamage;
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

        float effectiveDefense = Mathf.Max(0, playerData.defense - damageData.armorPenetration);
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
        playerData.elementType = newElement;
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
        currentHealth = Mathf.Min(currentHealth + amount, playerData.maxHealth);
        UpdateHealthBar();
    }

    public void UseMana(float amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
        OnManaChanged?.Invoke(currentMana);
    }

    public void RestoreMana(float amount)
    {
        currentMana = Mathf.Min(playerData.maxMana, currentMana + amount);
        OnManaChanged?.Invoke(currentMana);
    }

    private void Die()
    {
        if (isPlayerDie) return; // Prevent multiple death triggers
        isPlayerDie = true;
        animator.SetTrigger("Die");
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
        healthBar.maxValue = playerData.maxHealth;
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
    public float GetMaxMana() => playerData.maxMana;
    public float GetCurrentMana() => currentMana;
    public float GetMaxHealth() => playerData.maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public float GetDefense() => playerData.defense;
    public float GetDamage() => playerData.baseDamage + playerData.weaponDamage;
    public bool HasEnoughMana(float manaCost) => currentMana >= manaCost;
}