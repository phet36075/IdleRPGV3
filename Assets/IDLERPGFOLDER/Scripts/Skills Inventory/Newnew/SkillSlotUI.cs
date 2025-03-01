using UnityEngine;
using TMPro;
using UnityEngine.UI;
[System.Serializable]

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image cooldownOverlay;  // Image ที่จะใช้แสดง cooldown
    [SerializeField] private TextMeshProUGUI cooldownText;  // Text แสดงเวลา cooldown
    [SerializeField] private Button unequipButton;  // เพิ่มปุ่ม unequip
    
    private BaseSkill currentSkill;
    private int slotIndex;
    private bool isCooldown = false;
    private float cooldownTimeLeft = 0f;
    private float maxCooldown = 0f;
    private System.Action<BaseSkill> onUnequipCallback;  // callback สำหรับ unequip
    
    void Start()  // หรือใช้ OnEnable ก็ได้
    {
        unequipButton.onClick.AddListener(OnUnequipButtonClick);
    }
    public void SetSkill(BaseSkill skill, System.Action<BaseSkill> unequipCallback)
    {
        // Unsubscribe from old skill events
        if (currentSkill != null)
        {
            currentSkill.OnCooldownStart -= StartCooldown;
            currentSkill.OnCooldownEnd -= EndCooldown;
        }

        currentSkill = skill;
        onUnequipCallback = unequipCallback;

        if (skill != null)
        {
            iconImage.sprite = skill.SkillData.skillIcon;
            iconImage.gameObject.SetActive(true);
            backgroundImage.color = Color.white;
            unequipButton.gameObject.SetActive(true);  // แสดงปุ่ม unequip
            
            skill.OnCooldownStart += StartCooldown;
            skill.OnCooldownEnd += EndCooldown;

            if (skill.IsOnCooldown)
            {
                StartCooldown(skill.SkillData.cooldown);
            }
            else
            {
                cooldownOverlay.fillAmount = 0;
                cooldownText.gameObject.SetActive(false);
            }
        }
        else
        {
            iconImage.gameObject.SetActive(false);
            backgroundImage.color = Color.gray;
            cooldownOverlay.fillAmount = 0;
            cooldownText.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(false);  // ซ่อนปุ่ม unequip
        }
    }
    public void OnUnequipButtonClick()
    {
        if (currentSkill != null && onUnequipCallback != null &&!isCooldown)
        {
            onUnequipCallback.Invoke(currentSkill);
        }
    }
    public void SetIndex(int index)
    {
        slotIndex = index;
    }

    private void Update()
    {
        if (isCooldown)
        {
            cooldownTimeLeft -= Time.deltaTime;
            if (cooldownTimeLeft <= 0)
            {
                EndCooldown();
            }
            else
            {
                cooldownOverlay.fillAmount = cooldownTimeLeft / maxCooldown;
                cooldownText.text = Mathf.Ceil(cooldownTimeLeft).ToString();
            }
        }
    }

    private void StartCooldown(float duration)
    {
        isCooldown = true;
        cooldownTimeLeft = duration;
        maxCooldown = duration;
        cooldownText.gameObject.SetActive(true);
        cooldownOverlay.gameObject.SetActive(true);
    }

    private void EndCooldown()
    {
        isCooldown = false;
        cooldownOverlay.fillAmount = 0;
        cooldownText.gameObject.SetActive(false);
        cooldownOverlay.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (currentSkill != null)
        {
            currentSkill.OnCooldownStart -= StartCooldown;
            currentSkill.OnCooldownEnd -= EndCooldown;
        }
    }
}