using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllySkillInventoryUI : MonoBehaviour
{
    [SerializeField] private AllySkillInventoryManager allyInventoryManager;
    [SerializeField] private Transform skillSlotsContainer;
    [SerializeField] private AllySkillInventorySlotUI allySlotPrefab;
    [SerializeField] private AllySkillManager allySkillManager;
    [SerializeField] private AllySkillDetailWindow allyDetailWindow;
    [SerializeField] private GameObject inventoryPanel;  // Panel หลักของ inventory
    [SerializeField] private Button closeButton;  // ปุ่มปิด inventory
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private TextMeshProUGUI pageText;  // แสดงเลขหน้า
    [SerializeField] private GameObject UIContainer;
    [SerializeField] private UIContainer uiContainer;
    private List<AllySkillInventorySlotUI> slots = new List<AllySkillInventorySlotUI>();
    
    private bool isInventoryOpen = false;
    private void Start()
    {
        allyInventoryManager.OnAllyInventoryChanged += UpdateInventoryUI;
        allySkillManager.OnAllySkillsChanged += UpdateInventoryUI;
        UpdateInventoryUI();
        closeButton.onClick.AddListener(Hide);
      //  Hide();  // ซ่อนตอนเริ่มเกม
    }
    public void Show()
    {
        uiContainer.isSkillsInventoyryOpened = false;
        uiContainer.isStatsOpened = false;
        uiContainer.isAllySkillsInventoyryOpened = true;
        UIContainer.SetActive(true);
        
        
       // inventoryPanel.SetActive(true);
       // isInventoryOpen = true;
        UpdateInventoryUI();
    }

    public void Hide()
    {
        uiContainer.isSkillsInventoyryOpened = false;
        inventoryPanel.SetActive(false);
        isInventoryOpen = false;
        allyDetailWindow.Hide();  // ปิด detail window ด้วย
    }

    public void Toggle()
    {
        if (isInventoryOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    private void UpdateInventoryUI()
    {
        // ลบ slots เก่า
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();

        // สร้าง slots ใหม่จากสกิลในหน้าปัจจุบัน
        foreach (var skillData in allyInventoryManager.GetCurrentPageSkills())
        {
            var slotUI = Instantiate(allySlotPrefab, skillSlotsContainer);
            slotUI.Setup(skillData, OnSkillSlotClicked);
            slots.Add(slotUI);
        }

        // อัพเดทปุ่มเปลี่ยนหน้า
        UpdatePageButtons();
        UpdatePageText();
    }

    private void UpdatePageButtons()
    {
        nextPageButton.interactable = allyInventoryManager.HasNextPage;
        previousPageButton.interactable = allyInventoryManager.HasPreviousPage;
    }

    private void OnNextPageClick()
    {
        allyInventoryManager.NextPage();
    }

    private void OnPreviousPageClick()
    {
        allyInventoryManager.PreviousPage();
    }
    private void UpdatePageText()
    {
        pageText.text = $"Page {allyInventoryManager.CurrentPage}/{allyInventoryManager.TotalPages}";
    }
    private void OnTryEquipSkill(AllySkillData skillData)
    {
        // เช็คว่าสกิลนี้ถูก equip อยู่แล้วหรือไม่
        if (IsSkillAlreadyEquipped(skillData))
        {
            Debug.Log($"Skill {skillData.skillName} is already equipped!");
            return;
        }

        var skillType = skillData.GetSkillComponentType();
        if (skillType != null)
        {
            var targetObject = allySkillManager.gameObject;
            
            var skillComponent = targetObject.AddComponent(skillType) as AllyBaseSkill;
            if (skillComponent != null)
            {
                skillComponent.SetSkillData(skillData);
                allySkillManager.AddSkill(skillComponent);
                Debug.Log($"Equipped skill: {skillData.skillName}");
            }
            else
            {
                Debug.LogError($"Failed to create skill component of type: {skillType}");
            }
        }
        else
        {
            Debug.LogError($"Invalid skill component type for skill: {skillData.skillName}");
        }
    }

    // เพิ่ม method ใหม่สำหรับเช็คว่าสกิลถูก equip แล้วหรือยัง
    private bool IsSkillAlreadyEquipped(AllySkillData skillData)
    {
        foreach (var skill in allySkillManager.Skills)
        {
            if (skill.AllySkillData == skillData)
            {
                return true;
            }
        }
        return false;
    }
    private void OnSkillSlotClicked(AllySkillData skillData)
    {
        bool isEquipped = allySkillManager.Skills.Any(s => s.AllySkillData == skillData);
        allyDetailWindow.Show(skillData, isEquipped);
    }
    private void OnDestroy()
    {
        if (allyInventoryManager != null)
        {
            allyInventoryManager.OnAllyInventoryChanged -= UpdateInventoryUI;
        }
    }
}
