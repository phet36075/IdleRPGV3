using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;


public class UIPlayerStats : MonoBehaviour
{
    public GameObject UI_Stats;
    public PlayerStats playerStats;
    public PlayerData playerData;
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
    
    [Header("Remaining Points Text")]
    public TextMeshProUGUI txtRemainPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        strengthButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Strength));
        dexterityButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Dexterity));
        vitalityButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Vitality));
        intelligenceButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Intelligence));
        agilityButton.onClick.AddListener(() => playerStats.TrySpendStatPoint(StatType.Agility));
    }

    // Update is called once per frame
    void Update()
    {
        txtStr.text = playerStats.GetStat(StatType.Strength).ToString();
        txtDex.text = playerStats.GetStat(StatType.Dexterity).ToString();
        txtVit.text = playerStats.GetStat(StatType.Vitality).ToString();
        txtInt.text = playerStats.GetStat(StatType.Intelligence).ToString();
        txtAgi.text = playerStats.GetStat(StatType.Agility).ToString();
        
        txtHealth.text = playerData.maxHealth.ToString();
        txtDamage.text = playerData.baseDamage.ToString();
        txtDefense.text = playerData.defense.ToString();
        txtCritChance.text = playerData.criticalChance.ToString();
        txtCritDamage.text = playerData.criticalDamage.ToString();
        txtArmorPenatration.text = playerData.armorPenetration.ToString();
        txtHpRegen.text = playerData.regenRate.ToString();
        txtMana.text = playerData.maxMana.ToString();
        txtManaRegen.text = playerData.manaRegenRate.ToString();
        
        txtLevel.text = playerStats.Level.ToString();
        txtLevel2.text = playerStats.Level.ToString();
        txtExp.text = playerStats.CurrentExp.ToString();

        txtRemainPoints.text = playerStats.AvailableStatPoints.ToString();

    }

    public void ToggleUI()
    {
        UI_Stats.SetActive(!UI_Stats.activeSelf);
    }
    
}
