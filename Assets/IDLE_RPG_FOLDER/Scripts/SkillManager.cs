using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<ISkill> skills = new List<ISkill>();
    
    // Start is called before the first frame update
    void Start()
    {
        skills.Add(GetComponent<Skill1>());
        skills.Add(GetComponent<Skill2>());
        skills.Add(GetComponent<Skill3>());
    }
    public ISkill GetSkill(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            return skills[index];
        }
        return null;
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
    

    public float GetSkillCooldownTime(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            return skills[index].GetCooldownTime();
        }
        return 0f;
    }
    public float GetRemainingCooldownTime(int index)
    {
        if (index >= 0 && index < skills.Count)
        {
            return skills[index].GetRemainingCooldownTime();
        }
        return 0f;
    }
    
    
}
