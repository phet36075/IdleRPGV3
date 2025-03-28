using System.Collections;
using UnityEngine;

public class BossSkillHitbox : MonoBehaviour
{
    public Collider hitbox; // อ้างอิง Collider ที่เป็น Hitbox
    public int hitCount = 5; // จำนวนครั้งที่ Hitbox ทำงาน
    public float hitInterval = 0.2f; // เวลาระหว่างแต่ละ Hit
    public float skillMultiplier;
    private EnemyHealth enemyHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitbox.enabled = false; // ปิด Hitbox ตอนเริ่มต้น
        //StartCoroutine(EnableHitboxContinuously());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateContinuouslyHiitbox(EnemyHealth enemyHealth)
    {
        this.enemyHealth = enemyHealth;
        StartCoroutine(EnableHitboxContinuously());
    }
    
    public void ActivateMultipleHitbox(EnemyHealth enemyHealth = null)
    {
        this.enemyHealth = enemyHealth;
        StartCoroutine(EnableHitboxMultipleTimes());
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
    IEnumerator EnableHitboxContinuously()
    {
        while (true) // ใช้ loop แบบไม่มีที่สิ้นสุด
        {
            hitbox.enabled = true; // เปิด Hitbox
            yield return new WaitForSeconds(0.1f); // เปิดให้โจมตีได้สั้น ๆ
            hitbox.enabled = false; // ปิด Hitbox
            yield return new WaitForSeconds(hitInterval); // รอเวลาก่อน Hit รอบต่อไป
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy")) 
        {
            return; 
        }
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            
            int finalDamage = enemyHealth.CalculateAttackDamage();
            DamageData damageData = new DamageData(
                finalDamage * skillMultiplier,
                enemyHealth.EnemyData.armorPenetration,
                ElementType.Dark);
            target.TakeDamage(damageData);
        }
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     IDamageable target = other.GetComponent<IDamageable>();
    //     if (target != null)
    //     {
    //         DamageData damageData = new DamageData(
    //             5f,
    //             100f,
    //             ElementType.Dark);
    //         target.TakeDamage(damageData);
    //     }
    // }
}
