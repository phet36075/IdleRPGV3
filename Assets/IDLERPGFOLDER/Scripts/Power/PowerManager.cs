using System;
using System.Collections;
using TMPro;
using UnityEngine;


[System.Serializable]
public static class PowerCalculator 
{
    // ค่าถ่วงน้ำหนักของแต่ละ stat
    private const float POWER_PER_HEALTH = 0.5f;
    private const float POWER_PER_HEALTH_REGEN = 1f;
    private const float POWER_PER_DAMAGE = 2f;
    private const float POWER_PER_DEFENSE = 3f;
    private const float POWER_PER_MANA = 0.3f;
    private const float POWER_PER_MANA_REGEN = 1f;
    private const float POWER_PER_CRIT = 8f;
    private const float POWER_PER_CRIT_DAMAGE = 5f;
    private const float POWER_PER_ARMOR_PEN = 3f;

    public static int CalculatePower(PlayerProperty currentStats)
    {
        float defaultHealth = 140f;
        float defaultRegen = 10f;
        float defaultMana = 110f;
        float defaultManaRegen = 12f;
        float defaultDamage = 12f;
        float defaultDefense = 6f;
        float defaultCrit = 2f;
        float defaultCritDamage = 2f;
        float defaultArmorPen = 2f;
       

        int powerFromHealth = Mathf.RoundToInt((currentStats.maxHealth - defaultHealth) * POWER_PER_HEALTH);
        int powerFromHealthRegen = Mathf.RoundToInt((currentStats.regenRate - defaultRegen) * POWER_PER_HEALTH_REGEN);
        int powerFromMana = Mathf.RoundToInt((currentStats.maxMana - defaultMana) * POWER_PER_MANA);
        int powerFromManaRegen = Mathf.RoundToInt((currentStats.manaRegenRate - defaultManaRegen) * POWER_PER_MANA_REGEN);
        int powerFromDamage = Mathf.RoundToInt((currentStats.baseDamage + currentStats.weaponDamage - defaultDamage) * POWER_PER_DAMAGE);
        int powerFromDefense = Mathf.RoundToInt((currentStats.defense - defaultDefense) * POWER_PER_DEFENSE);
        int powerFromCrit = Mathf.RoundToInt((currentStats.criticalChance - defaultCrit) * POWER_PER_CRIT);
        int powerFromCritDamage = Mathf.RoundToInt((currentStats.criticalDamage - defaultCritDamage) * POWER_PER_CRIT_DAMAGE);
        int powerFromArmorPen = Mathf.RoundToInt((currentStats.armorPenetration - defaultArmorPen) * POWER_PER_ARMOR_PEN);

        // Debug เพื่อเช็คค่าของแต่ละตัว
        
        int totalPower = powerFromHealth + powerFromMana + powerFromDamage + powerFromCrit + powerFromDefense + powerFromHealthRegen +powerFromManaRegen +powerFromCritDamage+powerFromArmorPen ;
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
    public Transform PowerStatsCanvas;
    public GameObject powerChangePrefab;
    public float fadeDuration = 1.5f; // ระยะเวลาที่ข้อความจะหายไป

    public Animator animator;
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
                    healthSlot = powerHealthSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerHealthSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots healthComponent = PowerStatsHolder.Find("PowerHealthSlot").GetComponent<PowerStatsSlots>();
                healthComponent.HandleStatsSlots("Health", oldValue, newValue);
              
                break;

            case "defense":
                Transform defenseSlot = PowerStatsHolder.Find("PowerDefenseSlot");

                if (defenseSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerDefenseSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerDefenseSlot.name = "PowerDefenseSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerDefenseSlot.transform.SetParent(PowerStatsHolder);
                    defenseSlot = powerDefenseSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerDefenseSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots defenseComponent = PowerStatsHolder.Find("PowerDefenseSlot").GetComponent<PowerStatsSlots>();
                defenseComponent.HandleStatsSlots("defense", oldValue, newValue);
                break;
            case "regenRate":
                Transform regenSlot = PowerStatsHolder.Find("PowerRegenSlot");

                if (regenSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerRegenSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerRegenSlot.name = "PowerRegenSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerRegenSlot.transform.SetParent(PowerStatsHolder);
                    regenSlot = powerRegenSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerRegenSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots regenComponent = PowerStatsHolder.Find("PowerRegenSlot").GetComponent<PowerStatsSlots>();
                regenComponent.HandleStatsSlots("Regen", oldValue, newValue);
                break;
            
            case "baseDamage":
                Transform damageSlot = PowerStatsHolder.Find("PowerDamageSlot");

                if (damageSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerDamageSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerDamageSlot.name = "PowerDamageSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerDamageSlot.transform.SetParent(PowerStatsHolder);
                    damageSlot = powerDamageSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerDamageSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots damageComponent = PowerStatsHolder.Find("PowerDamageSlot").GetComponent<PowerStatsSlots>();
                damageComponent.HandleStatsSlots("Attack", oldValue, newValue);
                
                break;
            case "maxMana":
                Transform maxMaxSlot = PowerStatsHolder.Find("PowerMaxManaSlot");

                if (maxMaxSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerMaxManaSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerMaxManaSlot.name = "PowerMaxManaSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerMaxManaSlot.transform.SetParent(PowerStatsHolder);
                    maxMaxSlot = powerMaxManaSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerMaxManaSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots maxManaComponent = PowerStatsHolder.Find("PowerMaxManaSlot").GetComponent<PowerStatsSlots>();
                maxManaComponent.HandleStatsSlots("Mana", oldValue, newValue);
                
                break;
            
            case "manaRegenRate":
                Transform manaRegenSlot = PowerStatsHolder.Find("PowerManaRegenSlot");

                if (manaRegenSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerManaRegenSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerManaRegenSlot.name = "PowerManaRegenSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerManaRegenSlot.transform.SetParent(PowerStatsHolder);
                    manaRegenSlot = powerManaRegenSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerManaRegenSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots manaRegenComponent = PowerStatsHolder.Find("PowerManaRegenSlot").GetComponent<PowerStatsSlots>();
                manaRegenComponent.HandleStatsSlots("Mana Regen", oldValue, newValue);
                
                break;
            case "armorPenetration":
                Transform armorPenetrationSlot = PowerStatsHolder.Find("PowerArmorPenetrationSlot");

                if (armorPenetrationSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerArmorPenatrationSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerArmorPenatrationSlot.name = "PowerArmorPenetrationSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerArmorPenatrationSlot.transform.SetParent(PowerStatsHolder);
                    armorPenetrationSlot = powerArmorPenatrationSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerArmorPenatrationSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots armorPenetrationComponent = PowerStatsHolder.Find("PowerArmorPenetrationSlot").GetComponent<PowerStatsSlots>();
                armorPenetrationComponent.HandleStatsSlots("Armor Pen", oldValue, newValue);
                
                break;
            case "criticalChance":
                Transform criticalChanceSlot = PowerStatsHolder.Find("PowerCriticalChanceSlot");

                if (criticalChanceSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerCriticalChanceSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerCriticalChanceSlot.name = "PowerCriticalChanceSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerCriticalChanceSlot.transform.SetParent(PowerStatsHolder);
                    criticalChanceSlot = powerCriticalChanceSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerCriticalChanceSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots criticalChanceComponent = PowerStatsHolder.Find("PowerCriticalChanceSlot").GetComponent<PowerStatsSlots>();
                criticalChanceComponent.HandleStatsSlots("Crit Chance", oldValue, newValue);
                
                break;
            case "criticalDamage":
                Transform criticalDamageSlot = PowerStatsHolder.Find("PowerCriticalDamageSlot");

                if (criticalDamageSlot == null) // ถ้าไม่มีให้สร้างใหม่
                {
                    GameObject powerCriticalDamageSlot = Instantiate(PowerStatsSlot, PowerStatsHolder.position, Quaternion.identity);
                    powerCriticalDamageSlot.name = "PowerCriticalDamageSlot"; // ตั้งชื่อเพื่อให้หาเจอ
                    powerCriticalDamageSlot.transform.SetParent(PowerStatsHolder);
                    criticalDamageSlot = powerCriticalDamageSlot.transform; // อัปเดตตัวแปร
                    Destroy(powerCriticalDamageSlot, fadeDuration); // ทำลายหลังจาก 2 วินาที
                }

                // หา slot ที่มีอยู่แล้ว และเรียกใช้ฟังก์ชันซ้ำ
                PowerStatsSlots criticalDamageComponent = PowerStatsHolder.Find("PowerCriticalDamageSlot").GetComponent<PowerStatsSlots>();
                criticalDamageComponent.HandleStatsSlots("Crit Damage", oldValue, newValue);
                
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
            case "damage":
              
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
        //powerChangeCanvas.gameObject.SetActive(false);
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
    private Coroutine myCoroutine;
    public void ShowPowerChange(int amount)
    {
        powerChangeText.text = "Power: " + (amount > 0 ? "+" : "") + amount;
        powerChangeText.color = amount < 0 ? Color.red : Color.green; // เปลี่ยนสีเป็นแดงถ้าติดลบ เขียวถ้าบวก
        
        animator.Play("PowerDisplay_Anim", 0, 0f);
       // animator.SetTrigger("DisplayTrigger");
        
      // StartCoroutine(DisplayPower());
       if (myCoroutine != null)
       {
           StopCoroutine(myCoroutine); // หยุด Coroutine เดิมก่อน
       }
       myCoroutine = StartCoroutine(FadeText()); // เริ่มใหม่
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
        yield return new WaitForSeconds(fadeDuration); // รอ 1 วิ
       powerChangeCanvas.gameObject.SetActive(false);
       
    }
    private void NotifyPowerChange()
    {
        OnPowerChanged?.Invoke(currentPower);
    }
}
