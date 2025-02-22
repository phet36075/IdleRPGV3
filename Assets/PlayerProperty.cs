using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : MonoBehaviour
{
    public PlayerData playerData; // อ้างอิงไปยัง ScriptableObject เพื่อใช้เป็นค่าเริ่มต้น
    public event Action OnStatsChanged;
    public event Action<string, float, float> OnSpecificStatChanged; // เพิ่ม event ใหม่ที่ส่งชื่อสถิติ ค่าเก่า และค่าใหม่
    
    private bool statsChangedSinceLastFrame = false;
    private Dictionary<string, float> previousStats = new Dictionary<string, float>();
    private Dictionary<string, float> changedStats = new Dictionary<string, float>();
    
   [Header("----Health----")]
    [SerializeField] private float _maxHealth = 100f;
    public float maxHealth
    {
        get => _maxHealth;
        set
        {
            if (_maxHealth != value)
            {
                float oldValue = _maxHealth;
                _maxHealth = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("maxHealth", oldValue, value);
                changedStats["maxHealth"] = value;
            }
        }
    }

    [Header("----Mana----")]
    [SerializeField] private float _maxMana;
    public float maxMana
    {
        get => _maxMana;
        set
        {
            if (_maxMana != value)
            {
                float oldValue = _maxMana;
                _maxMana = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("maxMana", oldValue, value);
                changedStats["maxMana"] = value;
            }
        }
    }

    [Header("----Damage----")]
    [SerializeField] private float _baseDamage = 10f;
    public float baseDamage
    {
        get => _baseDamage;
        set
        {
            if (_baseDamage != value)
            {
                float oldValue = _baseDamage;
                _baseDamage = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("baseDamage", oldValue, value);
                changedStats["baseDamage"] = value;
            }
        }
    }

    [SerializeField] private float _weaponDamage = 10f;
    public float weaponDamage
    {
        get => _weaponDamage;
        set
        {
            if (_weaponDamage != value)
            {
                float oldValue = _weaponDamage;
                _weaponDamage = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("weaponDamage", oldValue, value);
                changedStats["weaponDamage"] = value;
            }
        }
    }

    [Header("----Critical----")]
    [SerializeField] private float _criticalChance = 0.05f;
    public float criticalChance
    {
        get => _criticalChance;
        set
        {
            if (_criticalChance != value)
            {
                float oldValue = _criticalChance;
                _criticalChance = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("criticalChance", oldValue, value);
                changedStats["criticalChance"] = value;
            }
        }
    }

    [SerializeField] private float _criticalDamage = 2f;
    public float criticalDamage
    {
        get => _criticalDamage;
        set
        {
            if (_criticalDamage != value)
            {
                float oldValue = _criticalDamage;
                _criticalDamage = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("criticalDamage", oldValue, value);
                changedStats["criticalDamage"] = value;
            }
        }
    }

    [Header("----Defense----")]
    [SerializeField] private float _defense = 5f;
    public float defense
    {
        get => _defense;
        set
        {
            if (_defense != value)
            {
                float oldValue = _defense;
                _defense = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("defense", oldValue, value);
                changedStats["defense"] = value;
            }
        }
    }

    [Header("----Armor Penetration----")]
    [SerializeField] private float _armorPenetration = 0f;
    public float armorPenetration
    {
        get => _armorPenetration;
        set
        {
            if (_armorPenetration != value)
            {
                float oldValue = _armorPenetration;
                _armorPenetration = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("armorPenetration", oldValue, value);
                changedStats["armorPenetration"] = value;
            }
        }
    }

    [Header("----Regen Rate----")]
    [SerializeField] private float _regenRate = 1f;
    public float regenRate
    {
        get => _regenRate;
        set
        {
            if (_regenRate != value)
            {
                float oldValue = _regenRate;
                _regenRate = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("regenRate", oldValue, value);
                changedStats["regenRate"] = value;
            }
        }
    }

    [SerializeField] private float _manaRegenRate = 20f;
    public float manaRegenRate
    {
        get => _manaRegenRate;
        set
        {
            if (_manaRegenRate != value)
            {
                float oldValue = _manaRegenRate;
                _manaRegenRate = value;
                statsChangedSinceLastFrame = true;
                OnSpecificStatChanged?.Invoke("manaRegenRate", oldValue, value);
                changedStats["manaRegenRate"] = value;
            }
        }
    }
    void Update()
    {
        if (statsChangedSinceLastFrame)
        {
            statsChangedSinceLastFrame = false;
            OnStatsChanged?.Invoke();
            
            // อัปเดตค่า previous stats เป็นค่าปัจจุบัน
            foreach (var stat in changedStats)
            {
                previousStats[stat.Key] = stat.Value;
            }
            
            // เคลียร์รายการสถิติที่เปลี่ยนแปลงหลังจากประมวลผลแล้ว
            changedStats.Clear();
        }
    }
    
    [Header("----Damage Variation---")]
    public float damageVariation = 0.2f; // 20% variation
    
    [Header("Elemental")]
    public ElementType elementType = ElementType.None;  // ธาตุเริ่มต้น

    
    
    #region UpgradeCost

    [Header("----UpgradeCost----")]
    public int WeaponupgradeCost = 100;
    public int Weaponlevel = 1;
    
    public int healthUpgradeCost = 100;
    public int healthLevel = 1;
    
    public int regenRateCost = 100;
    public int regenRateLevel = 1;
    
    public int criticalRateCost = 100;
    public int criticalRateLevel = 1;
    
    public int defenseCost = 100;
    public int defenseLevel = 1;

    public int armorPenetrationCost = 100;
    public int armorPenetrationLevel = 1;
    
    public int criticalDamageCost = 100;
    public int criticalDamageLevel = 1;

    #endregion
   
    
    // ค่าปัจจุบันของผู้เล่น (แยกต่างหากจากค่าสถานะ)
    private float _currentHealth;
    public float currentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnStatsChanged?.Invoke();
        }
    }
    
    private float _currentMana;
    public float currentMana
    {
        get => _currentMana;
        set
        {
            _currentMana = Mathf.Clamp(value, 0, maxMana);
            OnStatsChanged?.Invoke();
        }
    }

    private void Start()
    {
        InitializeFromData();
    }

    // เรียกใช้เมื่อเริ่มเกมหรือเมื่อต้องการรีเซ็ตสถานะ
    public void InitializeFromData()
    {
        if (playerData != null)
        {
            // คัดลอกค่าจาก ScriptableObject
            maxHealth = playerData.maxHealth;
            maxMana = playerData.maxMana;
            baseDamage = playerData.baseDamage;
            weaponDamage = playerData.weaponDamage;
            criticalChance = playerData.criticalChance;
            criticalDamage = playerData.criticalDamage;
            defense = playerData.defense;
            armorPenetration = playerData.armorPenetration;
            regenRate = playerData.regenRate;
            manaRegenRate = playerData.manaRegenRate;
            damageVariation = playerData.damageVariation;
            elementType = playerData.elementType;
            
            // คัดลอกค่าสำหรับการอัพเกรด
            WeaponupgradeCost = playerData.WeaponupgradeCost;
            Weaponlevel = playerData.Weaponlevel;
            healthUpgradeCost = playerData.healthUpgradeCost;
            healthLevel = playerData.healthLevel;
            regenRateCost = playerData.regenRateCost;
            regenRateLevel = playerData.regenRateLevel;
            criticalRateCost = playerData.criticalRateCost;
            criticalRateLevel = playerData.criticalRateLevel;
            defenseCost = playerData.defenseCost;
            defenseLevel = playerData.defenseLevel;
            armorPenetrationCost = playerData.armorPenetrationCost;
            armorPenetrationLevel = playerData.armorPenetrationLevel;
            criticalDamageCost = playerData.criticalDamageCost;
            criticalDamageLevel = playerData.criticalDamageLevel;
            
            // กำหนดค่าปัจจุบันให้เต็ม
            _currentHealth = maxHealth;
            _currentMana = maxMana;
            
            // เก็บค่าเริ่มต้นของทุกสถิติเพื่อใช้เปรียบเทียบภายหลัง
            previousStats["maxHealth"] = _maxHealth;
            previousStats["maxMana"] = _maxMana;
            previousStats["baseDamage"] = _baseDamage;
            previousStats["weaponDamage"] = _weaponDamage;
            previousStats["criticalChance"] = _criticalChance;
            previousStats["criticalDamage"] = _criticalDamage;
            previousStats["defense"] = _defense;
            previousStats["armorPenetration"] = _armorPenetration;
            previousStats["regenRate"] = _regenRate;
            previousStats["manaRegenRate"] = _manaRegenRate;
            
            
        }
    }
    
    // เมธอดสำหรับรีเซ็ตค่าเป็นค่าเริ่มต้น
    public void ResetToDefault()
    {
        if (playerData != null)
        {
            // ให้ playerData รีเซ็ตค่าของตัวเองก่อน (ถ้าต้องการ)
            playerData.ResetToDefault();
            
            // จากนั้นดึงค่ามาใช้
            InitializeFromData();
        }
        else
        {
            // กรณีไม่มี playerData ให้ใช้ค่าตั้งต้น
            maxHealth = 140f;
            maxMana = 100f;
            baseDamage = 10f;
            weaponDamage = 10f;
            criticalChance = 0.05f;
            criticalDamage = 2f;
            defense = 5f;
            armorPenetration = 0f;
            regenRate = 1f;
            manaRegenRate = 20f;
            
            Weaponlevel = 1;
            WeaponupgradeCost = 100;
            healthLevel = 1;
            healthUpgradeCost = 100;
            regenRateLevel = 1;
            regenRateCost = 250;
            criticalRateLevel = 1;
            criticalRateCost = 1000;
            defenseLevel = 1;
            defenseCost = 125;
            armorPenetrationLevel = 1;
            armorPenetrationCost = 200;
            criticalDamageLevel = 1;
            criticalDamageCost = 2500;
            
            // กำหนดค่าปัจจุบันให้เต็ม
            _currentHealth = maxHealth;
            _currentMana = maxMana;
        }
    }
    
    
    // เมธอดที่ส่งคืนรายการของทุกสถิติที่เปลี่ยนแปลงตั้งแต่เฟรมสุดท้าย
    public Dictionary<string, StatChange> GetChangedStats()
    {
        Dictionary<string, StatChange> result = new Dictionary<string, StatChange>();
        
        foreach (var stat in changedStats)
        {
            result[stat.Key] = new StatChange(previousStats[stat.Key], stat.Value);
        }
        
        return result;
    }
    
    // เมธอดที่เช็คว่าสถิติใดเปลี่ยนจากการเก็บค่าเริ่มต้น
    public Dictionary<string, StatChange> GetChangesFromInitial()
    {
        Dictionary<string, StatChange> result = new Dictionary<string, StatChange>();
        
        // ตรวจสอบสถิติทั้งหมดกับค่าเริ่มต้น (ต้องปรับตามค่า default ของสถิติ)
        CheckAndAddChange(result, "maxHealth", 100f, _maxHealth);
        CheckAndAddChange(result, "maxMana", 0f, _maxMana);
        CheckAndAddChange(result, "baseDamage", 10f, _baseDamage);
        CheckAndAddChange(result, "weaponDamage", 10f, _weaponDamage);
        CheckAndAddChange(result, "criticalChance", 0.05f, _criticalChance);
        CheckAndAddChange(result, "criticalDamage", 2f, _criticalDamage);
        CheckAndAddChange(result, "defense", 5f, _defense);
        CheckAndAddChange(result, "armorPenetration", 0f, _armorPenetration);
        CheckAndAddChange(result, "regenRate", 1f, _regenRate);
        CheckAndAddChange(result, "manaRegenRate", 20f, _manaRegenRate);
        
        return result;
    }
    
    private void CheckAndAddChange(Dictionary<string, StatChange> dict, string statName, float initialValue, float currentValue)
    {
        if (Math.Abs(initialValue - currentValue) > 0.001f) // ใช้ epsilon ในการเปรียบเทียบค่า float
        {
            dict[statName] = new StatChange(initialValue, currentValue);
        }
    }
    
    public void RestatAllProperties()
    {
        // บันทึกค่าปัจจุบันทั้งหมด
        Dictionary<string, float> oldValues = new Dictionary<string, float>
        {
            { "maxHealth", _maxHealth }//,
          //  { "maxMana", _maxMana },
            // บันทึกค่าอื่นๆ
        };
    
        // โหลดค่าจาก PlayerData หรือค่าเริ่มต้น
       /* if (playerData != null)
        {
            _maxHealth = playerData.maxHealth;
            _maxMana = playerData.maxMana;
            // กำหนดค่าอื่นๆ
        }
        else
        {
            // กำหนดค่าเริ่มต้นถ้าไม่มี PlayerData
            _maxHealth = 100f;
            _maxMana = 0f;
            // กำหนดค่าอื่นๆ
        }*/
    
        // บังคับส่ง event สำหรับทุก property
        foreach (var stat in oldValues)
        {
            float newValue = 0f;
        
            // ดึงค่าปัจจุบันตามชื่อ stat
            switch (stat.Key)
            {
                case "maxHealth": newValue = _maxHealth; break;
               // case "maxMana": newValue = _maxMana; break;
                // กรณีอื่นๆ
            }
        
            // ส่ง event แม้จะมีค่าเท่าเดิม
            OnSpecificStatChanged?.Invoke(stat.Key, stat.Value, newValue);
            changedStats[stat.Key] = newValue;
        }
    
        statsChangedSinceLastFrame = true;
    }
    
    // คลาสเพื่อเก็บข้อมูลการเปลี่ยนแปลงของสถิติ
    [System.Serializable]
    public class StatChange
    {
        public float oldValue;
        public float newValue;
        public float difference;
        public float percentChange;
    
        public StatChange(float oldVal, float newVal)
        {
            oldValue = oldVal;
            newValue = newVal;
            difference = newValue - oldValue;
            percentChange = (oldValue != 0) ? (difference / oldValue) * 100f : 0f;
        }
    }
}