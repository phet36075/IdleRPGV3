using System.Collections;
using UnityEngine;

public class SkillHitbox : MonoBehaviour
{
    private PlayerManager playerManager;
    private float damageMultiplier = 1f;  // ถ้าต้องการให้สกิลทำดาเมจต่างกัน

    private void Start()
    {
        // หา PlayerManager จาก parent (ตัวละครที่ใช้สกิล)
        playerManager = GetComponentInParent<PlayerManager>();
    }
    
    public void SetDamageMultiplier(float multiplier)
    {
        this.damageMultiplier = multiplier;
    }
   
    private void OnTriggerEnter(Collider other)
    {
        // เช็คว่าชนกับ enemy ไหม
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null && playerManager != null)
        {
            float attackDamage = playerManager.CalculatePlayerAttackDamage() * damageMultiplier;
            DamageData damageData = new DamageData(
                attackDamage,
                playerManager.playerData.armorPenetration,
                playerManager.playerData.elementType
            );
            target.TakeDamage(damageData);
        }
    }
}
