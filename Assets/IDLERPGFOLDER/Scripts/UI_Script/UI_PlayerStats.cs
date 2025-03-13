using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;


public class UIPlayerStats : MonoBehaviour
{
    public GameObject UI_Stats;
    public PlayerStats playerStats;
    public PlayerProperty playerProperty;
    public UIContainer uiContainer;
    [SerializeField] private GameObject UIContainer;
    [Header("Button")]
    public Button strengthButton; // ประกาศตัวแปรสำหรับปุ่ม
    public Button dexterityButton;
    public Button vitalityButton;
    public Button intelligenceButton;
    public Button agilityButton;
    
    [Header("Stats Text")]
    public TextMeshProUGUI txtStr;
    public TextMeshProUGUI txtDex;
    public TextMeshProUGUI txtVit;
    public TextMeshProUGUI txtInt;
    public TextMeshProUGUI txtAgi;
    
    public TextMeshProUGUI txtStrExtra;
    public TextMeshProUGUI txtDexExtra;
    public TextMeshProUGUI txtVitExtra;
    public TextMeshProUGUI txtIntExtra;
    public TextMeshProUGUI txtAgiExtra;
    [Header("Property Text")]
    public TextMeshProUGUI txtHealth;
    public TextMeshProUGUI txtDamage;
    public TextMeshProUGUI txtDefense;
    public TextMeshProUGUI txtCritChance;
    public TextMeshProUGUI txtCritDamage;
    public TextMeshProUGUI txtArmorPenatration;
    public TextMeshProUGUI txtHpRegen;
    public TextMeshProUGUI txtMana;
    public TextMeshProUGUI txtManaRegen;
    [Header("Level Text")]
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtLevel2;
    public TextMeshProUGUI txtExp;
    public TextMeshProUGUI powerText;
    [Header("Remaining Points Text")]
    public TextMeshProUGUI txtRemainPoints;

    private bool isStatsOpen;
    [Header("Exp Bar")] public Slider expBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        strengthButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Strength));
        dexterityButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Dexterity));
        vitalityButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Vitality));
        intelligenceButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Intelligence));
        agilityButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Agility));
        
        
        expBar.maxValue = playerStats.CalculateExpForNextLevel();
        expBar.value = playerStats.CurrentExp;
        if (playerStats != null)
        {
            // Subscribe to stat points change event
            playerStats.OnStatPointsChanged += UpdateButtonVisibility;
            
            // Update UI ครั้งแรก
            UpdateExpUI(playerStats.CurrentExp);
            UpdateButtonVisibility(playerStats.AvailableStatPoints);
        }

       // StartCoroutine(RegenerateMana());
    }
    
    // New method to update button visibility based on available points
    private void UpdateButtonVisibility(int availablePoints)
    {
        bool showButtons = availablePoints > 0;
        
        strengthButton.gameObject.SetActive(showButtons);
        dexterityButton.gameObject.SetActive(showButtons);
        vitalityButton.gameObject.SetActive(showButtons);
        intelligenceButton.gameObject.SetActive(showButtons);
        agilityButton.gameObject.SetActive(showButtons);
    }
    
    public void UpdateExpUI(float currentExp)
    {
        // อัพเดทแถบ mana
        //manaBarFill.fillAmount = currentMana / playerManager.GetMaxMana();
        // อัพเดทตัวเลข mana
      
        expBar.maxValue = playerStats.CalculateExpForNextLevel();
        StartCoroutine(SmoothExpBar());
       
    }
    IEnumerator SmoothExpBar()
    {
        float elapsedTime = 0f;
        float duration = 0.2f; // ระยะเวลาที่ต้องการให้การลดลงของแถบลื่นไหล
        float startValue = expBar.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            expBar.value = Mathf.Lerp(startValue, playerStats.CurrentExp, elapsedTime / duration);
            yield return null;
        }

        expBar.value = playerStats.CurrentExp;
      
    }
    // Update is called once per frame
    void Update()
    {
        txtStr.text = playerStats.GetStat(StatType.Strength).ToString();
        txtDex.text = playerStats.GetStat(StatType.Dexterity).ToString();
        txtVit.text = playerStats.GetStat(StatType.Vitality).ToString();
        txtInt.text = playerStats.GetStat(StatType.Intelligence).ToString();
        txtAgi.text = playerStats.GetStat(StatType.Agility).ToString();
        
        
        
        txtHealth.text = playerProperty.maxHealth.ToString();
        txtDamage.text = playerProperty.baseDamage.ToString();
        txtDefense.text = playerProperty.defense.ToString();
        txtCritChance.text = playerProperty.criticalChance.ToString();
        txtCritDamage.text = playerProperty.criticalDamage.ToString();
        txtArmorPenatration.text = playerProperty.armorPenetration.ToString();
        txtHpRegen.text = playerProperty.regenRate.ToString();
        txtMana.text = playerProperty.maxMana.ToString();
        txtManaRegen.text = playerProperty.manaRegenRate.ToString();
        
        txtLevel.text = playerStats.Level.ToString();
        txtLevel2.text = playerStats.Level.ToString();
        txtExp.text = playerStats.CurrentExp.ToString()  + "/ " + playerStats.CalculateExpForNextLevel().ToString();

        txtRemainPoints.text = playerStats.AvailableStatPoints.ToString();
    }
   
    public void ToggleUI()
    {
        UI_Stats.SetActive(!UI_Stats.activeSelf);
    }
    
    public void Show()
    {
        uiContainer.isStatsOpened = true;
        uiContainer.isSkillsInventoyryOpened = false;
        UIContainer.SetActive(true);
        
        
       // UI_Stats.SetActive(true);
      //  isStatsOpen = true;
        //UpdateInventoryUI();
    }

    public void Hide()
    {
       
       // uiContainer.isStatsOpened = false;
      //  UI_Stats.SetActive(false);
        //isStatsOpen = false;
     
    }

    public void Toggle()
    {
        if (isStatsOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    
    
    
    private void OnEnable()
    {
        PowerManager.OnPowerChanged += UpdatePowerText;
        if (playerStats != null)
        {
            playerStats.OnStatPointsChanged += UpdateButtonVisibility;
            playerStats.OnStatsChanged += HandleStatsChanged;
        }
    }

    private void OnDisable()
    {
        PowerManager.OnPowerChanged -= UpdatePowerText;
        if (playerStats != null)
        {
            playerStats.OnStatPointsChanged -= UpdateButtonVisibility;
            playerStats.OnStatsChanged -= HandleStatsChanged;
        }
    }
    
    // Added method to handle stats reset
    private void HandleStatsChanged()
    {
        // Update button visibility after stats change (including reset)
        UpdateButtonVisibility(playerStats.AvailableStatPoints);
    }

    private void UpdatePowerText(int newPower)
    {
        powerText.text = "Power: " + newPower;
    }
}