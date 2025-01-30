using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class CooldownUI : MonoBehaviour
{
    public SkillManager skillManager;
    public Image[] skillIcons;
    public Image[] cooldownOverlays;
   public TextMeshProUGUI[] cooldownTexts;
   
    private void Update()
    {
        
        for (int i = 0; i < skillIcons.Length; i++)
        {
            ISkill skill = skillManager.GetSkill(i);
            if (skill != null)
            {
                float cooldownPercentage = skillManager.GetSkillCooldownPercentage(i);
                float remainingCooldownTime = skillManager.GetRemainingCooldownTime(i);

                // อัพเดทเงา cooldown
                cooldownOverlays[i].fillAmount = cooldownPercentage;

                // อัพเดทเวลา cooldown
                if (cooldownPercentage > 0.01f && cooldownPercentage < 1)
                {
                   
                    cooldownTexts[i].text = Mathf.Ceil(remainingCooldownTime).ToString();
                    cooldownTexts[i].enabled = true;
                    cooldownOverlays[i].enabled = true; 
                }
                else if(cooldownPercentage == 1f)
                {
                    cooldownTexts[i].enabled = false;
                    cooldownOverlays[i].enabled = false; // ปิด overlay เมื่อสกิลพร้อมใช้งาน
                }
            }
            else
            {
                // กรณีไม่มีสกิลในตำแหน่งนี้ ปิด UI
                cooldownOverlays[i].enabled = false;
                cooldownTexts[i].enabled = false;
            }
        }
    }
}
