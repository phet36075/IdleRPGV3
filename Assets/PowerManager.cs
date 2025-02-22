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
        int powerFromDamage = Mathf.RoundToInt((currentStats.baseDamage + currentStats.weaponDamage - defaultDamage) * POWER_PER_DAMAGE);
        int powerFromDefense = Mathf.RoundToInt((currentStats.defense - defaultDefense) * POWER_PER_DEFENSE);
        //int powerFormDefense = Mathf.RoundToInt((currentStats.defense - defaultDefense) * POWER_PER_DEFENSE);
        int powerFromCrit = Mathf.RoundToInt((currentStats.criticalChance - defaultCrit) * POWER_PER_CRIT);

        // Debug เพื่อเช็คค่าของแต่ละตัว
        
        int totalPower = powerFromHealth + powerFromMana + powerFromDamage + powerFromCrit+powerFromDefense;
        return totalPower;
    }
}

public class PowerManager : MonoBehaviour
{
   // [SerializeField] private PlayerData playerData;
    [SerializeField] private int basePower = 100;
    [SerializeField] private PlayerProperty playerProperty;
    public GameObject powerChangeCanvas;
    
    public TextMeshProUGUI powerChangeText;
    public TextMeshProUGUI maxHealthChange;
    public GameObject PowerStatsSlot;
    public Transform PowerStatsHolder;
    
    public float fadeDuration = 1.5f; // ระยะเวลาที่ข้อความจะหายไป
    
    private int currentPower;
    private int previousPower;
    public static event Action<int> OnPowerChanged;
    
    void OnEnable()
    {
        if (playerProperty != null)
        {
            playerProperty.OnStatsChanged += OnStatsChanged;
            playerProperty.OnSpecificStatChanged += HandleSpecificStatChanged;
        }
    }

    void OnDisable()
    {
        if (playerProperty != null)
        {
            playerProperty.OnStatsChanged -= OnStatsChanged;
            playerProperty.OnSpecificStatChanged -= HandleSpecificStatChanged;
        }
    }
    private void HandleSpecificStatChanged(string statName, float oldValue, float newValue)
    {
        // แสดงการแจ้งเตือนหรือ Effect พิเศษเมื่อสถิติบางตัวเปลี่ยนแปลง
        Debug.Log($"Stat changed: {statName} from {oldValue} to {newValue}");

        switch (statName)
        {
            case "maxHealth":
                Transform healthSlot = PowerStatsHolder.Find("PowerHealthSlot");

                if (healthSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerHealthSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerHealthSlot.name = "PowerHealthSlot"; // ตั้งชื่อเพื่อให้ค้นหาได้
                    powerHealthSlot.transform.SetParent(PowerStatsHolder); // กำหนด Parent
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots healthComponent = PowerStatsHolder.Find("PowerHealthSlot").GetComponent<PowerStatsSlots>();
                healthComponent.HandleStatsSlots("maxHealth", oldValue, newValue);
                break;

            case "defense":
                Transform defenseSlot = PowerStatsHolder.Find("PowerDefenseSlot");

                if (defenseSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerDefenseSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerDefenseSlot.name = "PowerDefenseSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerDefenseSlot.transform.SetParent(PowerStatsHolder);
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots defenseComponent = PowerStatsHolder.Find("PowerDefenseSlot").GetComponent<PowerStatsSlots>();
                defenseComponent.HandleStatsSlots("defense", oldValue, newValue);
                break;
        }

        // ตัวอย่าง: แสดงการแจ้งเตือนเมื่อค่า HP สูงสุดเพิ่มขึ้น
        // if (statName == "maxHealth" && newValue > oldValue)
        // {
        //       ShowStatIncreaseEffect("maxHealth", oldValue, newValue);
        // }
    }
    private void ShowStatIncreaseEffect(string statName, float oldValue, float newValue)
    {

        switch (statName)
        {
            case "maxHealth":
               // maxHealthChange.text = "Health " + oldValue + "       " + newValue;
                // อาจจะเพิ่ม animation สีเขียวที่ตัวเลข
                break;
        }

       
        // if (statName == "maxHealth" && newValue < oldValue)
        // {
        //     maxHealthChange.text = "Health " + newValue + " " + oldValue;
        // }
       
        // แสดง animation หรือ visual effect เพื่อเน้นย้ำการเปลี่ยนแปลงของสถิติ
        // เช่น การส่องสว่างของไอคอนเกราะเมื่อค่า defense เพิ่มขึ้น
    }
    void Start()
    {
        // currentPower = basePower;
        // NotifyPowerChange();
        UpdatePowerFromStats();
        powerChangeCanvas.gameObject.SetActive(false);
        //powerChangeText.alpha = 0; // ซ่อนข้อความตอนเริ่ม
       
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
    private Coroutine myCoroutine;
    public void ShowPowerChange(int amount)
    {
        powerChangeText.text = "Power: " + (amount > 0 ? "+" : "") + amount;
        powerChangeText.color = amount < 0 ? Color.red : Color.green; // เปลี่ยนสีเป็นแดงถ้าติดลบ เขียวถ้าบวก
        
        
      // StartCoroutine(DisplayPower());
       if (myCoroutine != null)
       {
           StopCoroutine(myCoroutine); // หยุด Coroutine เดิมก่อน
       }
       myCoroutine = StartCoroutine(DisplayPower()); // เริ่มใหม่
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
    
    private IEnumerator DisplayPower()
    {
        powerChangeCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f); // รอ 1 วิ
       powerChangeCanvas.gameObject.SetActive(false);
       
    }
    private void NotifyPowerChange()
    {
        OnPowerChanged?.Invoke(currentPower);
    }
}
