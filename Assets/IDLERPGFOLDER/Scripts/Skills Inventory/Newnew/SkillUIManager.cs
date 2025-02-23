using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
     [SerializeField] private SkillManager skillManager;
    [SerializeField] private SkillSlotUI[] skillSlots;  // array ของ skill slots UI

    private void Start()
    {
        // ตั้งค่า index ให้แต่ละ slot
        for(int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i].SetIndex(i);
        }

        // Subscribe to events
        skillManager.OnSkillsChanged += UpdateSkillUI;
        
        // Update UI ครั้งแรก
        UpdateSkillUI();
    }

    private void UpdateSkillUI()
    {
        // อัพเดททุก slot
        for(int i = 0; i < skillSlots.Length; i++)
        {
            if (i < skillManager.Skills.Count)
            {
                // ส่ง callback สำหรับ unequip ด้วย
                skillSlots[i].SetSkill(skillManager.Skills[i], OnUnequipSkill);
            }
            else
            {
                skillSlots[i].SetSkill(null, null);
            }
        }
    }
    private void OnUnequipSkill(BaseSkill skill)
    {
        skillManager.UnequipSkill(skill);
    }
}
