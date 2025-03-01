using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllySkillInventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button slotButton;  // เพิ่มปุ่มสำหรับคลิกดูรายละเอียด
    [SerializeField] private TextMeshProUGUI manaCostText;
    private AllySkillData allySkillData;
    private System.Action<AllySkillData> onSlotClickCallback;

    private void Start()
    {
        slotButton.onClick.AddListener(OnSlotClick);
    }

    public void Setup(AllySkillData skill, System.Action<AllySkillData> slotClickCallback)
    {
        this.allySkillData = skill;
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
        if (allySkillData != null && onSlotClickCallback != null)
        {
            onSlotClickCallback.Invoke(allySkillData);
        }
    }
}
