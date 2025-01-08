using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using TMPro;

using System;
public class UpgradeUI : MonoBehaviour
{
    public PlayerData _PlayerData;
    private CurrencyManager _currencyManager;
    private ProjectileMovement _projectileMovement;
    public TextMeshProUGUI WeaponUpgradeCostTxT;
    public TextMeshProUGUI WeaponDmg;

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
        WeaponUpgradeCostTxT.text = _PlayerData.WeaponupgradeCost.ToString();
        WeaponDmg.text = _PlayerData.weaponDamage.ToString();
        
        HealthUpgradeCostTxT.text = _PlayerData.healthUpgradeCost.ToString();
        HealthTxT.text = _PlayerData.maxHealth.ToString();
        
        RegenUpgradeCostTxT.text = _PlayerData.regenRateCost.ToString();
        RegenTxT.text = _PlayerData.regenRate.ToString();
        
        CriticalUpgradeCostTxT.text = _PlayerData.criticalRateCost.ToString();
        CriticalTxT.text = _PlayerData.criticalChance.ToString();

        DefenseUpgradeCostTxT.text = _PlayerData.defenseCost.ToString();
        DefenseTxT.text = _PlayerData.defense.ToString();
        
        PenetrationUpgradeCostTxT.text = _PlayerData.armorPenetrationCost.ToString();
        PenetrationTxT.text = _PlayerData.armorPenetration.ToString();

        CriticalDamageUpgradeCostTxT.text = _PlayerData.criticalDamageCost.ToString();
        CriticalDamageTxT.text = _PlayerData.criticalDamage.ToString();

        
    }

    public void UpgradeWeapon()
    {
        if (_currencyManager.CurrentMoney >= _PlayerData.WeaponupgradeCost)
        {
            _PlayerData.Weaponlevel++;
            _currencyManager.SpendMoney(_PlayerData.WeaponupgradeCost);
            _PlayerData.weaponDamage += 5;
            _PlayerData.WeaponupgradeCost = (int)Math.Round(100*( _PlayerData.Weaponlevel * 1.5f));
        }
    }

    public void UpgradeHealth()
    {
        if (_currencyManager.CurrentMoney >= _PlayerData.healthUpgradeCost)
        {
            _PlayerData.healthLevel++;
            _currencyManager.SpendMoney(_PlayerData.healthUpgradeCost);
            _PlayerData.maxHealth += 150;
            _PlayerData.healthUpgradeCost= (int)Math.Round(100*( _PlayerData.healthLevel * 1.25f));
        }
    }

    public void UpgradeRegen()
    {
        if (_currencyManager.CurrentMoney >= _PlayerData.regenRateCost)
        {
            _PlayerData.regenRateLevel++;
            _currencyManager.SpendMoney(_PlayerData.regenRateCost);
            _PlayerData.regenRate += 10;
            _PlayerData.regenRateCost= (int)Math.Round(100*( _PlayerData.regenRateLevel * 1.5f));
        }
    }
    public void UpgradeCritical()
    {
        if (_currencyManager.CurrentMoney >= _PlayerData.criticalRateCost)
        {
            _PlayerData.criticalRateLevel++;
            _currencyManager.SpendMoney(_PlayerData.criticalRateCost);
            _PlayerData.criticalChance += 0.01f;
            _PlayerData.criticalRateCost= (int)Math.Round(100*( _PlayerData.criticalRateLevel * 1.5f));
        }
    }
    
    public void UpgradeDefense()
    {
        if (_currencyManager.CurrentMoney >= _PlayerData.defenseCost)
        {
            _PlayerData.defenseLevel++;
            _currencyManager.SpendMoney(_PlayerData.defenseCost);
            _PlayerData.defense += 30f;
            _PlayerData.defenseCost= (int)Math.Round(100*( _PlayerData.defenseLevel * 1.5f));
        }
    }
    
    public void UpgradePenetration()
    {
        if (_currencyManager.CurrentMoney >= _PlayerData.armorPenetrationCost)
        {
            _PlayerData.armorPenetrationLevel++;
            _currencyManager.SpendMoney(_PlayerData.armorPenetrationCost);
            _PlayerData.armorPenetration += 5f;
            _PlayerData.armorPenetrationCost= (int)Math.Round(100*( _PlayerData.armorPenetrationLevel * 1.5f));
        }
    }
    
    public void UpgradeCriticalDamage()
    {
        if (_currencyManager.CurrentMoney >= _PlayerData.criticalDamageCost)
        {
            _PlayerData.criticalDamageLevel++;
            _currencyManager.SpendMoney(_PlayerData.criticalDamageCost);
            _PlayerData.criticalDamage += 0.05f;
            _PlayerData.criticalDamageCost= (int)Math.Round(100*( _PlayerData.criticalDamageLevel * 1.5f));
        }
    }
    public void ToggleUpgradeUI()
    {
        isVisible = !isVisible;
        gameObject.SetActive(isVisible);
    }
}
