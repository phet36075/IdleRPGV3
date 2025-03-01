using UnityEngine;

public class AllySkillUIManager : MonoBehaviour
{
    [SerializeField] private AllySkillManager allySkillManager;
    [SerializeField] private AllySkillSlotUI[] allySkillSlots;  // array ของ skill slots UI

    private void Start()
    {
        // ตั้งค่า index ให้แต่ละ slot
        for(int i = 0; i < allySkillSlots.Length; i++)
        {
            allySkillSlots[i].SetIndex(i);
        }

        // Subscribe to events
        allySkillManager.OnAllySkillsChanged += UpdateSkillUI;
        
        // Update UI ครั้งแรก
        UpdateSkillUI();
    }

    private void UpdateSkillUI()
    {
        // อัพเดททุก slot
        for(int i = 0; i < allySkillSlots.Length; i++)
        {
            if (i < allySkillManager.Skills.Count)
            {
                // ส่ง callback สำหรับ unequip ด้วย
                allySkillSlots[i].SetSkill(allySkillManager.Skills[i], OnUnequipSkill);
            }
            else
            {
                allySkillSlots[i].SetSkill(null, null);
            }
        }
    }
    private void OnUnequipSkill(AllyBaseSkill skill)
    {
        allySkillManager.UnequipSkill(skill);
    }
}
