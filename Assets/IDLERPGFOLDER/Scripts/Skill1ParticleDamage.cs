using System;
using UnityEngine;

public class Skill1ParticleDamage : ISkillParticle
{
    
    private Skill1 skill1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        skill1 = FindObjectOfType<Skill1>();
        /*playerManager = FindObjectOfType<PlayerManager>();
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
        }
    }
    protected override void UpdateDamage()
    {
        attackDamage = playerManager.CalculatePlayerAttackDamage(skill1.skillDmgFirst2Hit);
        damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);
    }
}
