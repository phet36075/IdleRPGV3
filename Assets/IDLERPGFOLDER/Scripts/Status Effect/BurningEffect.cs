using UnityEngine;

// Component สำหรับจัดการ Burning effect
public class BurningEffect : MonoBehaviour, IStatusEffect
{
    private IDamageable target;
    private float duration = 5f; // ระยะเวลาที่โดนเผา
    private float tickInterval = 0.5f; // ช่วงเวลาระหว่างการทำดาเมจแต่ละครั้ง
    private float damagePercentage = 0.05f; 
    private float remainingDuration;
    private float nextTickTime;
    private bool isActive;
    private bool isBurning;

    private void Awake()
    {
        target = GetComponent<IDamageable>();
    }

    public bool IsActive => isActive;
    public float RemainingDuration => remainingDuration;

    public void Apply()
    {
        isActive = true;
        remainingDuration = duration;
        nextTickTime = Time.time;
        enabled = true; // Enable component
    }

    public void Remove()
    {
        isActive = false;
        enabled = false; // Disable component
    }

    private void Update()
    {
        if (!isActive) return;

        remainingDuration -= Time.deltaTime;
        
        if (remainingDuration <= 0)
        {
            Remove();
            return;
        }

        if (Time.time >= nextTickTime)
        {
            Tick();
            nextTickTime = Time.time + tickInterval;
        }
    }

    public void Tick()
    {
        if (!isActive || target == null) return;

        // สร้าง DamageData สำหรับ burning damage
        var damageData = new DamageData(
            damage: GetBurningDamage(),
            armorPenetration: 0f,
            elementType: ElementType.Extra
        );

        target.TakeDamage(damageData);
    }

    private float GetBurningDamage()
    {
        // คำนวณดาเมจตาม % ของ max HP
        // ในกรณีนี้ต้องเพิ่ม method GetMaxHealth() ใน IDamageable
        return target.GetMaxHealth() * damagePercentage;
    }

    public float GetDuration() => duration;
}

