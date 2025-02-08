using UnityEngine;
using System.Collections;
public class JudgementsLight : BaseSkill
{
   // [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private GameObject orbitalEffectPrefab;
    
    [SerializeField] private float damageMultiplier = 1f;  // ตัวคูณดาเมจของสกิลนี้
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
    
    //[SerializeField] private int hitCount = 5;  // จำนวนครั้งที่โจมตี
    //[SerializeField] private float timeBetweenHits = 0.2f;  // ระยะห่างระหว่างการโจมตีแต่ละครั้ง
  //  [SerializeField] private float hitboxDuration = 0.1f;  // ระยะเวลาที่ hitbox อยู่ในเกม
  //  private Coroutine multiHitCoroutine;
  // private PlayerManager playerManager;
   [SerializeField] private float hitboxSpawnDelay = 0.5f; // ระยะเวลาหน่วงก่อนเริ่มโจมตี
    // เพิ่มตัวแปรสำหรับปรับขนาด
    [Header("Size Settings")]
    [SerializeField] private Vector3 hitboxScale = Vector3.one;  // ขนาดของ hitbox
    [SerializeField] private Vector3 effectScale = Vector3.one;  // ขนาดของ effect
   
    protected override string GetHitboxEventName() => "JudgementsLight_OnHitboxActivate";
    protected override string GetEffectEventName() => "JudgementsLight_OnEffectSpawn";
    protected override string  GetSkillEndEventName() => "JudgementsLight_OnSkillEnd";

   // private GameObject effect;
    protected override void Start()
    {
        base.Start();
        playerManager = GetComponent<PlayerManager>();
    }

    public void JudgementsLight_OnSkillStart()
    {
        base.OnSkillStart();
    }
  

    private GameObject spawnedEffect; // เก็บอ้างอิงของ Effect ที่ Spawn
    public  void JudgementsLight_OnEffectSpawn()
    {
        
        base.OnEffectSpawn();
        Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
        spawnedEffect = Instantiate(orbitalEffectPrefab, spawnPosition, orbitalEffectPrefab.transform.rotation);
    
        // ปรับขนาดตามธาตุ
        spawnedEffect.transform.localScale = CalculateElementalScale(effectScale);
        // **Delay 0.5 วินาทีก่อนเปิด Hitbox**
        Invoke(nameof(JudgementsLight_OnHitboxActivate), 2f);
        Destroy(spawnedEffect, 5f);
    }
    public void JudgementsLight_OnHitboxActivate()
    {
        if (spawnedEffect != null) // ตรวจสอบว่า Effect ถูกสร้างขึ้นหรือยัง
        {
            SkillHitbox hitbox = spawnedEffect.GetComponent<SkillHitbox>();
            if (hitbox != null)
            {
                hitbox.SetDamageMultiplier(damageMultiplier);
                hitbox.ActivateMultipleHitbox(); // เปิดใช้งาน Hitbox
            }
        }
        
    }
  /*  private IEnumerator MultiHitSequence()
    {
        // รอเวลาก่อนเริ่มโจมตี
        //yield return new WaitForSeconds(hitboxSpawnDelay);
            
        for (int i = 0; i < hitCount; i++)
        {
            // คำนวณตำแหน่งที่จะ Spawn Hitbox
            Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;

            // วาดเส้นสีแดงไปที่ตำแหน่งของ Hitbox
            Debug.DrawLine(transform.position, spawnPosition, Color.magenta, 10f);

            // สร้าง hitbox
            var hitbox = Instantiate(hitboxPrefab, spawnPosition,transform.rotation);
            // ปรับขนาดตามธาตุ
            hitbox.transform.localScale = CalculateElementalScale(hitboxScale);
            
            // ทำลาย hitbox หลังจากเวลาที่กำหนด
            Destroy(hitbox, hitboxDuration);

            // รอก่อนสร้าง hitbox ครั้งถัดไป
            yield return new WaitForSeconds(timeBetweenHits);
        }
    }*/

  /*  private IEnumerator DelaySkill()
    {
        yield return new WaitForSeconds(hitboxSpawnDelay);
        // สร้าง hitbox
        var hitbox = Instantiate(hitboxPrefab, transform.position + transform.rotation * hitboxOffset, transform.rotation,transform);
        
        // ตั้งค่า damage multiplier ถ้าต้องการ
        var skillHitbox = hitbox.GetComponent<SkillHitbox>();
        if (skillHitbox != null)
        {
            skillHitbox.SetDamageMultiplier(damageMultiplier);
        }

        // ปรับขนาดตามธาตุ
        hitbox.transform.localScale = CalculateElementalScale(hitboxScale);
        Destroy(hitbox, 5.1f);
    }*/
    public  void JudgementsLight_OnSkillEnd()
    {
        base.OnSkillEnd();
        // ยกเลิก Coroutine ถ้ายังทำงานอยู่
       // if (multiHitCoroutine != null)
       // {
       //     StopCoroutine(multiHitCoroutine);
       //     multiHitCoroutine = null;
       // }
    }
}
