using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillInventoryUI : MonoBehaviour
{
    [SerializeField] private SkillInventoryManager inventoryManager;
    [SerializeField] private Transform skillSlotsContainer;
    [SerializeField] private SkillInventorySlotUI slotPrefab;
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SkillDetailWindow detailWindow;
    [SerializeField] private GameObject inventoryPanel;  // Panel หลักของ inventory
    [SerializeField] private Button closeButton;  // ปุ่มปิด inventory
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private TextMeshProUGUI pageText;  // แสดงเลขหน้า
    [SerializeField] private GameObject UIContainer;
    [SerializeField] private UIContainer uiContainer;
    private List<SkillInventorySlotUI> slots = new List<SkillInventorySlotUI>();
    private bool isInventoryOpen = false;
    private void Start()
    {
        inventoryManager.OnInventoryChanged += UpdateInventoryUI;
        skillManager.OnSkillsChanged += UpdateInventoryUI;
        UpdateInventoryUI();
        closeButton.onClick.AddListener(Hide);
      //  Hide();  // ซ่อนตอนเริ่มเกม
    }
    public void Show()
    {
        uiContainer.isSkillsInventoyryOpened = true;
        uiContainer.isStatsOpened = false;
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
        detailWindow.Hide();  // ปิด detail window ด้วย
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
        foreach (var skillData in inventoryManager.GetCurrentPageSkills())
        {
            var slotUI = Instantiate(slotPrefab, skillSlotsContainer);
            slotUI.Setup(skillData, OnSkillSlotClicked);
            slots.Add(slotUI);
        }

        // อัพเดทปุ่มเปลี่ยนหน้า
        UpdatePageButtons();
        UpdatePageText();
    }

    private void UpdatePageButtons()
    {
        nextPageButton.interactable = inventoryManager.HasNextPage;
        previousPageButton.interactable = inventoryManager.HasPreviousPage;
    }

    private void OnNextPageClick()
    {
        inventoryManager.NextPage();
    }

    private void OnPreviousPageClick()
    {
        inventoryManager.PreviousPage();
    }
    private void UpdatePageText()
    {
        pageText.text = $"Page {inventoryManager.CurrentPage}/{inventoryManager.TotalPages}";
    }
    private void OnTryEquipSkill(SkillData skillData)
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
            var targetObject = skillManager.gameObject;
            
            var skillComponent = targetObject.AddComponent(skillType) as BaseSkill;
            if (skillComponent != null)
            {
                skillComponent.SetSkillData(skillData);
                skillManager.AddSkill(skillComponent);
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
    private bool IsSkillAlreadyEquipped(SkillData skillData)
    {
        foreach (var skill in skillManager.Skills)
        {
            if (skill.SkillData == skillData)
            {
                return true;
            }
        }
        return false;
    }
    private void OnSkillSlotClicked(SkillData skillData)
    {
        bool isEquipped = skillManager.Skills.Any(s => s.SkillData == skillData);
        detailWindow.Show(skillData, isEquipped);
    }
    private void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= UpdateInventoryUI;
        }
    }
}
