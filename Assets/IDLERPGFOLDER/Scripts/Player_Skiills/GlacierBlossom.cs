using UnityEngine;

public class GlacierBlossom : BaseSkill
{
      [SerializeField] private GameObject GlacierBlossomEffectPrefab;
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
    [SerializeField] private float hitboxSpawnDelay = 0.5f; // ระยะเวลาหน่วงก่อนเริ่มโจมตี
    // เพิ่มตัวแปรสำหรับปรับขนาด
    [Header("Size Settings")]
    [SerializeField] private Vector3 effectScale;  // ขนาดของ effect
    
    protected override void Start()
    {
        base.Start();
        playerManager = GetComponent<PlayerManager>();
    }

    public void GlacierBlossom_OnSkillStart()
    {
        base.OnSkillStart();
    }
  

    private GameObject spawnedEffect; // เก็บอ้างอิงของ Effect ที่ Spawn
    public void GlacierBlossom_OnEffectSpawn()
    {
        
        base.OnEffectSpawn();
        Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
        spawnedEffect = Instantiate(GlacierBlossomEffectPrefab, spawnPosition, GlacierBlossomEffectPrefab.transform.rotation);
    
        // ปรับขนาดตามธาตุ
        spawnedEffect.transform.localScale = CalculateElementalScale(effectScale);
        // **Delay 0.5 วินาทีก่อนเปิด Hitbox**
        Invoke(nameof(GlacierBlossom_OnHitboxActivate), hitboxSpawnDelay);
        Destroy(spawnedEffect, 2f);
    }
    public void GlacierBlossom_OnHitboxActivate()
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

    public void GlacierBlossom_OnSingleHitboxActivate()
    {
        SkillHitbox hitbox = spawnedEffect.GetComponent<SkillHitbox>();
        if (hitbox != null)
        {
            hitbox.SetDamageMultiplier(skillData.baseSkillDamage,playerManager.GetMaxMana(),skillData.manaMultiplier,skillData.weaponMultiplier);
            hitbox.ActivateHitbox(); // เปิดใช้งาน Hitbox
        }
    }
  
    public  void GlacierBlossom_OnSkillEnd()
    {
        base.OnSkillEnd();
    }
}
