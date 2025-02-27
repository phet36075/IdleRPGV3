using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// SkillManager สำหรับจัดการสกิลทั้งหมด
public class SkillManager : MonoBehaviour
{
   
// Event สำหรับแจ้งเมื่อมีการเปลี่ยนแปลง skills
    private const int MAX_SKILLS = 3;
    [SerializeField] public List<BaseSkill> skills = new List<BaseSkill>();
    public event System.Action OnSkillsChanged;
    [SerializeField] private SkillInventoryManager skillInventory;
    [SerializeField] private SkillData skillsToUnlock,skilltoUnlock2;  // ลาก SkillData ที่จะปลดล็อคใส่ใน inspector
    [SerializeField] private PlayerStats playerStats;

    [SerializeField] private PowerManager powerManager;
    // Property สำหรับเข้าถึง skills จากภายนอก
    public IReadOnlyList<BaseSkill> Skills => skills;
    private int currentSkillIndex = 0;  // ตำแหน่งสกิลปัจจุบัน
    private void Start()
    {
       // SlashSkill slashSkill = GetComponent<SlashSkill>();
       // AddSkill(slashSkill);
    }

    void Update()
    {
        // ตัวอย่างการใช้ Input
       
        if (Input.GetKeyDown(KeyCode.Q)) UseSkill(0);
        if (Input.GetKeyDown(KeyCode.E)) UseSkill(1);
        if (Input.GetKeyDown(KeyCode.R)) UseSkill(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
           // skillInventory.UnlockSkill(skillsToUnlock);
        }
      /*  if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SlashSkill slashSkill = GetComponent<SlashSkill>();
            AddSkill(slashSkill);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
           RemoveSkillAtIndex(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwapSkills(0, 1);
        }*/
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
           // skillInventory.UnlockSkill(skilltoUnlock2);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // UseNextAvailableSkill();
        }
    }
    public void UnequipSkill(BaseSkill skill)
    {
        if (skills.Contains(skill))
        {
           // powerManager.DecreasePower(skill.GetPowerBonus());
          // playerStats.RecalculatePower(); // อัปเดตค่าพลังใหม่ทุกครั้งที่เพิ่มสกิล
            skills.Remove(skill);
            Destroy(skill);  // ลบ component ออกจาก GameObject
            OnSkillsChanged?.Invoke();
        }
    }
    public void UseSkill(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            skills[index].UseSkill();
        }
    }
    // ใช้สกิลถัดไปที่พร้อมใช้งาน
    public bool UseNextAvailableSkill()
    {
        if (skills.Count == 0) return false;  // Return false if no skills exist

        for (int i = 0; i < skills.Count; i++)
        {
            BaseSkill currentSkill = skills[i];
        
            if (!currentSkill.IsOnCooldown)
            {
                currentSkill.UseSkill();
                return true;
            }
        }

        Debug.Log("No available skills to use!");
        return false;
    }

   
    
    // รีเซ็ตลำดับการใช้สกิล
    public void ResetSkillSequence()
    {
        currentSkillIndex = 0;
    }

    // ใช้สกิลที่ตำแหน่งที่กำหนด (ถ้าพร้อมใช้งาน)
    public void UseSkillAtIndex(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            BaseSkill skill = skills[index];
            if (!skill.IsOnCooldown && !skill.IsSkillActive)
            {
                skill.UseSkill();
            }
        }
    }

    // เช็คว่ามีสกิลที่พร้อมใช้งานไหม
    public bool HasAvailableSkill()
    {
        foreach (var skill in skills)
        {
            if (!skill.IsOnCooldown && !skill.IsSkillActive)
            {
                return true;
            }
        }
        return false;
    }
    public void AddSkill(BaseSkill skill)
    {
        if (skills.Count < MAX_SKILLS && !skills.Contains(skill))
        {
            skills.Add(skill);
          //  powerManager.IncreasePower(skill.GetPowerBonus());
           // playerStats.RecalculatePower(); // อัปเดตค่าพลังใหม่ทุกครั้งที่เพิ่มสกิล
            OnSkillsChanged?.Invoke();
        }
        else
        {
            Debug.Log("Cannot add more skills. Maximum limit reached or skill already exists!");
        }
    }

    public void RemoveSkill(BaseSkill skill)
    {
        skills.Remove(skill);
        OnSkillsChanged?.Invoke();  // แจ้ง UI ให้อัพเดท
    }
    public void RemoveSkillAtIndex(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            BaseSkill skillToRemove = skills[index];
            RemoveSkill(skillToRemove);
        }
    }
    public void SwapSkills(int index1, int index2)
    {
        if (index1 >= 0 && index1 < skills.Count && 
            index2 >= 0 && index2 < skills.Count)
        {
            var temp = skills[index1];
            skills[index1] = skills[index2];
            skills[index2] = temp;
            OnSkillsChanged?.Invoke();  // แจ้ง UI ให้อัพเดท
        }
    }
    
}
