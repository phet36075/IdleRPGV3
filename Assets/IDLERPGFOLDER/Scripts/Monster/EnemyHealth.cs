using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;

public class EnemyHealth : MonoBehaviour,IDamageable
{
    #region Variables
  /*  [Header("Stats")]
    public float baseAttack = 1.0f;
    public float baseAttackMultiplier = 1.0f;
    public float baseSpeed = 3.0f;
    public float baseAttackSpeed = 1.0f;
    public float baseCriticalChance = 0.05f;
    public float speedMultiplier = 1.0f;
    public float attackSpeedMultiplier = 1.0f;
    public float criticalChanceMultiplier = 1.0f;
    public bool bypassArmor = false;*/
    
    [Header("Health")]
    public EnemyData EnemyData;
    private float maxHealth;
    private float currentHealth;
    private float defense;
    public Slider healthBar;
   
    private bool isDead = false;
    public bool IsthisBoss = false;
    
    [Header("Visual Effects")]
    public GameObject hitVFX;
    public Animator animator;
    public Collider enemyCollider;
    public GameObject slowVFX;
    public GameObject burningVFX;
    public GameObject holyVFX;
    public GameObject JudgementVFX;
    public GameObject DarkVFX;
    public GameObject darknessVFX;
    public GameObject IceEffect;
    public Transform darnessVFXPosition;
    public Transform spawnVFXPosition;
    
    [Header("Combat Settings")]
    public float staggerDuration = 0.5f;
    public float cooldownStagger = 4;
    private float lastTimeStagger = 0;
    private bool isHurt;
    public bool isMultiplyingDamageNextHit = false; // Tracks if next hit should be buffed
    public float storedMultiplier = 1.0f; // Stores the multiplier for the next hit
    [Header("Elemental")]
    [SerializeField] private ElementType enemyElementType = ElementType.None;
    [SerializeField] private List<ElementalResistance> elementalResistances = new List<ElementalResistance>();

    // References
    private PlayerStats playerStats;
    private EnemySpawner _enemySpawner;
    private EnemySpawner spawner;
    private PlayerManager _playerManager;
    public CharacterHitEffect CharacterHitEffect;
    public DamageDisplay _damageDisplay;
    public AudioManager _audioManager;
    private NavMeshAgent agent;
    [SerializeField] private StatusEffectUI statusEffectUI;
    
    [Header("Water Effect Settings")]
    [SerializeField] private float waterSlowMultiplier = 0.7f; // Slow effect strength (30% slower)
    [SerializeField] private float waterEffectDuration = 5f;
    [SerializeField] private float waterEffectChance = 0.9f; // 40% chance
    private bool isFreeze;
    // Water effect tracking
    private int waterEffectStacks = 0;
    private float originalSpeed;
    private float originalAnimSpeed;
    private bool isSlowed = false;
    private float lastTimeFreeze;

    [Header("Light Effect Setting")] 
    [SerializeField] private float holyEffectDuration = 3f;

    private float lastTimeHoly;
    private bool isHoly;
    private int HolyEffectStacks = 0;
    
    [Header("Dark Effect Setting")] 
    [SerializeField] private float darkEffectDuration = 3f;

    private float lastTimeDark;
    private bool isDark;
    private int DarkEffectStacks = 0;

    
    #endregion
   
    #region Unity Lifecycle
    void Start()
    {
        InitializeComponents();
        InitializeStats();
    }


    private void Update()
    {
        if (Time.time - lastTimeFreeze >= waterEffectDuration)
        {
            waterEffectStacks = 0;
        }
        if (Time.time - lastTimeHoly >= holyEffectDuration)
        {
            HolyEffectStacks = 0;
        }
        if (Time.time - lastTimeDark >= darkEffectDuration)
        {
            DarkEffectStacks = 0;
        }
        // Check for 4 stacks
        if (waterEffectStacks >= 4)
        {
           
            ApplyWaterBurst();
            waterEffectStacks = 0; // Reset stacks
        }
    }

