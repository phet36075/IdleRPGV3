using System.Collections;
using UnityEngine;

// ตัวอย่างสกิล Slash
public class SlashSkill : BaseSkill
{
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private GameObject hitboxPrefab2;
    [SerializeField] private GameObject slashEffectPrefab;
    [SerializeField] private GameObject slashEffectPrefab2;
    [SerializeField] private float damageMultiplier = 1f;  // ตัวคูณดาเมจของสกิลนี้
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
    
    [SerializeField] private int hitCount = 5;  // จำนวนครั้งที่โจมตี
    [SerializeField] private float timeBetweenHits = 0.2f;  // ระยะห่างระหว่างการโจมตีแต่ละครั้ง
    [SerializeField] private float hitboxDuration = 0.1f;  // ระยะเวลาที่ hitbox อยู่ในเกม
    private Coroutine multiHitCoroutine;

    private PlayerManager playerManager;

    // เพิ่มตัวแปรสำหรับปรับขนาด
    [Header("Size Settings")]
    [SerializeField] private Vector3 hitboxScale = Vector3.one;  // ขนาดของ hitbox
    [SerializeField] private Vector3 effectScale = Vector3.one;  // ขนาดของ effect
   
    
    protected override void Start()
    {
        base.Start();
        playerManager = GetComponent<PlayerManager>();
    }
   
    
    public override void OnHitboxActivate()
    {
        base.OnHitboxActivate();
        
        // สร้าง hitbox
        var hitbox = Instantiate(hitboxPrefab, transform.position + transform.rotation * hitboxOffset, transform.rotation, transform);
        
        // ตั้งค่า damage multiplier ถ้าต้องการ
        var skillHitbox = hitbox.GetComponent<SkillHitbox>();
        if (skillHitbox != null)
        {
            skillHitbox.SetDamageMultiplier(damageMultiplier);
        }

        // ปรับขนาดตามธาตุ
        hitbox.transform.localScale = CalculateElementalScale(hitboxScale);
        Destroy(hitbox, 0.1f);
        
    }

   
    public void OnHitboxActivate2()
    {
       
        // เริ่ม Coroutine การโจมตีหลายครั้ง
        if (multiHitCoroutine != null)
        {
            StopCoroutine(multiHitCoroutine);
        }
        multiHitCoroutine = StartCoroutine(MultiHitSequence());
       
    }
    
    public void OnEffectSpawn2()
    {
        base.OnEffectSpawn();
        var effect = Instantiate(slashEffectPrefab2, transform.position, transform.rotation);
        // ปรับขนาดตามธาตุ
        effect.transform.localScale = CalculateElementalScale(effectScale);
        Destroy(effect, 1f);
    }
    public override void OnEffectSpawn()
    {
        base.OnEffectSpawn();
        var effect = Instantiate(slashEffectPrefab, transform.position, transform.rotation);
        // ปรับขนาดตามธาตุ
        effect.transform.localScale = CalculateElementalScale(effectScale);
        Destroy(effect, 1f);
    }
    private IEnumerator MultiHitSequence()
    {
        for (int i = 0; i < hitCount; i++)
        {
            // สร้าง hitbox
            var hitbox = Instantiate(hitboxPrefab2, transform.position + transform.rotation * hitboxOffset, transform.rotation, transform);
            // ปรับขนาดตามธาตุ
            hitbox.transform.localScale = CalculateElementalScale(hitboxScale);
            
            // ทำลาย hitbox หลังจากเวลาที่กำหนด
            Destroy(hitbox, hitboxDuration);

            // รอก่อนสร้าง hitbox ครั้งถัดไป
            yield return new WaitForSeconds(timeBetweenHits);
        }
    }
    public override void OnSkillEnd()
    {
        base.OnSkillEnd();
        // ยกเลิก Coroutine ถ้ายังทำงานอยู่
        if (multiHitCoroutine != null)
        {
            StopCoroutine(multiHitCoroutine);
            multiHitCoroutine = null;
        }
    }
}
