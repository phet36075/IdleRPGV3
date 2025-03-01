using UnityEngine;

public class SkillInventoryToggle : MonoBehaviour
{
    [SerializeField] private SkillInventoryUI inventoryUI;
    [SerializeField] private AllySkillInventoryUI AllyinventoryUI;
    [SerializeField] private KeyCode toggleKey = KeyCode.I;  // ปุ่มลัดสำหรับเปิด/ปิด

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            inventoryUI.Toggle();
        }
    }

    // สำหรับปุ่มใน UI
    public void OnShowButtonClick()
    {
        inventoryUI.Show();
    }

    public void OnShowAllyButtonClick()
    {
        AllyinventoryUI.Show();
    }
}
