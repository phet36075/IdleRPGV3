using UnityEngine;

public class BossClawHitbox : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            int finalDamage = enemyHealth.CalculateAttackDamage();
            DamageData damageData = new DamageData(
                finalDamage,
                enemyHealth.EnemyData.armorPenetration,
                ElementType.Dark);
            target.TakeDamage(damageData);
        }
    }
}
