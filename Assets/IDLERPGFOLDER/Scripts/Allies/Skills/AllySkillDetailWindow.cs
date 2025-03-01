using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllySkillDetailWindow : MonoBehaviour
{
      [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private AllySkillManager allySkillManager;
    [SerializeField] private GameObject windowPanel;  // panel หลักของ window
    [SerializeField] private Button closeButton;  // เพิ่มปุ่มปิด
    private AllySkillData currentSkillData;

    private void Start()
    {
        equipButton.onClick.AddListener(OnEquipButtonClick);
        unequipButton.onClick.AddListener(OnUnequipButtonClick);
        closeButton.onClick.AddListener(Hide);  // เพิ่ม listener สำหรับปุ่มปิด
        Hide();  // ซ่อนหน้าต่างตอนเริ่มต้น
    }

    public void Show(AllySkillData skillData, bool isEquipped)
    {
        currentSkillData = skillData;
        
        // แสดงข้อมูลสกิล
        skillIcon.sprite = skillData.skillIcon;
        skillNameText.text = skillData.skillName;
        descriptionText.text = skillData.description;
        cooldownText.text = $"Cooldown: {skillData.cooldown}s";
        damageText.text = $"Damage: {skillData.damage}";
        manaText.text = $"Mana Cost: {skillData.manaCost}";
        // แสดง/ซ่อนปุ่มตามสถานะ
        equipButton.gameObject.SetActive(!isEquipped);
        unequipButton.gameObject.SetActive(isEquipped);

        windowPanel.SetActive(true);
    }

    public void Hide()
    {
        windowPanel.SetActive(false);
    }

    private void OnEquipButtonClick()
    {
        if (currentSkillData != null)
        {
            // เช็คจำนวนสกิลก่อน
            if (allySkillManager.Skills.Count >= 3)
            {
                Debug.Log("Skills are full! Maximum 3 skills allowed.");
                warningText.gameObject.SetActive(true);
                StartCoroutine(HideWarningSecs());
                // อาจจะเพิ่ม UI แสดงข้อความเตือนตรงนี้
                return;
            }
            warningText.gameObject.SetActive(false);
            var skillType = currentSkillData.GetSkillComponentType();
            if (skillType != null)
            {
                var targetObject = allySkillManager.gameObject;
                var skillComponent = targetObject.AddComponent(skillType) as AllyBaseSkill;
                if (skillComponent != null)
                {
                    skillComponent.SetSkillData(currentSkillData);
                    allySkillManager.AddSkill(skillComponent);
                    // อัพเดทปุ่มหลังจาก equip
                    equipButton.gameObject.SetActive(false);
                    unequipButton.gameObject.SetActive(true);
                }
            }
        }
    }

    private IEnumerator HideWarningSecs()
    {
        
        yield return new WaitForSeconds(1f);
        HideWarning();
    }
    
    public void HideWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }
    private void OnUnequipButtonClick()
    {
        if (currentSkillData != null)
        {
            var equippedSkill = allySkillManager.Skills.FirstOrDefault(s => s.AllySkillData == currentSkillData);
            if (equippedSkill != null)
            {
                allySkillManager.UnequipSkill(equippedSkill);
                // อัพเดทปุ่มหลังจาก unequip
                equipButton.gameObject.SetActive(true);
                unequipButton.gameObject.SetActive(false);
            }
        }
    }
}