    private void InitializeComponents()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        spawner = FindObjectOfType<EnemySpawner>();
        _playerManager = FindObjectOfType<PlayerManager>();
        agent = GetComponent<NavMeshAgent>();
       // statusEffectUI = GetComponent<StatusEffectUI>();
    }

    private void InitializeStats()
    {
        maxHealth = (int)Math.Round((EnemyData.maxhealth * _enemySpawner.currentStage) * 1.25f);
        defense = ((EnemyData.defense * _enemySpawner.currentStage) * 1.1f);
        
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        if (agent != null)
        {
            originalSpeed = agent.speed;
        }
        if (animator != null)
        {
            originalAnimSpeed = animator.speed;
        }
    }
    #endregion
    
    #region UI Health Bar

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

    #endregion
    
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
  public void TakeDamage(float incomingDamage, float attackerArmorPenetration, WeaponType weaponType)
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
            
            if (weaponType == WeaponType.Mace)
            {
                    _audioManager.PlayMaceHitSound();
                    _damageDisplay.DisplayDamage(finalDamage);
            }
            else
            if (weaponType == WeaponType.Spell)
            {
                    _audioManager.PlaySpellHitSound();
                    _damageDisplay.DisplayDamage(finalDamage);
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

    #region Main TakeDamage
 public void TakeDamage(DamageData damageData)
    {
        if (isDead) return;
        // Calculate final damage
        float finalDamage = CalculateFinalDamage(damageData);

        // Apply damage to health
        ApplyDamage(finalDamage);
    
        // Apply element effects
        ApplyElementalEffects(damageData, finalDamage);

        // Spawn hit VFX
        SpawnHitEffect();
    
        // Update UI and play sounds
        UpdateDamageDisplay(finalDamage, damageData.elementType);

        // Check health state
        CheckHealthState();
    }
    private float CalculateFinalDamage(DamageData damageData)
    {
        float elementalMultiplier = CalculateElementalMultiplier(damageData.elementType);
        float effectiveDefense = Mathf.Max(0, defense - damageData.armorPenetration);
        float damageReduction = effectiveDefense / (effectiveDefense + 100f);

        float finalDamage = damageData.damage * elementalMultiplier * (1f - damageReduction);
       
        // Apply multiplier from previous hit if it was set
        if (isMultiplyingDamageNextHit)
        {
            finalDamage *= storedMultiplier; // Use stored multiplier from last hit
            isMultiplyingDamageNextHit = false; // Reset to prevent further multiplication
        }

        // If this hit should buff the next hit, store the multiplier
        if (damageData.multiple == Multiple.Yes)
        {
            isMultiplyingDamageNextHit = true;
            storedMultiplier = damageData.multipleNextHit; // Store multiplier for next hit
        }

        return finalDamage;
    }

    private void ApplyDamage(float damage)
    {
        currentHealth -= damage ;
        currentHealth = Mathf.Max(currentHealth, 0f);

        CharacterHitEffect.StartHitEffect();
        animator.SetBool("isHurt", true);
        isHurt = true;
        Invoke("ResetHurt", 0.5f);
    
        UpdateHealthBar();
    }

    
    private void ApplyElementalEffects(DamageData damageData, float finalDamage)
    {
        if (damageData.elementType == ElementType.Fire)
        {
            ApplyBurningEffect();
        }
        else if (damageData.elementType == ElementType.Earth && !damageData.isEarthTremor)
        {
            StartCoroutine(ApplyEarthTremor(finalDamage));
        }
        else if (damageData.elementType == ElementType.Water)
        {
            HandleWaterEffect(damageData);
        }
        else if(damageData.elementType == ElementType.Light)
        {
           HandleLightEffect();
        }
        else if (damageData.elementType == ElementType.Dark)
        {
            HandleDarkEffect();
        }

        if (damageData.status == Status.Freezing)
        {
           
           Freeze();
            statusEffectUI.AddStatusEffect("PoisonExplode" ,null , 5);
            HandleWaterEffect(damageData);
        }

        if (damageData.status == Status.Radiant)
        {
            statusEffectUI.AddStatusEffect("Judgement" ,null , 1);
            StartCoroutine(ApplyJudgement(1.5f));
        }
    }
    
    private void SpawnHitEffect()
    {
        GameObject effect = Instantiate(hitVFX, spawnVFXPosition.position, spawnVFXPosition.rotation);
        Destroy(effect, 1f);
    }

    private void UpdateDamageDisplay(float damage, ElementType elementType)
    {
        if (_playerManager.isCritical)
        {
            _damageDisplay.DisplayDamageCritical(damage);
            _playerManager.isCritical = false;
            _audioManager.PlayHitCritSound();
        }
        else if (elementType == ElementType.None)
        {
            _damageDisplay.DisplayDamage(damage);
        }
        else
        {
            _audioManager.PlayHitSound();
            _damageDisplay.DisplayDamage(damage);
        }
    }

    private void CheckHealthState()
    {
        if (currentHealth > 0)
        {
            Stagger();
        }
        else
        {
            Die();
        }
    }

    #region Fire

    private void ApplyBurningEffect()
    {
        var burningEffect = gameObject.GetComponent<BurningEffect>();
        if (burningEffect == null)
        {
            burningEffect = gameObject.AddComponent<BurningEffect>();
        }

        if (burningEffect.IsActive != true)
        {
            burningEffect.Apply();
            statusEffectUI.AddStatusEffect("Burn", null, burningEffect.GetDuration());
            burningVFX.gameObject.SetActive(true);
            StartCoroutine(RemoveBurningEffect());
        }
    }
    private IEnumerator RemoveBurningEffect()
    {
        yield return new WaitForSeconds(5f);
        burningVFX.gameObject.SetActive(false);
    }

    #endregion
    
    #region Earth

    // เพิ่มระบบ Earth Tremor
    private IEnumerator ApplyEarthTremor(float initialDamage)
    {
        float[] tremorMultipliers = { 1.0f, 0.85f, 0.75f };
    
        for(int i = 0; i < tremorMultipliers.Length; i++)
        {
            yield return new WaitForSeconds(0.1f); // รอ 0.5 วินาทีระหว่างแต่ละการโจมตี
        
            var tremorDamage = new DamageData
            {
                damage = initialDamage * tremorMultipliers[i],
                elementType = ElementType.Earth,
                armorPenetration = 0,
                isEarthTremor = true
            };
        
            TakeDamage(tremorDamage);
        }
    }

    #endregion
    
    #region Water
    
    private GameObject currentSlowEffect; // เพิ่มตัวแปรเก็บ reference ของ effect
    private Coroutine slowEffectCoroutine; // เพิ่มตัวแปรเพื่อเก็บ Coroutine
    private void HandleWaterEffect(DamageData damageData)
    {
        // Check for water effect chance (40%)
        //if (Random.value <= waterEffectChance)
      //  {

            if (damageData.status != Status.Freezing)
            {
                waterEffectStacks += 1;
                statusEffectUI.AddStatusEffect("Poison" ,null , waterEffectDuration,1);
                ApplySlowEffect();
                lastTimeFreeze = Time.time;
                slowVFX.gameObject.SetActive(true);
                // Reset slow effect duration
                if (slowEffectCoroutine != null)
                {
                    StopCoroutine(RemoveSlowEffect());
                }

                // เริ่ม Coroutine ใหม่และเก็บไว้ในตัวแปร
                slowEffectCoroutine = StartCoroutine(RemoveSlowEffect());
                StartCoroutine(RemoveSlowEffect());
            }
           
       // }
        
        
        
    }
    
    private void ApplySlowEffect()
    {
        isSlowed = true;
        if (!isFreeze)
        {
            // Slow movement speed
            if (agent != null)
            {
                agent.speed = originalSpeed * waterSlowMultiplier;
            }
        
            // Slow animation speed
            if (animator != null)
            {
                animator.speed = originalAnimSpeed * waterSlowMultiplier;
            }
        }
       
    }
    
    private IEnumerator RemoveSlowEffect()
    {
        yield return new WaitForSeconds(waterEffectDuration);
       
       
        slowVFX.gameObject.SetActive(false);
        // Reset speeds if no stacks remain
        if (waterEffectStacks == 0)
        {
            isSlowed = false;
            if (!isFreeze)
            {
                if (agent != null)
                {
                    agent.speed = originalSpeed;
                }
            
                if (animator != null)
                {
                    animator.speed = originalAnimSpeed;
                }
            }
            
        }
    }
    
    private void ApplyWaterBurst()
    {
        if (_playerManager != null)
        {
            //statusEffectUI.AddStatusEffect("PoisonExplode" ,null , 5);
            float burstDamage = _playerManager.GetDamage() * 1.5f; // 150% of player's attack power
            TakeDamage(new DamageData(burstDamage, 0, ElementType.Water));
            Freeze();
        }
    }

    private void Freeze()
    {
        IceEffect.SetActive(true);
        // Freeze
        if (agent != null)
        {
            agent.speed = 0;
        }
            
        if (animator != null)
        {
            animator.speed = 0;
        }
        isFreeze = true;
           
        StartCoroutine(UnFreeze());
    }

    private IEnumerator UnFreeze()
    {
        if (isFreeze)
        {
            yield return new WaitForSeconds(5f);
            if (agent != null)
            {
                agent.speed = originalSpeed;
            }
            
            if (animator != null)
            {
                animator.speed = originalAnimSpeed;
            }
            IceEffect.SetActive(false);
            isFreeze = false;
        }
       
    }

    #endregion

    #region Light

    private void HandleLightEffect()
    {
        lastTimeHoly = Time.time;
        HolyEffectStacks++;
        holyVFX.gameObject.SetActive(true);
        isHoly = true;
        statusEffectUI.AddStatusEffect("Holy",null,holyEffectDuration);
        StopCoroutine(RemoveHolyEffect());
        StartCoroutine(RemoveHolyEffect());
        
        // Check for 3 stacks
        if (HolyEffectStacks >= 2)
        {
            isHoly = false;
            StartCoroutine(ApplyJudgement());
            HolyEffectStacks = 0; // Reset stacks
            holyVFX.gameObject.SetActive(false);
        }
    }

    private IEnumerator RemoveHolyEffect()
    {
        yield return new WaitForSeconds(holyEffectDuration);

        if (HolyEffectStacks == 0)
        {
            isHoly = false;
        }
        
        holyVFX.gameObject.SetActive(false);
        
    }

    private IEnumerator ApplyJudgement(float multiplier = 1f)
    {
        GameObject effect = Instantiate(JudgementVFX, darnessVFXPosition.position, darnessVFXPosition.rotation);
        Destroy(effect, 3f);
        float[] holyMultipliers = { 2.25f * multiplier, 1.75f * multiplier};
    
        for(int i = 0; i < holyMultipliers.Length; i++)
        {
            yield return new WaitForSeconds(0.3f); // รอ 0.5 วินาทีระหว่างแต่ละการโจมตี
        
            var holyDamage = new DamageData
            {
                damage = _playerManager.GetDamage() * holyMultipliers[i],
                elementType = ElementType.None,
                armorPenetration = 0,
               
            };
        
            TakeDamage(holyDamage);
        }

       
    }
    #endregion

    #region Dark

    private void HandleDarkEffect()
    {
        lastTimeDark = Time.time;
        DarkEffectStacks++;
        DarkVFX.gameObject.SetActive(true);
        isDark = true;
        statusEffectUI.AddStatusEffect("Dark",null,darkEffectDuration);
        StopCoroutine(RemoveDarkEffect());
        StartCoroutine(RemoveDarkEffect());
        
        // Check for 3 stacks
        if (DarkEffectStacks >= 4)
        {
            isDark = false;
           ApplyDarkness();
            DarkEffectStacks = 0; // Reset stacks
            DarkVFX.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator RemoveDarkEffect()
    {
        yield return new WaitForSeconds(darkEffectDuration);

        if (darkEffectDuration == 0)
        {
            isDark = false;
        }
        
        DarkVFX.gameObject.SetActive(false);
        
    }

    private void ApplyDarkness()
    {
        GameObject effect = Instantiate(darknessVFX, darnessVFXPosition.position, darnessVFXPosition.rotation);
        Destroy(effect, 1f);
        float damageAmount = maxHealth * 0.2f;
            var darknessDamage = new DamageData
            {
                damage = damageAmount,
                elementType = ElementType.None,
                armorPenetration = 0,
               
            };
            TakeDamage(darknessDamage);
            
    }
    
    
    #endregion

    #endregion

    #region Elemental System

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
        // ธาตุแสง vs มืด
        if (attackElement == ElementType.Light && defenderElement == ElementType.Dark) return 2.0f;    // แสงแรงกว่ามืด
        if (attackElement == ElementType.Dark && defenderElement == ElementType.Light) return 0.5f;    // มืดเสียเปรียบแสง
    
        // ธาตุแสงได้เปรียบธาตุทั่วไป (แต่ไม่มากเท่าธาตุหลัก)
        if (attackElement == ElementType.Light && defenderElement != ElementType.Dark) return 1.25f;
    
        // ธาตุมืดได้เปรียบธาตุทั่วไป (แต่ไม่มากเท่าธาตุหลัก)
        if (attackElement == ElementType.Dark && defenderElement != ElementType.Light) return 1.25f;

        // ความสัมพันธ์ของธาตุหลัก
        if (attackElement == ElementType.Fire && defenderElement == ElementType.Earth) return 1.5f;
        if (attackElement == ElementType.Water && defenderElement == ElementType.Fire) return 1.5f;
        if (attackElement == ElementType.Earth && defenderElement == ElementType.Wind) return 1.5f;
        if (attackElement == ElementType.Wind && defenderElement == ElementType.Water) return 1.5f;
    
        // ธาตุที่เสียเปรียบ
        if (attackElement == ElementType.Earth && defenderElement == ElementType.Fire) return 0.5f;
        if (attackElement == ElementType.Fire && defenderElement == ElementType.Water) return 0.5f;
        if (attackElement == ElementType.Wind && defenderElement == ElementType.Earth) return 0.5f;
        if (attackElement == ElementType.Water && defenderElement == ElementType.Wind) return 0.5f;
    
        // ธาตุทั่วไปจะเสียเปรียบธาตุพิเศษเล็กน้อย
        if ((defenderElement == ElementType.Light || defenderElement == ElementType.Dark) && 
            (attackElement != ElementType.Light && attackElement != ElementType.Dark)) return 0.75f;
    
        return 1f; // ธาตุไม่มีผลต่อกัน
    }

    #endregion
    
    #region Stagger System

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


    #endregion
    private void Die()
    {
        isDead = true;
        if (IsthisBoss)
        {
           // CongratulationUI.gameObject.SetActive(true);
        }
        // Remove Burning
        var burningEffect = gameObject.GetComponent<BurningEffect>();
        if (burningEffect != null)
        {
            burningEffect.Remove();
        }
      
        // play dead sound
        _audioManager.PlayDieSound();
        // add money
        CurrencyManager.Instance.AddMoney( Mathf.RoundToInt((EnemyData.moneyDrop * _enemySpawner.currentStage) *1.25f));
        // add exp
        playerStats.AddExperience(100);
        
        
        animator.SetTrigger("Die");
        GetComponent<NavMeshAgent>().enabled = false;
        Destroy(gameObject,3f);
        enemyCollider.enabled = false;
        animator.SetBool("IsWalking",false);
        animator.SetBool("IsAttacking",false);
        
        currentHealth = 0;
    }

    #region Modifier

  /*  private enum Modifier
    {
        AttackBoost,       // เพิ่มความแรง
        AttackSpeedBoost, // เพิ่มความเร็วการโจมตี
        CriticalHit100,   // ติดคริ 100%
        ArmorBreaker,     // ศัตรูพังเกราะ
        None              // ไม่มี Modifier
    }*/
  /*  private Modifier currentModifier;
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
    }*/

    #endregion
   
    
    public bool IsAlive() => currentHealth > 0;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => EnemyData.maxhealth;
    public bool GetIsDead() => isDead;
   
    public void OnHurtAnimationEnd() => ResetHurt();
    public void ResetHurt()
    {
        animator.SetBool("isHurt", false);
        isHurt = false;
    }
    
}
