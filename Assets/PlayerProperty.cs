using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : MonoBehaviour
{
    public PlayerData playerData; // อ้างอิงไปยัง ScriptableObject เพื่อใช้เป็นค่าเริ่มต้น
    
    public event Action OnStatsChanged;
    private bool statsChangedSinceLastFrame = false;
    [Header("----Health----")]
    [SerializeField] private float _maxHealth = 100f;
    public float maxHealth
    {
        get => _maxHealth;
        set
        {
            if (_maxHealth != value)
            {
                _maxHealth = value;
                statsChangedSinceLastFrame = true;
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
                _maxMana = value;
                statsChangedSinceLastFrame = true;
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
                _baseDamage = value;
                statsChangedSinceLastFrame = true;
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
                _weaponDamage = value;
                statsChangedSinceLastFrame = true;
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
                _criticalChance = value;
                statsChangedSinceLastFrame = true;
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
                _criticalDamage = value;
                statsChangedSinceLastFrame = true;
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
                _defense = value;
                statsChangedSinceLastFrame = true;
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
                _armorPenetration = value;
                statsChangedSinceLastFrame = true;
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
                _regenRate = value;
                statsChangedSinceLastFrame = true;
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
                _manaRegenRate = value;
                statsChangedSinceLastFrame = true;
            }
        }
    }
    void Update()
    {
        if (statsChangedSinceLastFrame)
        {
            statsChangedSinceLastFrame = false;
            OnStatsChanged?.Invoke();
        }
    }
    
    [Header("----Damage Variation---")]
    public float damageVariation = 0.2f; // 20% variation
    
    [Header("Elemental")]
    public ElementType elementType = ElementType.None;  // ธาตุเริ่มต้น

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
    
   
}