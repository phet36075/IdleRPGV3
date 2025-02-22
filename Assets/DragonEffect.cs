using System;
using System.Collections;
using UnityEngine;

public class DragonEffect : MonoBehaviour
{
    public GameObject flameEffect;
    public GameObject screamEffect;
    private Transform player;
    public Collider normalAttackHtibox;
    [SerializeField] private DragonMovement dragonMovement;
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;  // ระยะห่างของ hitbox
    public void EnableFlame()
    {
        if (flameEffect != null)
        {
            flameEffect.SetActive(true);
        }
    }
    public void DisableFlame()
    {
        if (flameEffect != null)
        {
            flameEffect.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normalAttackHtibox.enabled = false;
        DisableFlame();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

   

    public void EnableHitbox()
    {
        StartCoroutine(animDealsDamage());
    }

    public void ScreamAnim()
    {
        Vector3 spawnPosition = transform.position + transform.rotation *hitboxOffset;
        GameObject spawnedEffect = Instantiate(screamEffect, spawnPosition, screamEffect.transform.rotation);
        
        Destroy(spawnedEffect,2f);
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= dragonMovement.attackRange)
        {
            IDamageable target = player.GetComponent<IDamageable>();
            if (target != null)
            {
                DamageData damageData = new DamageData(
                    20f,
                    100f,
                    ElementType.Dark);
                target.TakeDamage(damageData);
            }
        }
    }
    IEnumerator animDealsDamage()
    {
        normalAttackHtibox.enabled = true;
      
        yield return new WaitForSeconds(0.1f); // เปิดให้โจมตีได้สั้น ๆ
        normalAttackHtibox.enabled = false; // ปิด Hitbox


        //int finalDamage = CalculateDamage();
        //  PlayerManager playerManager = player.GetComponent<PlayerManager>();

        // playerManager.TakeDamage(finalDamage,_enemyData.EnemyData.armorPenetration);

        // DamageData damageData = new DamageData(
        //     finalDamage,
        //     10,
        //     ElementType.None
        // );
        // playerManager.TakeDamage(damageData);

    }
    /*int CalculateDamage()
    {
        float randomFactor = Random.Range(1f - damageVariation, 1f + damageVariation);
        int finalDamage = Mathf.RoundToInt(((_enemyData.EnemyData.BaseAttack * _enemySpawner.currentStage) * 1.25f) * randomFactor);
        return finalDamage;
        
    }*/
    // Update is called once per frame
    void Update()
    {
        
    }
}
