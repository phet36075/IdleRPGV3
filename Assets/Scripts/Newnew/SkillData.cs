using UnityEngine;

// ปรับปรุง SkillData
[CreateAssetMenu(fileName = "SkillData", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public string animationTriggerName;
    public Sprite skillIcon;
    public float cooldown;
    public float damage;
    [SerializeField] private string skillComponentTypeName;  // เก็บชื่อ class ของ skill component

    [TextArea(3, 10)]
    public string description;  // เพิ่มคำอธิบายสกิล
    
    // ใช้ชื่อ class ในการสร้าง Type (เช่น "SlashSkill")
    public System.Type GetSkillComponentType()
    {
        return System.Type.GetType(skillComponentTypeName);
    }
}