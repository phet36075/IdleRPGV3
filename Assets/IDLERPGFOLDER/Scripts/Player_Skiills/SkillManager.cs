using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<ISkill> skills;
    public Animator playerAnimator; // ลาก Animator ของ Player มาใส่ใน Inspector
  
    // Start is called before the first frame update
   
    
    private void Start()
    {
        foreach (var skill in skills)
        {
            skill.SetAnimator(playerAnimator); // ส่ง Animator ไปที่สกิล
        }
    }
    public ISkill GetSkill(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            return skills[index];
        }
        return null;
    }
    // เพิ่มเมธอดสำหรับเพิ่มสกิลใหม่
    public int maxSkills = 4; // กำหนดจำนวนสกิลสูงสุด

    public void AddSkill(ISkill newSkill)
    {
        if (newSkill != null && skills.Count < maxSkills)
        {
            newSkill.SetAnimator(playerAnimator);
            skills.Add(newSkill);
            Debug.Log($"Added new skill. Total skills: {skills.Count}");
        }
        else
        {
            Debug.Log("Cannot add more skills. Maximum limit reached or skill is null.");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSkill(0); // ใช้สกิลแรก
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
           UseSkill(1); // ใช้สกิลที่สอง
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            UseSkill(2);
        }else if (Input.GetKeyDown(KeyCode.Y))
        {
            // สร้างสกิลใหม่และเพิ่มเข้าไป
            ISkill newSkill = GetComponent<Skill3>();
            AddSkill(newSkill);
        }
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
        for (int i = 0; i < skills.Count; i++)
        {
            if (!skills[i].IsOnCooldown())
            {
                skills[i].UseSkill();
                return true;
            }
        }
        return false;
    }
    
    public float GetSkillCooldownPercentage(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            return skills[index].GetCooldownPercentage();
        }
        return 0f;
    }
    

   /* public float GetSkillCooldownTime(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            return skills[index].GetCooldownTime();
        }
        return 0f;
    }*/
    public float GetRemainingCooldownTime(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            return skills[index].GetRemainingCooldownTime();
        }
        return 0f;
    }
    
    
}
