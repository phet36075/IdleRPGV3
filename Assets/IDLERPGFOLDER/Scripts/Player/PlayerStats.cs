using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public CharacterStats baseStats;
    
    private int currentStr;
    private int currentDex;
    private int currentVit;
    private int currentInt;
    private int currentAgi;
    
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

    public void PlusStr()
    {
        currentStr += 1;
        OnStatsChanged?.Invoke();
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

