using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public  abstract class ISkill : MonoBehaviour
{
    public float cooldownTime;
    private float lastUsedTime;
    protected Animator playerAnimator; // เก็บ Animator ของผู้เล่น
    protected bool isOnCooldown = false;
    private float lastUseTime = -Mathf.Infinity;
    public void SetAnimator(Animator animator)
    {
        playerAnimator = animator;
    }

    public abstract void UseSkill();
    
        
    
    private void StartCooldown()
    {
        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    private void ResetCooldown()
    {
        isOnCooldown = false;
    }

    public bool IsOnCooldown() => isOnCooldown;     

  /*  public bool IsOnCooldown()
    {
        return Time.time < lastUsedTime + cooldownTime;
    }*/

    public float GetCooldownTime()
    {
        return cooldownTime;
    }

    public float GetRemainingCooldownTime()
    {
        return Mathf.Max(0, (lastUsedTime + cooldownTime) - Time.time);
    }

    public float GetCooldownPercentage()
    {
        return Mathf.Clamp01((Time.time - lastUsedTime) / cooldownTime);
    }

    protected void ActivateCooldown()
    {
        lastUsedTime = Time.time;
    }
}