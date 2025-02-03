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

    public SkillData SkillData => skillData != null ? skillData : null;
    
    // Events สำหรับ cooldown
    public event System.Action<float> OnCooldownStart;
    public event System.Action OnCooldownEnd;
    public bool IsOnCooldown => isOnCooldown;
    public bool IsSkillActive => isSkillActive;  // เพิ่ม property นี้
    public virtual void SetSkillData(SkillData data)
    {
        skillData = data;
    }
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void UseSkill()
    {
        if (!isOnCooldown && !isSkillActive)
        {
            isSkillActive = true;
            // Trigger animation ผ่าน parameter
            animator.SetTrigger(skillData.animationTriggerName);
            StartCoroutine(CooldownRoutine());
        }
    }

    // Animation Event Handlers
    public virtual void OnSkillStart()
    {
        Debug.Log($"Skill {skillData.skillName} started");
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