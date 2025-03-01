using System.Collections;
using UnityEngine;

public class SkillHitbox : MonoBehaviour
{
    public bool isHealingSkill;
    private PlayerManager playerManager;
    private float damageMultiplier = 1f;  // ถ้าต้องการให้สกิลทำดาเมจต่างกัน
    private float weaponMultiplier;
    public Collider hitbox; // อ้างอิง Collider ที่เป็น Hitbox
    public float activeTime = 0.3f; // ระยะเวลาที่ Hitbox ทำงาน
    
    public int hitCount = 5; // จำนวนครั้งที่ Hitbox ทำงาน
    public float hitInterval = 0.2f; // เวลาระหว่างแต่ละ Hit
    public bool isPulling = false;
    public bool isPushing = false;
   
    public float pushForce;
    private Status StatusSkill;
    public bool isMultiplierNextHit = false;
    private float multiplierNextHit;
    public float multiplierForNextHit;
    
    private void Start()
    {
        // หา PlayerManager จาก parent (ตัวละครที่ใช้สกิล)
        playerManager = FindAnyObjectByType<PlayerManager>();
         hitbox.enabled = false; // ปิด Hitbox ตอนเริ่มต้น
         multiplierNextHit = 1f;
    }
    public void ActivateHitbox()
    {
        StartCoroutine(EnableHitbox());
    }
    public void ActivateMultipleHitbox()
    {
        StartCoroutine(EnableHitboxMultipleTimes());
    }
    IEnumerator EnableHitbox()
    {
        Debug.Log("HITBOX ENABLE");
        hitbox.enabled = true; // เปิด Hitbox
        yield return new WaitForSeconds(activeTime); // รอให้ทำงาน
        hitbox.enabled = false; // ปิด Hitbox
    }
    
    IEnumerator EnableHitboxMultipleTimes()
    {
        for (int i = 0; i < hitCount; i++) 
        {
            hitbox.enabled = true; // เปิด Hitbox
            yield return new WaitForSeconds(0.1f); // เปิดให้โจมตีได้สั้น ๆ
            hitbox.enabled = false; // ปิด Hitbox
            yield return new WaitForSeconds(hitInterval); // รอเวลาก่อน Hit รอบต่อไป
        }
    }

    public void PullMonster()
    {
        
    }
    public void SetDamageMultiplier(float baseSkillDamage,float maxMana,float manaMultiplier,float weaponMultiplier, Status status = Status.None)
    {
        this.damageMultiplier = baseSkillDamage + ( maxMana * manaMultiplier) ;
        this.weaponMultiplier = weaponMultiplier;
        StatusSkill = status;
    }
   
   
    private void OnTriggerEnter(Collider other)
    {
        // เช็คว่าไม่ใช่ Player
        if (!isHealingSkill)
        {
            if (other.CompareTag("Player"))
            {
                return; 
            }
            
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null && playerManager != null)
            {
                float attackDamage = playerManager.CalculatePlayerAttackDamage()  * weaponMultiplier + damageMultiplier ;
            
                // Status status = Status.None; // กำหนดค่าเริ่มต้น
                Multiple multiple = Multiple.None;
                /*  if (StatusSkill == Status.Freezing)
                  {
                      status = Status.Freezing;
                  }

                  if (StatusSkill == Status.Radiant)
                  {
                      status = Status.Radiant;
                  }*/
                if (isMultiplierNextHit)
                {
                    multiple = Multiple.Yes;
                    multiplierNextHit = 1.8f;
                }
            
                DamageData damageData = new DamageData(
                    attackDamage,
                    playerManager.playerProperty.armorPenetration,
                    playerManager.playerProperty.elementType,StatusSkill,multiple,multiplierNextHit
                );
                target.TakeDamage(damageData);
                
           
            
            }
            
        }

        playerManager.Heal(50f);
        // เช็คว่าชนกับ enemy ไหม
       
           
       

        if (isPulling)
        {
            Rigidbody enemy = other.GetComponent<Rigidbody>();
            Vector3 direction = (transform.position - enemy.transform.position).normalized;
            enemy.AddForce(direction * 20,ForceMode.Force);
        }

        if (isPushing)
        {
            Rigidbody enemy = other.GetComponent<Rigidbody>();
            Vector3 direction = -(transform.position - enemy.transform.position).normalized;
            enemy.AddForce(direction * pushForce,ForceMode.Force);
        }
        
    }
}
