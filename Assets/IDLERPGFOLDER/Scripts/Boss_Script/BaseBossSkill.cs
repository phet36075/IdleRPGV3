using System.Collections;
using UnityEngine;

public abstract class BaseBossSkill : MonoBehaviour
{
    [SerializeField] protected BossSkillData bossSkillData;
    protected Animator animator;
    protected BossBehavior bossBehavior;
    
    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        bossBehavior = GetComponentInParent<BossBehavior>();  // หา PlayerManager ตอน start
    }
    public virtual void UseSkill()
    {
        animator.SetTrigger(bossSkillData.animTriggerName);
    }

    public virtual void OnSkillEnd()
    {
        bossBehavior.IsUsingSkill = false;
    }
}
