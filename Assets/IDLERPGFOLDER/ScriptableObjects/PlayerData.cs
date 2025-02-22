using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("----Health----")]
    [SerializeField] private float _maxHealth = 140f;
    public float maxHealth => _maxHealth;

    [Header("----Mana----")]
    [SerializeField] private float _maxMana = 100f;
    public float maxMana => _maxMana;

    [Header("----Damage----")]
    [SerializeField] private float _baseDamage = 10f;
    public float baseDamage => _baseDamage;

    [SerializeField] private float _weaponDamage = 10f;
    public float weaponDamage => _weaponDamage;

    [Header("----Critical----")]
    [SerializeField] private float _criticalChance = 0.05f;
    public float criticalChance => _criticalChance;

    [SerializeField] private float _criticalDamage = 2f;
    public float criticalDamage => _criticalDamage;

    [Header("----Defense----")]
    [SerializeField] private float _defense = 5f;
    public float defense => _defense;

    [Header("----Armor Penetration----")]
    [SerializeField] private float _armorPenetration = 0f;
    public float armorPenetration => _armorPenetration;

    [Header("----Regen Rate----")]
    [SerializeField] private float _regenRate = 1f;
    public float regenRate => _regenRate;

    [SerializeField] private float _manaRegenRate = 20f;
    public float manaRegenRate => _manaRegenRate;
    
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

    private void OnEnable()
    {
        ResetToDefault();
    }
    
    public void ResetToDefault()
    {
        // ค่าพื้นฐานของตัวละคร
        _maxHealth = 140f;
        _maxMana = 100f;
        _baseDamage = 10f;
        _weaponDamage = 10f;
        _criticalChance = 0.05f;
        _criticalDamage = 2f;
        _defense = 5f;
        _armorPenetration = 0f;
        _regenRate = 1f;
        _manaRegenRate = 20f;
        
        // ค่าเริ่มต้นสำหรับการอัพเกรด
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
    }
}