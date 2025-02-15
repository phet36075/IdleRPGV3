using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossSkill1 : BaseBossSkill
{
   public GameObject skillIndicatorPrefab;
   public GameObject effectPrefab;  // แยก effect ออกมาเป็น prefab ต่างหาก
   private GameObject spawnedIndicator;
   [SerializeField] private Vector3 hitboxOffset = Vector3.forward;
   [SerializeField] private float indicatorDuration = 1f;
   [SerializeField] private float hitboxSpawnDelay = 0.5f; // ระยะเวลาหน่วงก่อนเริ่มโจมตี
   public override void UseSkill()
   {
      base.UseSkill(); 
      //SpawnIndicator();
   }

   public void SpawnIndicator()
   {
      Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
      spawnedIndicator = Instantiate(skillIndicatorPrefab, spawnPosition, skillIndicatorPrefab.transform.rotation);

      // เริ่ม fade out หลังจากแสดง indicator ครบเวลา
      IndicatorFadeEffect fadeEffect = spawnedIndicator.GetComponent<IndicatorFadeEffect>();
      if (fadeEffect != null)
      {
         // ตั้งเวลาเริ่ม fade out
         Invoke(nameof(StartIndicatorFadeOut), indicatorDuration);
      }
   }

   private void StartIndicatorFadeOut()
   {
      if (spawnedIndicator != null)
      {
         IndicatorFadeEffect fadeEffect = spawnedIndicator.GetComponent<IndicatorFadeEffect>();
         if (fadeEffect != null)
         {
            // เริ่ม fade out และระบุ callback เมื่อ fade out เสร็จ
            fadeEffect.StartFadeOut(SpawnEffect);
         }
      }
   }
   private GameObject spawnedEffect; // เก็บอ้างอิงของ Effect ที่ Spawn
   private void SpawnEffect()
   {
      // สร้าง effect หลังจาก indicator หายไป
      Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
      spawnedEffect = Instantiate(effectPrefab, spawnPosition, effectPrefab.transform.rotation);
        
      // เรียกใช้ hitbox
      
      Invoke(nameof(BossSkill1_OnHitboxActivate), hitboxSpawnDelay);
      // ทำลาย effect หลังจากเวลาที่กำหนด
      Destroy(spawnedEffect, 2f);
   }
   
   public void BossSkill1_OnHitboxActivate()
   {
      if (spawnedEffect != null) // ตรวจสอบว่า Effect ถูกสร้างขึ้นหรือยัง
      {
         BossSkillHitbox hitbox = spawnedEffect.GetComponent<BossSkillHitbox>();
         if (hitbox != null)
         {
            hitbox.ActivateMultipleHitbox();
         }
      }
        
   }

   public void BossSkill1_OnSkillEnd()
   {
      base.OnSkillEnd();
   }
   
}
