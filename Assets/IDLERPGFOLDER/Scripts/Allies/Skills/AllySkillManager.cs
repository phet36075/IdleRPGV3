using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllySkillManager : MonoBehaviour
{
    [SerializeField] public List<AllyBaseSkill> skills = new List<AllyBaseSkill>();
    private const int MAX_SKILLS = 3;
    public event System.Action OnAllySkillsChanged;
    [SerializeField] private AllySkillInventoryManager allySkillInventory;
    public IReadOnlyList<AllyBaseSkill> Skills => skills;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseSkill(0);
    }
    
    public void UseSkill(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            skills[index].UseSkill();
        }
    }
    public void UnequipSkill(AllyBaseSkill skill)
    {
        if (skills.Contains(skill))
        {
            // powerManager.DecreasePower(skill.GetPowerBonus());
            // playerStats.RecalculatePower(); // อัปเดตค่าพลังใหม่ทุกครั้งที่เพิ่มสกิล
            skills.Remove(skill);
            Destroy(skill);  // ลบ component ออกจาก GameObject
            OnAllySkillsChanged?.Invoke();
        }
    }
    public bool UseNextAvailableSkill()
    {
        if (skills.Count == 0) return false;  // Return false if no skills exist

        for (int i = 0; i < skills.Count; i++)
        {
            AllyBaseSkill currentSkill = skills[i];
        
            if (!currentSkill.IsOnCooldown)
            {
                currentSkill.UseSkill();
                return true;
            }
        }

        Debug.Log("No available skills to use!");
        return false;
    }
    public void AddSkill(AllyBaseSkill skill)
    {
        if (skills.Count < MAX_SKILLS && !skills.Contains(skill))
        {
            skills.Add(skill);
            //  powerManager.IncreasePower(skill.GetPowerBonus());
            // playerStats.RecalculatePower(); // อัปเดตค่าพลังใหม่ทุกครั้งที่เพิ่มสกิล
            OnAllySkillsChanged?.Invoke();
        }
        else
        {
            Debug.Log("Cannot add more skills. Maximum limit reached or skill already exists!");
        }
    }
    
    public void RemoveSkill(AllyBaseSkill skill)
    {
        skills.Remove(skill);
        OnAllySkillsChanged?.Invoke();  // แจ้ง UI ให้อัพเดท
    }
}
