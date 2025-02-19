using System;
using System.Collections;
using TMPro;
using UnityEngine;


[System.Serializable]
public class PowerCalculator
{
    public static int CalculatePower(PlayerData stats)
    {
        // สูตรคำนวณแบบตรงไปตรงมา
        float totalPower = 0;
        
        // พลังโจมตี
        float attackPower = stats.baseDamage + stats.weaponDamage;
        totalPower += attackPower;
        
        // พลังป้องกัน
        float defensePower = stats.defense;
        totalPower += defensePower;
        
        // เพิ่มจากเลือด
        float healthPower = stats.maxHealth * 0.5f;
        totalPower += healthPower;
        
        // เพิ่มจากมานา
        float manaPower = stats.maxMana * 0.3f;
        totalPower += manaPower;
        
        return Mathf.RoundToInt(totalPower);
    }
}



public class PowerManager : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private int basePower = 100;
    [SerializeField] private PlayerStats playerStats;
    public TextMeshProUGUI powerChangeText;
    public float fadeDuration = 1.5f; // ระยะเวลาที่ข้อความจะหายไป
    
    private int currentPower;
    private int previousPower;
    public static event Action<int> OnPowerChanged;
    
    void OnEnable()
    {
        if (playerData != null)
        {
            playerData.OnStatsChanged += OnStatsChanged;
        }
    }

    void OnDisable()
    {
        if (playerData != null)
        {
            playerData.OnStatsChanged -= OnStatsChanged;
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
        currentPower = PowerCalculator.CalculatePower(playerData);
        
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
        powerChangeText.text = (amount > 0 ? "+" : "") + amount; // แสดง + ถ้าเป็นค่าบวก
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
