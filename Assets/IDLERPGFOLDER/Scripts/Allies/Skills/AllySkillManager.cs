using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllySkillManager : MonoBehaviour
{
    [SerializeField] public List<AllyBaseSkill> skills = new List<AllyBaseSkill>();
    private const int MAX_SKILLS = 3;
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
            //OnSkillsChanged?.Invoke();
        }
        else
        {
            Debug.Log("Cannot add more skills. Maximum limit reached or skill already exists!");
        }
    }
}
