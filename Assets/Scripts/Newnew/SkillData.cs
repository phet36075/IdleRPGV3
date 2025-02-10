using UnityEngine;

// ปรับปรุง SkillData
[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public ElementType elementType = ElementType.None;
    public string skillName;
    public string animationTriggerName;
    public Sprite skillIcon;
    public float cooldown;
    public float damage;
    public float baseSkillDamage ;
    public float baseSkillDamage2;
    public float manaMultiplier;
    public float weaponMultiplier;
    [SerializeField] private string skillComponentTypeName;  // เก็บชื่อ class ของ skill component
    public float manaCost;  // เพิ่มค่า mana ที่ต้องใช้

    [TextArea(3, 10)]
    public string description;  // เพิ่มคำอธิบายสกิล
    
    // ใช้ชื่อ class ในการสร้าง Type (เช่น "SlashSkill")
    public System.Type GetSkillComponentType()
    {
        return System.Type.GetType(skillComponentTypeName);
    }
}