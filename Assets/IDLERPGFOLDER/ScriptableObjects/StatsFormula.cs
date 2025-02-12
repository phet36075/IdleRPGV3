using UnityEngine;

[CreateAssetMenu(fileName = "StatsFormula", menuName = "Scriptable Objects/StatsFormula")]
public class StatsFormula : ScriptableObject
{
    [Header("HP Formula")]
    public float baseHP = 100f;
    public float hpPerVit = 10f;
    public float regenPerVit = 10f;
    public float hpPerLevel = 20f;

    [Header("Mana Formula")] public float baseMana = 100f;
    public float baseManaRegen = 10f;
    public float manaPerInt = 0.1f;
    public float manaRegenPerInt = 2f;
    [Header("Damage Formula")]
    public float baseDamage = 10f;
    public float damagePerStr = 2f;
    public float criticalMultiplier = 1.5f;
    public float criticalChancePerDex = 0.01f; // 1% ต่อ 1 Dex
    public float criticalChancePerAgi = 0.005f; // 0.5% ต่อ 1 Agi
    public float armorPenatrationPerAgi = 2f; // 2 ต่อ 1 Agi
    
    
    [Header("Defense Formula")]
    public float baseDefense = 5f;
    public float baseRegen = 5f;
    public float defensePerVit = 1f;
    
    public float evasionPerAgi = 0.005f; // 0.5% ต่อ 1 Agi
}
