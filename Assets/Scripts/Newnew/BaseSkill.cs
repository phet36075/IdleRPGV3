using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class สำหรับทุกสกิล
public abstract class BaseSkill : MonoBehaviour
{
    [SerializeField] protected SkillData skillData;
    protected bool isOnCooldown = false;
    protected Animator animator;
    protected bool isSkillActive = false;
    protected PlayerManager playerManager;  
    protected PlayerMovement playerMovement;
    protected WeaponSystem weaponSystem;
    public SkillData SkillData => skillData != null ? skillData : null;
    
    // Events สำหรับ cooldown
    public event System.Action<float> OnCooldownStart;
    public event System.Action OnCooldownEnd;
    public bool IsOnCooldown => isOnCooldown;
    public bool IsSkillActive => isSkillActive;

    public int GetPowerBonus()
    {
        return skillData.skillPower;
    }
    [SerializeField] private int powerBonus = 0; // ค่าพลังที่สกิลเพิ่มให้
    public virtual void SetSkillData(SkillData data)
    {
        skillData = data;
    }
   
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        playerManager = GetComponentInParent<PlayerManager>();  // หา PlayerManager ตอน start
        playerMovement = GetComponentInParent<PlayerMovement>();  
        weaponSystem = GetComponentInParent<WeaponSystem>();
    }

    public virtual bool CanUseSkill()
    {
        if (playerManager == null) return false;
        
        // เช็คทั้ง cooldown และ mana
        return !isOnCooldown && 
             //  !isSkillActive && 
               playerManager.HasEnoughMana(skillData.manaCost) && weaponSystem.GetIsDrawn;
    }
    public virtual void UseSkill()
    {
        if (CanUseSkill())
        {
            // ใช้ mana
           weaponSystem.ResetIdleTimer();
            playerManager.UseMana(skillData.manaCost);
            isSkillActive = true;
            animator.SetTrigger(skillData.animationTriggerName);
            StartCoroutine(CooldownRoutine());
        }
    }
// เพิ่มเมธอดสำหรับเช็คธาตุ
    protected bool IsWindElement()
    {
        if (playerManager != null)
        {
            return playerManager.playerProperty.elementType == ElementType.Wind;
        }
        return false;
    }

    // เพิ่มเมธอดสำหรับคำนวณขนาด
    protected Vector3 CalculateElementalScale(Vector3 baseScale)
    {
        return IsWindElement() ? baseScale * 1.5f : baseScale;
    }

    protected void ElementalCheck()
    {
        if (skillData.elementType == playerManager.playerProperty.elementType)
        {
            Debug.Log("HOLYYYYYYYYYYYYYYYYYYYYYYYYYYYY");
        }
    }
    
    // Animation Event Handlers
    public virtual void OnSkillStart()
    {
        isSkillActive = true;
        ElementalCheck();
        playerMovement.isTakingAction = true;
        Debug.Log($"Skill {skillData.skillName} started");
        Invoke(nameof(OnSkillEnd),4f);
    }

    public virtual void OnHitboxActivate()
    {
        Debug.Log($"Skill {skillData.skillName} hitbox activated");
    }

    public virtual void OnEffectSpawn()
    {
        Debug.Log($"Skill {skillData.skillName} effect spawned");
    }

    public virtual void OnSkillEnd()
    {
        isSkillActive = false;
        playerMovement.isTakingAction = false;
        Debug.Log($"Skill {skillData.skillName} ended");
    }

    protected IEnumerator CooldownRoutine()
    {
        
        isOnCooldown = true;
        OnCooldownStart?.Invoke(skillData.cooldown);
        
        yield return new WaitForSeconds(skillData.cooldown);
        
        isOnCooldown = false;
        OnCooldownEnd?.Invoke();
    }
}