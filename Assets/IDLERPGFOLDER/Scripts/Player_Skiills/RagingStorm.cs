using UnityEngine;

public class RagingStorm : BaseSkill
{
    [SerializeField] private GameObject RagingStormEffectPrefab;
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
   [SerializeField] private float hitboxSpawnDelay = 0.5f; // ระยะเวลาหน่วงก่อนเริ่มโจมตี
    // เพิ่มตัวแปรสำหรับปรับขนาด
    [Header("Size Settings")]
    [SerializeField] private Vector3 effectScale = Vector3.one;  // ขนาดของ effect
    
    protected override void Start()
    {
        base.Start();
        playerManager = GetComponent<PlayerManager>();
    }

    public void RagingStorm_OnSkillStart()
    {
        base.OnSkillStart();
    }
  

    private GameObject spawnedEffect; // เก็บอ้างอิงของ Effect ที่ Spawn
    public void RagingStorm_OnEffectSpawn()
    {
        
        base.OnEffectSpawn();
        Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
        spawnedEffect = Instantiate(RagingStormEffectPrefab, spawnPosition, RagingStormEffectPrefab.transform.rotation);
    
        // ปรับขนาดตามธาตุ
        spawnedEffect.transform.localScale = CalculateElementalScale(effectScale);
        // **Delay 0.5 วินาทีก่อนเปิด Hitbox**
        Invoke(nameof(RagingStorm_OnHitboxActivate), hitboxSpawnDelay);
        Destroy(spawnedEffect, 5f);
    }
    public void RagingStorm_OnHitboxActivate()
    {
        if (spawnedEffect != null) // ตรวจสอบว่า Effect ถูกสร้างขึ้นหรือยัง
        {
            SkillHitbox hitbox = spawnedEffect.GetComponent<SkillHitbox>();
            if (hitbox != null)
            {
                hitbox.SetDamageMultiplier(skillData.baseSkillDamage,playerManager.GetMaxMana(),skillData.manaMultiplier,skillData.weaponMultiplier);
                hitbox.ActivateMultipleHitbox(); // เปิดใช้งาน Hitbox
            }
        }
        
    }
  
    public  void RagingStorm_OnSkillEnd()
    {
        base.OnSkillEnd();
    }
}
