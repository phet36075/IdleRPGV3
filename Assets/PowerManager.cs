using System;
using System.Collections;
using TMPro;
using UnityEngine;


[System.Serializable]
public static class PowerCalculator 
{
    // ค่าถ่วงน้ำหนักของแต่ละ stat
    private const float POWER_PER_HEALTH = 0.5f;
    private const float POWER_PER_DAMAGE = 2f;
    private const float POWER_PER_DEFENSE = 3f;
    private const float POWER_PER_MANA = 0.3f;
    private const float POWER_PER_CRIT = 5f;

    public static int CalculatePower(PlayerProperty currentStats)
    {
        float defaultHealth = 140f;
        float defaultMana = 100f;
        float defaultDamage = 10f;
        float defaultDefense = 5f;
        float defaultCrit = 0.05f;

        int powerFromHealth = Mathf.RoundToInt((currentStats.maxHealth - defaultHealth) * POWER_PER_HEALTH);
        int powerFromMana = Mathf.RoundToInt((currentStats.maxMana - defaultMana) * POWER_PER_MANA);
        int powerFromDamage = Mathf.RoundToInt((currentStats.baseDamage + currentStats.weaponDamage - defaultDamage) *
                                               POWER_PER_DAMAGE);
        //int powerFormDefense = Mathf.RoundToInt((currentStats.defense - defaultDefense) * POWER_PER_DEFENSE);
        int powerFromCrit = Mathf.RoundToInt((currentStats.criticalChance - defaultCrit) * POWER_PER_CRIT);

        // Debug เพื่อเช็คค่าของแต่ละตัว
        Debug.Log($"Power from Health: {powerFromHealth}");
        Debug.Log($"Power from Mana: {powerFromMana}");
        Debug.Log($"Power from Damage: {powerFromDamage}");
        Debug.Log($"Power from Crit: {powerFromCrit}");
        //Debug.Log($"Power from Defense: {powerFormDefense}");
        int totalPower = powerFromHealth + powerFromMana + powerFromDamage + powerFromCrit;
        return totalPower;
    }
}

public class PowerManager : MonoBehaviour
{
   // [SerializeField] private PlayerData playerData;
    [SerializeField] private int basePower = 100;
    [SerializeField] private PlayerProperty playerProperty;
    public TextMeshProUGUI powerChangeText;
    public float fadeDuration = 1.5f; // ระยะเวลาที่ข้อความจะหายไป
    
    private int currentPower;
    private int previousPower;
    public static event Action<int> OnPowerChanged;
    
    void OnEnable()
    {
        if (playerProperty != null)
        {
            playerProperty.OnStatsChanged += OnStatsChanged;
        }
    }

    void OnDisable()
    {
        if (playerProperty != null)
        {
            playerProperty.OnStatsChanged -= OnStatsChanged;
        }
    }
    void Start()
    {
        // currentPower = basePower;
        // NotifyPowerChange();
        UpdatePowerFromStats();
        powerChangeText.alpha = 0; // ซ่อนข้อความตอนเริ่ม
       
    }
    private void OnStatsChanged()
    {
        UpdatePowerFromStats();
    }
    public void UpdatePowerFromStats()
    {
        previousPower = currentPower;
        currentPower = PowerCalculator.CalculatePower(playerProperty);
        
        int powerDifference = currentPower - previousPower;
        if (powerDifference != 0)
        {
            NotifyPowerChange();
            ShowPowerChange(powerDifference);
        }
    }
    public void IncreasePower(int amount)
    {
        currentPower += amount;
        NotifyPowerChange();
        ShowPowerChange(amount);
    }

    public void DecreasePower(int amount)
    {
        currentPower = Mathf.Max(0, currentPower - amount);
        NotifyPowerChange();
        ShowPowerChange(-amount);
    }
    
    public void ShowPowerChange(int amount)
    {
        powerChangeText.text = "Power: " + (amount > 0 ? "+" : "") + amount;
        powerChangeText.color = amount < 0 ? Color.red : Color.green; // เปลี่ยนสีเป็นแดงถ้าติดลบ เขียวถ้าบวก
        StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        powerChangeText.alpha = 1; // แสดงข้อความ
        yield return new WaitForSeconds(1f); // รอ 1 วิ
       
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            powerChangeText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration); // ค่อย ๆ จางลง
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        powerChangeText.alpha = 0; // ซ่อนเมื่อจบ
    }
    private void NotifyPowerChange()
    {
        OnPowerChanged?.Invoke(currentPower);
    }
}
