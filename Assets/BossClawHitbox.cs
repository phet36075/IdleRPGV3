using UnityEngine;

public class BossClawHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            DamageData damageData = new DamageData(
                5f,
                100f,
                ElementType.Dark);
            target.TakeDamage(damageData);
        }
    }
}
