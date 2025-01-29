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
                if (cooldownPercentage > 0)
                {
                    // TimeSpan timeSpan = TimeSpan.FromSeconds(remainingCooldownTime);
                    cooldownTexts[i].text = remainingCooldownTime.ToString("F1");
                    cooldownTexts[i].enabled = true;
                }
                else
                {
                    cooldownTexts[i].enabled = false;
                }
            }
           
        }
    }
}
