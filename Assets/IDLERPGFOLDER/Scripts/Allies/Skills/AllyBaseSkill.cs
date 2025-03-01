using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AllyBaseSkill : MonoBehaviour
{
    [SerializeField] protected AllySkillData allySkillData;
    protected bool isOnCooldown = false;
    protected Animator animator;
    protected bool isSkillActive = false;
    public event System.Action<float> OnAllyCooldownStart;
    public event System.Action OnAllyCooldownEnd;
    protected NavMeshAgent agent;
    public bool IsOnCooldown => isOnCooldown;
    public bool IsSkillActive => isSkillActive;
    
    public AllySkillData AllySkillData => allySkillData != null ? allySkillData : null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void SetSkillData(AllySkillData data)
    {
        allySkillData = data;
    }
   
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
       
    }
    public virtual bool CanUseSkill()
    {
       // if (playerManager == null) return false;
        
        // เช็คทั้ง cooldown และ mana
        return !isOnCooldown &&
               !isSkillActive;
        //&& playerManager.HasEnoughMana(skillData.manaCost);
    }
    public virtual void UseSkill()
    {
        if (CanUseSkill())
        {
            isSkillActive = true;
            animator.SetTrigger(allySkillData.animationTriggerName);
            StartCoroutine(CooldownRoutine());
        }
    }
    public virtual void OnSkillStart()
    {
        isSkillActive = true;
        agent.isStopped = true;
        Debug.Log($"Skill {allySkillData.skillName} started");
        Invoke(nameof(OnSkillEnd),4f);
    }

    public virtual void OnHitboxActivate()
    {
        Debug.Log($"Skill {allySkillData.skillName} hitbox activated");
    }

    public virtual void OnEffectSpawn()
    {
        Debug.Log($"Skill {allySkillData.skillName} effect spawned");
    }

    public virtual void OnSkillEnd()
    {
        isSkillActive = false;
        agent.isStopped = false;
        Debug.Log($"Skill {allySkillData.skillName} ended");
    }
    protected IEnumerator CooldownRoutine()
    {
        
        isOnCooldown = true;
        OnAllyCooldownStart?.Invoke(allySkillData.cooldown);
        
        yield return new WaitForSeconds(allySkillData.cooldown);
        
        isOnCooldown = false;
        OnAllyCooldownEnd?.Invoke();
    }
}
