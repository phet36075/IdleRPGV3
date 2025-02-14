using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// SkillManager สำหรับจัดการสกิลทั้งหมด
public class SkillManager : MonoBehaviour
{
    [SerializeField] private List<BaseSkill> skills = new List<BaseSkill>();
// Event สำหรับแจ้งเมื่อมีการเปลี่ยนแปลง skills
    public event System.Action OnSkillsChanged;
    [SerializeField] private SkillInventoryManager skillInventory;
    [SerializeField] private SkillData skillsToUnlock,skilltoUnlock2;  // ลาก SkillData ที่จะปลดล็อคใส่ใน inspector
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
    public void UseNextAvailableSkill()
    {
        if (skills.Count == 0) return;

        // เริ่มเช็คจากตำแหน่งปัจจุบัน
        int startIndex = currentSkillIndex;
        
        do
        {
            BaseSkill currentSkill = skills[currentSkillIndex];
            
            // ถ้าสกิลไม่ติด cooldown และไม่กำลังใช้งานอยู่
            if (!currentSkill.IsOnCooldown /* && !currentSkill.IsSkillActive*/)
            {
                currentSkill.UseSkill();
                // เลื่อนไปตำแหน่งถัดไป
                currentSkillIndex = (currentSkillIndex + 1) % skills.Count;
                return;
            }
            
            // เลื่อนไปตำแหน่งถัดไปเพื่อเช็คสกิลต่อไป
            currentSkillIndex = (currentSkillIndex + 1) % skills.Count;
            
        } while (currentSkillIndex != startIndex);  // วนจนกว่าจะครบรอบ

        Debug.Log("No available skills to use!");
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
        if (!skills.Contains(skill))
        {
            skills.Add(skill);
            OnSkillsChanged?.Invoke();  // แจ้ง UI ให้อัพเดท
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
