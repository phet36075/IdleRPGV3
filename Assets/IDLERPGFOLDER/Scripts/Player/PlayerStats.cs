using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Strength,
    Dexterity,
    Vitality,
    Intelligence,
    Agility
}

public class PlayerStats : MonoBehaviour
{
    #region References

    public PlayerProperty playerProperty;
    public CharacterStats baseStats;
    public SkillManager skillManager;
    [SerializeField] private GameObject lvUpPrefab;
    [SerializeField] private UIPlayerStats uiPlayerStats;
   // [SerializeField] private PowerChangeUI powerChangeUI;
    [SerializeField] private PowerManager powerManager;
    #endregion

    #region Base Stats
    private int currentStr;
    private int currentDex;
    private int currentVit;
    private int currentInt;
    private int currentAgi;
    #endregion

    #region Power Per Stat Settings
    // [SerializeField] private int powerPerStr = 5;
    // [SerializeField] private int powerPerDex = 3;
    // [SerializeField] private int powerPerVit = 4;
    // [SerializeField] private int powerPerInt = 2;
    // [SerializeField] private int powerPerAgi = 3;
    // private Dictionary<StatType, int> statsPowerGain = new Dictionary<StatType, int>();
    #endregion

    #region Level and Experience
    private int level = 1;
    private int currentExp = 0;
    private int availableStatPoints = 0;
    private const int POINTS_PER_LEVEL = 3;
    private int totalSpentStatPoints = 0;
    
    public int Level => level;
    public int CurrentExp => currentExp;
    public int AvailableStatPoints => availableStatPoints;
    #endregion

    #region Power System
    // [SerializeField] private int basePower = 100;
    // private int currentPower;
    // public static event Action<int> OnPowerChanged;
    #endregion

    #region Events
    public delegate void StatPointsChangedHandler(int availablePoints);
    public delegate void StatsChangedHandler();
    public delegate void LevelUpHandler(int newLevel);
    
    public event StatPointsChangedHandler OnStatPointsChanged;
    public event StatsChangedHandler OnStatsChanged;
    public event LevelUpHandler OnLevelUp;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeStats();
        InitializeStatsPowerGain();
    }

    void Start()
    {
       // currentPower = basePower;
        // NotifyPowerChange();
        NotifyStatPointsChanged();
    }
    #endregion

    #region Initialization Methods
    private void InitializeStatsPowerGain()
    {
        // statsPowerGain[StatType.Strength] = powerPerStr;
        // statsPowerGain[StatType.Dexterity] = powerPerDex;
        // statsPowerGain[StatType.Vitality] = powerPerVit;
        // statsPowerGain[StatType.Intelligence] = powerPerInt;
        // statsPowerGain[StatType.Agility] = powerPerAgi;
    }

    private void InitializeStats()
    {
        currentStr = baseStats.baseStr;
        currentDex = baseStats.baseDex;
        currentVit = baseStats.baseVit;
        currentInt = baseStats.baseInt;
        currentAgi = baseStats.baseAgi;
    }
    #endregion

    #region Power Management
    // public void IncreasePower(int amount)
    // {
    //     currentPower += amount;
    //     NotifyPowerChange();
    //     powerChangeUI.ShowPowerChange(amount);
    // }
    //
    // public void DecreasePower(int amount)
    // {
    //     currentPower = Mathf.Max(0, currentPower - amount);
    //     NotifyPowerChange();
    //     powerChangeUI.ShowPowerChange(-amount);
    // }
    //
    // private void NotifyPowerChange()
    // {
    //     OnPowerChanged?.Invoke(currentPower);
    // }
    #endregion

    #region Experience and Level Management
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
            currentExp -= expRequired;
            LevelUp();
            expRequired = CalculateExpForNextLevel();
            uiPlayerStats.UpdateExpUI(currentExp);
        }
    }
    
    private void LevelUp()
    {
        GameObject spawnedEffect = Instantiate(lvUpPrefab, transform.position, lvUpPrefab.transform.rotation);
        Destroy(spawnedEffect, 1f);
        level++;
        availableStatPoints += POINTS_PER_LEVEL;
        OnLevelUp?.Invoke(level);
        NotifyStatPointsChanged();
    }
    
    public int CalculateExpForNextLevel()
    {
        float K = 1.5f;
        return Mathf.FloorToInt(100 * level * Mathf.Log(level + 1) * K);
    }
    #endregion

    #region Stat Management
    public bool TrySpendStatPoint(StatType statType)
    {
        if (availableStatPoints <= 0) return false;
        
        ModifyStat(statType, 1);
        availableStatPoints--;
        totalSpentStatPoints++;
       // int powerGain = statsPowerGain[statType];
        //powerManager.IncreasePower(powerGain);
        NotifyStatPointsChanged();
        return true;
    }

    public void ResetAllStats()
    {
        // int totalPowerToReduce = CalculateTotalStatsPower();
        // powerManager.DecreasePower(totalPowerToReduce);

        currentStr = baseStats.baseStr;
        currentDex = baseStats.baseDex;
        currentVit = baseStats.baseVit;
        currentInt = baseStats.baseInt;
        currentAgi = baseStats.baseAgi;

        availableStatPoints += totalSpentStatPoints;
        totalSpentStatPoints = 0;
       // playerProperty.RestatAllProperties();
        OnStatsChanged?.Invoke();
    }

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
        
        OnStatsChanged?.Invoke();
    }
    #endregion

    #region Stat Calculations and Queries
    // public int CalculateTotalStatsPower()
    // {
    //     int totalPower = 0;
    //     totalPower += (currentStr - baseStats.baseStr) * powerPerStr;
    //     totalPower += (currentDex - baseStats.baseDex) * powerPerDex;
    //     totalPower += (currentVit - baseStats.baseVit) * powerPerVit;
    //     totalPower += (currentInt - baseStats.baseInt) * powerPerInt;
    //     totalPower += (currentAgi - baseStats.baseAgi) * powerPerAgi;
    //     return totalPower;
    // }

    // public int GetPowerGainPerStat(StatType statType)
    // {
    //     return statsPowerGain[statType];
    // }

    public int GetTotalSpentStatPoints()
    {
        return totalSpentStatPoints;
    }

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
    #endregion

    #region UI Notifications
    private void NotifyStatPointsChanged()
    {
        OnStatPointsChanged?.Invoke(availableStatPoints);
    }
    #endregion
}