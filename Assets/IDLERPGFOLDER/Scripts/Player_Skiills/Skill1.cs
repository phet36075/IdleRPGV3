using System.Collections;
using UnityEngine;

public class Skill1 : MonoBehaviour, ISkill
{
    #region Variables
    [Header("Skill Settings")]
    public float skillDmgFirst2Hit = 1.5f;
    public float skillDmgLastHitAttack = 0.6f;
    public float cooldownTime = 5f;
    public float firstDamageRadius = 2f;
    public float firstDamageDistance = 2f;
    public int numberOfHits = 5;
    public float timeBetweenHits = 0.2f;
    private float skillDuration = 3f;

    [Header("Visual Effects")]
    public GameObject skillEffectPrefab;
    public GameObject skillEffectPrefabSlash;
    public Animator animator;

    [Header("AoE Settings")]
    public Vector3 AoEMods = Vector3.one;
    public float AoEModsMultiplier = 1f;
    private float defaultAoEModsMultiplier = 1f;

    // Cooldown Management
    private bool isOnCooldown = false;
    private float lastUseTime = -Mathf.Infinity;

    // Component References
    private PlayerController _aiController;
    private PlayerManager playerManager;
    private float attackDamage;
    private DamageData damageData;
    #endregion

    #region Initialization
    private void Start()
    {
        InitializeComponents();
        InitializeSkillData();
    }

    private void InitializeComponents()
    {
        playerManager = GetComponent<PlayerManager>();
        _aiController = FindObjectOfType<PlayerController>();
    }

    private void InitializeSkillData()
    {
        AoEModsMultiplier = defaultAoEModsMultiplier;
        attackDamage = playerManager.CalculatePlayerAttackDamage();
        damageData = new DamageData(
            attackDamage, 
            playerManager.playerData.armorPenetration,
            playerManager.playerData.elementType
        );
    }
    #endregion

    #region Skill Execution
    public void UseSkill()
    {
        if (isOnCooldown) 
        {
            Debug.Log($"YOUR ON COOLDOWN {cooldownTime - (Time.time - lastUseTime)}");
            return;
        }

        UpdateDamageData();
        AdjustAoEForElement();
        
        lastUseTime = Time.time;
        StartCoroutine(Cooldown());
        animator.SetTrigger("UseSkill");
    }

    private void UpdateDamageData()
    {
        damageData = new DamageData(
            attackDamage, 
            playerManager.playerData.armorPenetration,
            playerManager.playerData.elementType
        );
    }

    private void AdjustAoEForElement()
    {
        if (damageData.elementType == ElementType.Wind)
        {
            AoEMods = new Vector3(2f, 2f, 2f);
        }
        else
        {
            AoEModsMultiplier = defaultAoEModsMultiplier;
            AoEMods = Vector3.one;
        }
    }
    #endregion

    #region Visual Effects
    public void PerformLastHit()
    {
        LastHit();
    }

    private void LastHit()
    {
        Vector3 effectPosition = transform.position + transform.forward * 2f;
        GameObject effect = Instantiate(skillEffectPrefab, effectPosition, transform.rotation);
        effect.transform.localScale = AoEMods;
        Destroy(effect, 2f);
    }

    public void DoAnimDamage()
    {
        Vector3 effectPosition = transform.position + transform.forward * 2f;
        GameObject effect = Instantiate(skillEffectPrefabSlash, effectPosition, transform.rotation);
        effect.transform.localScale = AoEMods;
        Destroy(effect, 0.2f);
    }
    #endregion

    #region Cooldown Management
    private IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    public float GetCooldownPercentage()
    {
        if (!isOnCooldown) return 0f;
        
        float elapsedTime = Time.time - lastUseTime;
        return 1f - Mathf.Clamp01(elapsedTime / cooldownTime);
    }

    public float GetRemainingCooldownTime()
    {
        if (!isOnCooldown) return 0f;
        
        float timeSinceUse = Time.time - lastUseTime;
        return cooldownTime - timeSinceUse;
    }

    public bool IsOnCooldown() => isOnCooldown;
    public float GetCooldownTime() => cooldownTime;
    #endregion

    #region Navigation Control
    public void EnableNav()
    {
        _aiController.isAIActive = true;
    }

    public void DisableNav()
    {
        _aiController.isAIActive = false;
    }
    #endregion
}