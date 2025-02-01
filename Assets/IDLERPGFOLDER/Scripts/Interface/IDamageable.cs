using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // รับดาเมจพื้นฐาน
  //  void TakeDamage(float amount);
    void TakeDamage(float amount ,float armorPen);
    // รับดาเมจพร้อมข้อมูลเพิ่มเติม
    void TakeDamage(DamageData damageData);
    float GetMaxHealth(); // เพิ่ม method นี้
  //  bool isDead { get; }
    // ตรวจสอบว่าวัตถุนี้ยังมีชีวิตอยู่หรือไม่
    //   bool IsAlive();

    // ดึงค่า HP ปัจจุบัน
    //  float GetCurrentHealth();

    // ดึงค่า HP สูงสุด
    //  float GetMaxHealth();
}

// struct สำหรับเก็บข้อมูลดาเมจแบบละเอียด


public struct DamageData
{
    public float damage;
    public float armorPenetration;
    public ElementType elementType;
    public DamageData(float damage, float armorPenetration,ElementType elementType)
    {
        this.damage = damage;
        this.armorPenetration = armorPenetration;
        this.elementType = elementType;
    }
}
[System.Serializable]
public class ElementalResistance
{
    public ElementType elementType;
    public float resistance; // เช่น -50 = รับดาเมจ 150%, +50 = รับดาเมจ 50%
}
// enum สำหรับประเภทของดาเมจ
public enum ElementType
{
    None,
    Fire,
    Water,
    Earth,
    Wind,
    Light,
    Dark
}

