using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoss", menuName = "Game/Boss Data")]
public class BossData : ScriptableObject
{
    public float maxhealth = 50f;
    public string enemyName = "Enemy";
    public float defense = 5f;
    public float armorPenetration = 5f;
    public bool isCritical = false;
}
