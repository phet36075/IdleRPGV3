using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Game/Monster Data")]
public class EnemyData : ScriptableObject
{
   
    public float maxhealth = 50f;
    public float DefaultMaxHealth;
    public string enemyName = "Enemy";
    public float defense = 5f;
    public float DefaultDefense = 5f;
    public float armorPenetration = 5f;
    public float DefaultArmorPenetration = 5f;
    public bool isCritical = false;
    public float BaseAttack = 10f;
    public float DefaultBaseAttack;
    public int moneyDrop = 100;
    public int DefaultmoneyDrop;


    private void OnEnable()
    {
        maxhealth = DefaultMaxHealth;
        BaseAttack = DefaultBaseAttack;
        moneyDrop = DefaultmoneyDrop;
        defense = DefaultDefense;
        armorPenetration = DefaultArmorPenetration;
    }
}