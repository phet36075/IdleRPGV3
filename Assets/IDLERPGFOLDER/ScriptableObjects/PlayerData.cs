using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "Game/Player Data")]

public class PlayerData : ScriptableObject
{
    [Header("----Health----")]
    public float DefaultMaxHealth;
    public float maxHealth = 100f;
    public float currentHealth;
    [Header("----Damage----")]
    public float baseDamage = 10f;
    public float DefaultbaseDamage;
    public float weaponDamage = 10f;
    [Header("----Critical----")]
    public float criticalChance = 0.05f;
    public float criticalDamage = 2f;
    [Header("----Defense----")]
    public float defense = 5f;
    [Header("----Armor Penetration----")]
    public float armorPenetration = 0f;
   // public bool isCritical;
   [Header("----Damage Variation---")]
    public float damageVariation = 0.2f; // 20% variation
    [Header("----Regen Rate----")]
    public float regenRate = 1f;

    [Header("----Move Speed----")]
    public float moveSpeed = 2;

  
    
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
        baseDamage = DefaultbaseDamage;
        weaponDamage = 5f;
        criticalChance = 0.05f;
        defense = 5f;
        armorPenetration = 0;
     
        
        Weaponlevel = 1;
        WeaponupgradeCost = 100;

        maxHealth = DefaultMaxHealth;
        currentHealth = maxHealth;
        healthLevel = 1;
        healthUpgradeCost = 100;

        regenRate = 5;
        regenRateLevel = 1;
        regenRateCost = 250;

        criticalRateLevel = 1;
        criticalRateCost = 1000;

        defense = 5;
        defenseLevel = 1;
        defenseCost = 125;

        armorPenetration = 0;
        armorPenetrationLevel = 1;
        armorPenetrationCost = 200;

        criticalDamage = 2;
        criticalDamageLevel = 1;
        criticalDamageCost = 2500;

    }
    
    
}
