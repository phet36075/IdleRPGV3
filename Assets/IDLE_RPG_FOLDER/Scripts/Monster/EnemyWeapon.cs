using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyWeapon : MonoBehaviour
{
    public float damageVariation = 0.2f;
    private EnemyHealth _enemyData;
    void OnTriggerEnter(Collider other)
    {
        // ตรวจสอบว่าชนกับผู้เล่น
        if (other.CompareTag("Player"))
        {
            // เข้าถึงสคริปต์ PlayerHealth ของผู้เล่น
            PlayerManager playerHealth = other.GetComponent<PlayerManager>();
            int finalDamage = CalculateDamage();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(finalDamage,_enemyData.EnemyData.armorPenetration);  // ส่งดาเมจให้กับผู้เล่น
            }
        }
    }
    
    int CalculateDamage()
    {
        float randomFactor = Random.Range(1f - damageVariation, 1f + damageVariation);
        int finalDamage = Mathf.RoundToInt(_enemyData.EnemyData.BaseAttack * randomFactor);
        return finalDamage;
        
    }

    private void Start()
    {
        _enemyData = FindObjectOfType<EnemyHealth>();
    }
}
