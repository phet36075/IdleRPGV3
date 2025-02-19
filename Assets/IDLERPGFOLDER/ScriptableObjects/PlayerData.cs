using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "Game/Player Data")]

public class PlayerData : ScriptableObject
{
    public event Action OnStatsChanged;

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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
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
                OnStatsChanged?.Invoke();
            }
        }
    }
    [Header("----Damage Variation---")]
    public float damageVariation = 0.2f; // 20% variation
  
    
    [Header("Elemental")]
    public ElementType elementType = ElementType.None;  // ธาตุเริ่มต้น
    
    // public int stage = 1;
    // ชั่วคราว
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
    private void OnEnable()
    {
       
        ResetToDefault();
    }
    
    public void ResetToDefault()
    {
        _maxHealth = 100f;
        _maxMana = 100f;
        _baseDamage = 10f;
        _weaponDamage = 10f;
        _criticalChance = 0.05f;
        _criticalDamage = 2f;
        _defense = 5f;
        _armorPenetration = 0f;
        _regenRate = 1f;
        _manaRegenRate = 20f;
        
        // weaponDamage = 5f;
        // criticalChance = 0.05f;
        // defense = 5f;
        // armorPenetration = 0;
        Weaponlevel = 1;
        WeaponupgradeCost = 100;

    
        healthLevel = 1;
        healthUpgradeCost = 100;

        // regenRate = 5;
        regenRateLevel = 1;
        regenRateCost = 250;

        criticalRateLevel = 1;
        criticalRateCost = 1000;

        // defense = 5;
        defenseLevel = 1;
        defenseCost = 125;

        // armorPenetration = 0;
        armorPenetrationLevel = 1;
        armorPenetrationCost = 200;

        // criticalDamage = 2;
        criticalDamageLevel = 1;
        criticalDamageCost = 2500;

    }
    
    
}
