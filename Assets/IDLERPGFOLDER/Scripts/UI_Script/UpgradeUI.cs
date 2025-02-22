using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using TMPro;

using System;
public class UpgradeUI : MonoBehaviour
{
    public PlayerManager playerManager;
    
    public PlayerStats _PlayerStats;
    public PlayerProperty playerProperty;
    private CurrencyManager _currencyManager;
    private ProjectileMovement _projectileMovement;
    public TextMeshProUGUI WeaponUpgradeCostTxT;
    public TextMeshProUGUI WeaponDmg;

    public TextMeshProUGUI WeaponDmg2;
    public TextMeshProUGUI Strtxt;
    public TextMeshProUGUI HealthUpgradeCostTxT;
    public TextMeshProUGUI HealthTxT;
    
    public TextMeshProUGUI RegenUpgradeCostTxT;
    public TextMeshProUGUI RegenTxT;
    
    public TextMeshProUGUI CriticalUpgradeCostTxT;
    public TextMeshProUGUI CriticalTxT;
    
    
    public TextMeshProUGUI DefenseUpgradeCostTxT;
    public TextMeshProUGUI DefenseTxT;
    
    public TextMeshProUGUI PenetrationUpgradeCostTxT;
    public TextMeshProUGUI PenetrationTxT;
    
    public TextMeshProUGUI CriticalDamageUpgradeCostTxT;
    public TextMeshProUGUI CriticalDamageTxT;
    
   /* public TextMeshProUGUI AllyDamageUpgradeCostTxT;
    public TextMeshProUGUI AllyDamageTxT;*/
    //public int upgradeCost;
    private float level = 1;
    private bool isVisible = false;
    // Start is called before the first frame update
    void Start()
    {
        _currencyManager = FindObjectOfType<CurrencyManager>();
        _projectileMovement = FindObjectOfType<ProjectileMovement>();
        // _PlayerData.upgradeCost = 100;
    }

    // Update is called once per frame
    void Update()
    {
        WeaponUpgradeCostTxT.text = playerProperty.WeaponupgradeCost.ToString();
        WeaponDmg.text = playerProperty.weaponDamage.ToString();
        WeaponDmg2.text = playerProperty.baseDamage.ToString();
        Strtxt.text = _PlayerStats.GetStat(StatType.Strength).ToString();
        
        
        HealthUpgradeCostTxT.text = playerProperty.healthUpgradeCost.ToString();
        HealthTxT.text = playerProperty.maxHealth.ToString();
        
        RegenUpgradeCostTxT.text = playerProperty.regenRateCost.ToString();
        RegenTxT.text = playerProperty.regenRate.ToString();
        
        CriticalUpgradeCostTxT.text = playerProperty.criticalRateCost.ToString();
        CriticalTxT.text = playerProperty.criticalChance.ToString();

        DefenseUpgradeCostTxT.text = playerProperty.defenseCost.ToString();
        DefenseTxT.text = playerProperty.defense.ToString();
        
        PenetrationUpgradeCostTxT.text = playerProperty.armorPenetrationCost.ToString();
        PenetrationTxT.text = playerProperty.armorPenetration.ToString();

        CriticalDamageUpgradeCostTxT.text = playerProperty.criticalDamageCost.ToString();
        CriticalDamageTxT.text = playerProperty.criticalDamage.ToString();

        
    }

    public void UpgradeWeapon()
    {
        if (_currencyManager.CurrentMoney >= playerProperty.WeaponupgradeCost)
        {
            playerProperty.Weaponlevel++;
            _currencyManager.SpendMoney(playerProperty.WeaponupgradeCost);
            playerProperty.weaponDamage += 5;
            playerProperty.WeaponupgradeCost = (int)Math.Round(100*( playerProperty.Weaponlevel * 1.5f));
        }
    }

    public void UpgradeHealth()
    {
        if (_currencyManager.CurrentMoney >= playerProperty.healthUpgradeCost)
        {
            playerProperty.healthLevel++;
            _currencyManager.SpendMoney(playerProperty.healthUpgradeCost);
            playerProperty.maxHealth += 150;
            playerProperty.healthUpgradeCost= (int)Math.Round(100*( playerProperty.healthLevel * 1.25f));
            playerManager.UpdateHealthBar();
        }
    }

    public void UpgradeRegen()
    {
        if (_currencyManager.CurrentMoney >= playerProperty.regenRateCost)
        {
            playerProperty.regenRateLevel++;
            _currencyManager.SpendMoney(playerProperty.regenRateCost);
            playerProperty.regenRate += 10;
            playerProperty.regenRateCost= (int)Math.Round(100*( playerProperty.regenRateLevel * 1.5f));
        }
    }
    public void UpgradeCritical()
    {
        if (_currencyManager.CurrentMoney >= playerProperty.criticalRateCost)
        {
            playerProperty.criticalRateLevel++;
            _currencyManager.SpendMoney(playerProperty.criticalRateCost);
            playerProperty.criticalChance += 0.01f;
            playerProperty.criticalRateCost= (int)Math.Round(100*( playerProperty.criticalRateLevel * 1.5f));
        }
    }
    
    public void UpgradeDefense()
    {
        if (_currencyManager.CurrentMoney >= playerProperty.defenseCost)
        {
            playerProperty.defenseLevel++;
            _currencyManager.SpendMoney(playerProperty.defenseCost);
            playerProperty.defense += 30f;
            playerProperty.defenseCost= (int)Math.Round(100*( playerProperty.defenseLevel * 1.5f));
        }
    }
    
    public void UpgradePenetration()
    {
        if (_currencyManager.CurrentMoney >= playerProperty.armorPenetrationCost)
        {
            playerProperty.armorPenetrationLevel++;
            _currencyManager.SpendMoney(playerProperty.armorPenetrationCost);
            playerProperty.armorPenetration += 5f;
            playerProperty.armorPenetrationCost= (int)Math.Round(100*( playerProperty.armorPenetrationLevel * 1.5f));
        }
    }
    
    public void UpgradeCriticalDamage()
    {
        if (_currencyManager.CurrentMoney >= playerProperty.criticalDamageCost)
        {
            playerProperty.criticalDamageLevel++;
            _currencyManager.SpendMoney(playerProperty.criticalDamageCost);
            playerProperty.criticalDamage += 0.05f;
            playerProperty.criticalDamageCost= (int)Math.Round(100*( playerProperty.criticalDamageLevel * 1.5f));
        }
    }
    public void ToggleUpgradeUI()
    {
        isVisible = !isVisible;
        gameObject.SetActive(isVisible);
    }
}
