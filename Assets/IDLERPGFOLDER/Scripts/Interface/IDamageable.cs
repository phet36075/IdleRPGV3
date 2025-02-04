using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // รับดาเมจพื้นฐาน
  //  void TakeDamage(float amount);
    void TakeDamage(float amount ,float armorPen);
    
    void TakeDamage(float amount ,float armorPen, WeaponType weaponType);
    // รับดาเมจพร้อมข้อมูลเพิ่มเติม
    void TakeDamage(DamageData damageData);
    float GetMaxHealth(); // เพิ่ม method นี้
 
}

public struct DamageData
{
    public float damage;
    public float armorPenetration;
    public ElementType elementType;
    public bool isEarthTremor; // เพิ่มเพื่อป้องกันการเกิด tremor ซ้ำซ้อน
    
    public DamageData(float damage, float armorPenetration,ElementType elementType)
    {
        this.damage = damage;
        this.armorPenetration = armorPenetration;
        this.elementType = elementType;
        this.isEarthTremor = false; // กำหนดค่าเริ่มต้นเป็น false
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
public enum WeaponType
{
    Sword,
    Mace,
    Spell
    
}

