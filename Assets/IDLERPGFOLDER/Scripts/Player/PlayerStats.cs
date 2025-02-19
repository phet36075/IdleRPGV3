using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterStats baseStats;
    public SkillManager skillManager;
    [SerializeField] private GameObject lvUpPrefab;
    [SerializeField] private UIPlayerStats uiPlayerStats;
    [SerializeField] private PowerChangeUI powerChangeUI; // ลาก Object UI มาที่ Inspector
    private int currentStr;
    private int currentDex;
    private int currentVit;
    private int currentInt;
    private int currentAgi;
    
    [SerializeField] private int powerPerStr = 5;  // พลังที่ได้ต่อ 1 STR
    [SerializeField] private int powerPerDex = 3;  // พลังที่ได้ต่อ 1 DEX
    [SerializeField] private int powerPerVit = 4;  // พลังที่ได้ต่อ 1 VIT
    [SerializeField] private int powerPerInt = 2;  // พลังที่ได้ต่อ 1 INT
    [SerializeField] private int powerPerAgi = 3;  // พลังที่ได้ต่อ 1 AGI
    
    private Dictionary<StatType, int> statsPowerGain = new Dictionary<StatType, int>();
    // Level and EXP properties
    private int level = 1;
    private int currentExp = 0;
    private int availableStatPoints = 0;
    private const int POINTS_PER_LEVEL = 3;
    
    public int Level => level;
    public int CurrentExp => currentExp;
    public int AvailableStatPoints => availableStatPoints;
    private int totalSpentStatPoints = 0;
    
    public static event Action<int> OnPowerChanged; // Event แจ้ง UI เมื่อพลังเปลี่ยน
    [SerializeField] private int basePower = 100; // พลังพื้นฐาน
    private int currentPower;
    
    // เพิ่ม event สำหรับแจ้งเตือนเมื่อมีการเปลี่ยนแปลงแต้มสเตตัส
    public delegate void StatPointsChangedHandler(int availablePoints);
    public event StatPointsChangedHandler OnStatPointsChanged;
    
    void Start()
    {
        currentPower = basePower;
        NotifyPowerChange();
        NotifyStatPointsChanged();
    }
    
    private void Awake()
    {
        InitializeStats();
        InitializeStatsPowerGain();
    }
    private void InitializeStatsPowerGain()
    {
        statsPowerGain[StatType.Strength] = powerPerStr;
        statsPowerGain[StatType.Dexterity] = powerPerDex;
        statsPowerGain[StatType.Vitality] = powerPerVit;
        statsPowerGain[StatType.Intelligence] = powerPerInt;
        statsPowerGain[StatType.Agility] = powerPerAgi;
    }
    private void InitializeStats()
    {
        currentStr = baseStats.baseStr;
        currentDex = baseStats.baseDex;
        currentVit = baseStats.baseVit;
        currentInt = baseStats.baseInt;
        currentAgi = baseStats.baseAgi;
    }
    // ฟังก์ชันสำหรับเพิ่มค่าพลัง เช่น เวลอัพ
    public void IncreasePower(int amount)
    {
        currentPower += amount;
        NotifyPowerChange();
        powerChangeUI.ShowPowerChange(amount); // แสดงข้อความ
    }

    // ฟังก์ชันสำหรับลดค่าพลัง
    public void DecreasePower(int amount)
    {
        currentPower = Mathf.Max(0, currentPower - amount); // ไม่ให้ติดลบ
        NotifyPowerChange();
        powerChangeUI.ShowPowerChange(-amount); // แสดงข้อความ
    }
    public void AddExperience(int amount)
    {
        currentExp += amount;
        uiPlayerStats.UpdateExpUI(currentExp);
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        int expRequired = CalculateExpForNextLevel();
        while (currentExp >= expRequired)
        {
            currentExp -= expRequired; // ลดค่า EXP ตามที่ต้องใช้
            LevelUp();
            expRequired = CalculateExpForNextLevel();
            uiPlayerStats.UpdateExpUI(currentExp);
        }
    }
    
    private void LevelUp()
    {
        GameObject spawnedEffect = Instantiate(lvUpPrefab, transform.position, lvUpPrefab.transform.rotation);
        Destroy(spawnedEffect,1f);
        level++;
        availableStatPoints += POINTS_PER_LEVEL;
        OnLevelUp?.Invoke(level);
        NotifyStatPointsChanged();
    }
    
    public int CalculateExpForNextLevel()
    {
        float K = 1.5f; // ปรับค่า K ตามความต้องการ
        return Mathf.FloorToInt(100 * level * Mathf.Log(level + 1) * K);
    }
    // เมธอดสำหรับ restat
    public void ResetAllStats()
    {
        // คำนวณค่าพลังที่ต้องลดจากสเตตัสที่เคยอัพ
        int totalPowerToReduce = 0;
        totalPowerToReduce += (currentStr - baseStats.baseStr) * powerPerStr;
        totalPowerToReduce += (currentDex - baseStats.baseDex) * powerPerDex;
        totalPowerToReduce += (currentVit - baseStats.baseVit) * powerPerVit;
        totalPowerToReduce += (currentInt - baseStats.baseInt) * powerPerInt;
        totalPowerToReduce += (currentAgi - baseStats.baseAgi) * powerPerAgi;

        // ลดค่าพลัง
        DecreasePower(totalPowerToReduce);

        // รีเซ็ตค่าสเตตัส
        currentStr = baseStats.baseStr;
        currentDex = baseStats.baseDex;
        currentVit = baseStats.baseVit;
        currentInt = baseStats.baseInt;
        currentAgi = baseStats.baseAgi;

        // คืน stat points
        availableStatPoints += totalSpentStatPoints;
        totalSpentStatPoints = 0;

        // แจ้งเตือน UI
        OnStatsChanged?.Invoke();
    }
    public bool TrySpendStatPoint(StatType statType)
    {
        if (availableStatPoints <= 0) return false;
        
        ModifyStat(statType, 1);
        availableStatPoints--;
        totalSpentStatPoints++; // เพิ่มการนับจำนวน stat points ที่ใช้ไป
        // เพิ่มค่าพลังตามสเตตัสที่อัพ
        int powerGain = statsPowerGain[statType];
        IncreasePower(powerGain);
        NotifyStatPointsChanged();
        return true;
    }
    // เมธอดสำหรับคำนวณค่าพลังที่ได้จากสเตตัสทั้งหมด
    public int CalculateTotalStatsPower()
    {
        int totalPower = 0;
        totalPower += (currentStr - baseStats.baseStr) * powerPerStr;
        totalPower += (currentDex - baseStats.baseDex) * powerPerDex;
        totalPower += (currentVit - baseStats.baseVit) * powerPerVit;
        totalPower += (currentInt - baseStats.baseInt) * powerPerInt;
        totalPower += (currentAgi - baseStats.baseAgi) * powerPerAgi;
        return totalPower;
    }

    // เมธอดสำหรับดูว่าการอัพแต่ละสเตตัสจะได้พลังเท่าไหร่
    public int GetPowerGainPerStat(StatType statType)
    {
        return statsPowerGain[statType];
    }
    // เพิ่มเมธอดสำหรับตรวจสอบจำนวน stat points ที่ใช้ไปทั้งหมด
    public int GetTotalSpentStatPoints()
    {
        return totalSpentStatPoints;
    }
    // เพิ่มเมธอดสำหรับตรวจสอบว่า stats ถูกอัพไปแล้วเท่าไหร่เทียบกับค่าเริ่มต้น
    public int GetStatIncrease(StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength:
                return currentStr - baseStats.baseStr;
            case StatType.Dexterity:
                return currentDex - baseStats.baseDex;
            case StatType.Vitality:
                return currentVit - baseStats.baseVit;
            case StatType.Intelligence:
                return currentInt - baseStats.baseInt;
            case StatType.Agility:
                return currentAgi - baseStats.baseAgi;
            default:
                return 0;
        }
    }
    // เมธอดสำหรับปรับค่า stats
    public void ModifyStat(StatType statType, int amount)
    {
        switch (statType)
        {
            case StatType.Strength:
                currentStr += amount;
                break;
            case StatType.Dexterity:
                currentDex += amount;
                break;
            case StatType.Vitality:
                currentVit += amount;
                break;
            case StatType.Intelligence:
                currentInt += amount;
                break;
            case StatType.Agility:
                currentAgi += amount;
                break;
        }
        
        // เรียก event หรือ callback เมื่อ stats มีการเปลี่ยนแปลง
        OnStatsChanged?.Invoke();
    }
    
    // Event สำหรับแจ้งเตือนเมื่อ stats เปลี่ยนแปลง
    public delegate void StatsChangedHandler();
    public event StatsChangedHandler OnStatsChanged;
    
    public delegate void LevelUpHandler(int newLevel);
    public event LevelUpHandler OnLevelUp;
    public int GetStat(StatType statType)
    {
        switch (statType)
        {
            case StatType.Strength:
                return currentStr;
            case StatType.Dexterity:
                return currentDex;
            case StatType.Vitality:
                return currentVit;
            case StatType.Intelligence:
                return currentInt;
            case StatType.Agility:
                return currentAgi;
            default:
                return 0;
        }
    }
   
    
    // เพิ่มเมธอดสำหรับแจ้งเตือนการเปลี่ยนแปลงแต้มสเตตัส
    private void NotifyStatPointsChanged()
    {
        OnStatPointsChanged?.Invoke(availableStatPoints);
    }
    // แจ้ง UI ให้แสดงค่าพลังที่เปลี่ยนแปลง
    private void NotifyPowerChange()
    {
        OnPowerChanged?.Invoke(currentPower);
    }
}

// Enum สำหรับระบุประเภทของ stat
public enum StatType
{
    Strength,
    Dexterity,
    Vitality,
    Intelligence,
    Agility
}

