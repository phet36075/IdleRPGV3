using System;
using System.Collections;
using UnityEngine;

public class Skill1ParticleDamage1 : ISkillParticle
{
   /* private PlayerManager playerManager;
    private float attackDamage;
    private DamageData damageData;
    */
   
    private Skill1 skill1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   protected override void Start()
    {
        base.Start();
        skill1 = FindObjectOfType<Skill1>();
       /* playerManager = FindObjectOfType<PlayerManager>();
        attackDamage = playerManager.CalculatePlayerAttackDamage();
        damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
                UpdateDamage();
                target.TakeDamage(damageData); 
            
                StartCoroutine(MultipleHits(target));
            
        }
    }
  protected override void UpdateDamage()
    {
        attackDamage = playerManager.CalculatePlayerAttackDamage(skill1.skillDmgLastHitAttack);
        damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);
    }

    private IEnumerator MultipleHits(IDamageable target)
    {
        for (int i = 0; i < skill1.numberOfHits; i++)
        {
            UpdateDamage();
            yield return new WaitForSeconds(skill1.timeBetweenHits);
            target.TakeDamage(damageData);
        }
    }
}
