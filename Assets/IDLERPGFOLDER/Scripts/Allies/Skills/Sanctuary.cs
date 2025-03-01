using UnityEngine;

public class Sanctuary : AllyBaseSkill
{
      
    [SerializeField] private GameObject SanctuaryPrefab;
    //[SerializeField] private float damageMultiplier = 1f;  // ตัวคูณดาเมจของสกิลนี้
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
   
    [SerializeField] private float hitboxSpawnDelay = 0.5f; // ระยะเวลาหน่วงก่อนเริ่มโจมตี
    // เพิ่มตัวแปรสำหรับปรับขนาด
    [Header("Size Settings")]
    //[SerializeField] private Vector3 hitboxScale = Vector3.one;  // ขนาดของ hitbox
    [SerializeField] private Vector3 effectScale = Vector3.one;  // ขนาดของ effect
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        //playerManager = GetComponent<PlayerManager>();
    }

    public void JudgementsLight_OnSkillStart()
    {
        base.OnSkillStart();
    }
  

    private GameObject spawnedEffect; // เก็บอ้างอิงของ Effect ที่ Spawn
    public  void Sanctuary_OnEffectSpawn()
    {
        base.OnEffectSpawn();
        Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
        spawnedEffect = Instantiate(SanctuaryPrefab, spawnPosition, SanctuaryPrefab.transform.rotation);
    
        // ปรับขนาดตามธาตุ
        spawnedEffect.transform.localScale = effectScale; 
        // **Delay 0.5 วินาทีก่อนเปิด Hitbox**
      //  Invoke(nameof(JudgementsLight_OnHitboxActivate), hitboxSpawnDelay);
        Destroy(spawnedEffect, 5f);
    }
    // public void JudgementsLight_OnHitboxActivate()
    // {
    //     if (spawnedEffect != null) // ตรวจสอบว่า Effect ถูกสร้างขึ้นหรือยัง
    //     {
    //         SkillHitbox hitbox = spawnedEffect.GetComponent<SkillHitbox>();
    //         if (hitbox != null)
    //         {
    //             hitbox.SetDamageMultiplier(skillData.baseSkillDamage,playerManager.GetMaxMana(),skillData.manaMultiplier,skillData.weaponMultiplier);
    //             hitbox.ActivateMultipleHitbox(); // เปิดใช้งาน Hitbox
    //         }
    //     }
    //     
    // }
  
    public  void SanctuaryPrefab_OnSkillEnd()
    {
        base.OnSkillEnd();
    }
}
