using UnityEngine;

public abstract class ISkillParticle : MonoBehaviour
{
    protected PlayerManager playerManager;
    protected float attackDamage;
    protected DamageData damageData;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   protected virtual void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        InitializeDamage();
    }
    protected virtual void InitializeDamage()
    {
        attackDamage = playerManager.CalculatePlayerAttackDamage();
        damageData = new DamageData(
            attackDamage, 
            playerManager.playerData.armorPenetration,
            playerManager.playerData.elementType
        );
    }
    
    protected abstract void UpdateDamage(); // ให้แต่ละ skill กำหนดวิธีการ update damage เอง

    protected abstract void OnTriggerEnter(Collider other); // ให้แต่ละ skill กำหนดวิธีการ update damage เอง
    // Update is called once per frame
    void Update()
    {
        
    }
}
