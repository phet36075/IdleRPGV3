using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // รับดาเมจพื้นฐาน
    void TakeDamage(float amount);
    void TakeDamage(float amount ,float armorPen);
    // รับดาเมจพร้อมข้อมูลเพิ่มเติม
    void TakeDamage(DamageInfo damageInfo);

    // ตรวจสอบว่าวัตถุนี้ยังมีชีวิตอยู่หรือไม่
    bool IsAlive();

    // ดึงค่า HP ปัจจุบัน
    float GetCurrentHealth();

    // ดึงค่า HP สูงสุด
    float GetMaxHealth();
}

// struct สำหรับเก็บข้อมูลดาเมจแบบละเอียด
public struct DamageInfo
{
    public float amount; // จำนวนดาเมจ
    public DamageType damageType; // ประเภทของดาเมจ
    public GameObject attacker; // ผู้โจมตี
    public Vector3 hitPoint; // จุดที่โดน

    public DamageInfo(float amount, DamageType damageType, GameObject attacker, Vector3 hitPoint)
    {
        this.amount = amount;
        this.damageType = damageType;
        this.attacker = attacker;
        this.hitPoint = hitPoint;
    }
}

// enum สำหรับประเภทของดาเมจ
public enum DamageType
{
    Physical,
    Fire,
    Ice,
    Lightning,
    Poison
    // เพิ่มประเภทอื่นๆ ตามต้องการ
}

