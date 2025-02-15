using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossSkill1 : BaseBossSkill
{
   public GameObject skillIndicatorPrefab; // Prefab ที่มีทั้ง indicator และ effect
   private GameObject spawnedIndicator;
   private GameObject spawnedEffect; // เก็บอ้างอิงของ Effect ที่ Spawn
   [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
   [SerializeField] private float hitboxSpawnDelay = 0.5f; // ระยะเวลาหน่วงก่อนเริ่มโจมตี
   [SerializeField] private float indicatorDuration = 1f;
   public override void UseSkill()
   {
      base.UseSkill();
      SpawnIndicatorAndEffect();
   }
   private void SpawnIndicatorAndEffect()
   {
      // spawn indicator
      Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
      spawnedIndicator = Instantiate(skillIndicatorPrefab, spawnPosition, skillIndicatorPrefab.transform.rotation);

      // เรียก effect ให้ทำงานหลังจากแสดง indicator
      Transform effectHolder = spawnedIndicator.transform.Find("EffectHolder");
      if (effectHolder != null)
      {
         effectHolder.gameObject.SetActive(false); // ให้แน่ใจว่าปิดไว้ตอนเริ่ม
         Invoke(nameof(ShowEffect), indicatorDuration);
      }

      // ทำลาย GameObject หลังจากเสร็จสิ้น
      Destroy(spawnedIndicator, indicatorDuration + 2f); // +2f สำหรับระยะเวลาของ effect
   }

   private void ShowEffect()
   {
      if (spawnedIndicator != null)
      {
         Transform effectHolder = spawnedIndicator.transform.Find("EffectHolder");
         if (effectHolder != null)
         {
            effectHolder.gameObject.SetActive(true);
                
            // เรียกใช้ hitbox
            BossSkillHitbox hitbox = effectHolder.GetComponentInChildren<BossSkillHitbox>();
            if (hitbox != null)
            {
               hitbox.ActivateMultipleHitbox();
            }
         }
      }
   }
      
}
