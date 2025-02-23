using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillInventoryManager : MonoBehaviour
{
    [SerializeField] private List<SkillData> unlockedSkills = new List<SkillData>();  // สกิลที่ปลดล็อคแล้ว
    [SerializeField] private int skillsPerPage = 8;  // จำนวนสกิลต่อหน้า
    
    private int currentPage = 0;
    public event System.Action OnInventoryChanged;

    // Property สำหรับเช็คว่าเปลี่ยนหน้าได้ไหม
    public bool HasNextPage => (currentPage + 1) * skillsPerPage < unlockedSkills.Count;
    public bool HasPreviousPage => currentPage > 0;
    // เพิ่ม properties เหล่านี้
    public int CurrentPage => currentPage + 1;  // +1 เพราะเราต้องการแสดงเริ่มจาก 1 ไม่ใช่ 0
    public int TotalPages => Mathf.CeilToInt((float)unlockedSkills.Count / skillsPerPage);
    
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
    // ดึงสกิลเฉพาะหน้าปัจจุบัน
    public List<SkillData> GetCurrentPageSkills()
    {
        int startIndex = currentPage * skillsPerPage;
        return unlockedSkills
            .Skip(startIndex)
            .Take(skillsPerPage)
            .ToList();
    }

    public void NextPage()
    {
        if (HasNextPage)
        {
            currentPage++;
            OnInventoryChanged?.Invoke();
        }
    }

    public void PreviousPage()
    {
        if (HasPreviousPage)
        {
            currentPage--;
            OnInventoryChanged?.Invoke();
        }
    }
    
}
