using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SkillInventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button slotButton;  // เพิ่มปุ่มสำหรับคลิกดูรายละเอียด
    [SerializeField] private TextMeshProUGUI manaCostText;
    private SkillData skillData;
    private System.Action<SkillData> onSlotClickCallback;

    private void Start()
    {
        slotButton.onClick.AddListener(OnSlotClick);
    }

    public void Setup(SkillData skill, System.Action<SkillData> slotClickCallback)
    {
        this.skillData = skill;
        this.onSlotClickCallback = slotClickCallback;

        if (skill != null)
        {
           // manaCostText.text = $"Mana: {skill.manaCost}";
            iconImage.sprite = skill.skillIcon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    private void OnSlotClick()
    {
        if (skillData != null && onSlotClickCallback != null)
        {
            onSlotClickCallback.Invoke(skillData);
        }
    }
    /*public void OnEquipButtonClick()
    {
        if (skillData != null && onEquipCallback != null)
        {
            onEquipCallback.Invoke(skillData);
        }
    }*/
}
