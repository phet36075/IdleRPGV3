using System.Collections.Generic;
using UnityEngine;

public class SkillInventoryManager : MonoBehaviour
{
    [SerializeField] private List<SkillData> unlockedSkills = new List<SkillData>();  // สกิลที่ปลดล็อคแล้ว
    public event System.Action OnInventoryChanged;

    // เพิ่มสกิลใหม่เข้า inventory
    public void UnlockSkill(SkillData skillData)
    {
        if (!unlockedSkills.Contains(skillData))
        {
            unlockedSkills.Add(skillData);
            OnInventoryChanged?.Invoke();
        }
    }

    // ดึงรายการสกิลทั้งหมด
    public List<SkillData> GetUnlockedSkills()
    {
        return unlockedSkills;
    }

    // เช็คว่ามีสกิลนี้ใน inventory ไหม
    public bool HasSkill(SkillData skillData)
    {
        return unlockedSkills.Contains(skillData);
    }
}
