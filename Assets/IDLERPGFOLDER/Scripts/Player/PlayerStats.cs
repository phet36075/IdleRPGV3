using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterStats baseStats;
    
    private int currentStr;
    private int currentDex;
    private int currentVit;
    private int currentInt;
    private int currentAgi;
    
    // Level and EXP properties
    private int level = 1;
    private int currentExp = 0;
    private int availableStatPoints = 0;
    private const int POINTS_PER_LEVEL = 3;
    
    public int Level => level;
    public int CurrentExp => currentExp;
    public int AvailableStatPoints => availableStatPoints;
    private void Awake()
    {
        InitializeStats();
    }
    
    private void InitializeStats()
    {
        currentStr = baseStats.baseStr;
        currentDex = baseStats.baseDex;
        currentVit = baseStats.baseVit;
        currentInt = baseStats.baseInt;
        currentAgi = baseStats.baseAgi;
    }
    
    public void AddExperience(int amount)
    {
        currentExp += amount;
        CheckLevelUp();
    }
    
    private void CheckLevelUp()
    {
        int expRequired = CalculateExpForNextLevel();
        while (currentExp >= expRequired)
        {
            LevelUp();
            expRequired = CalculateExpForNextLevel();
        }
    }
    
    private void LevelUp()
    {
        level++;
        availableStatPoints += POINTS_PER_LEVEL;
        OnLevelUp?.Invoke(level);
    }
    
    public int CalculateExpForNextLevel()
    {
        return level * 100; // Simple formula: each level requires level * 100 EXP
    }
    
    public bool TrySpendStatPoint(StatType statType)
    {
        if (availableStatPoints <= 0) return false;
        
        ModifyStat(statType, 1);
        availableStatPoints--;
        return true;
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

