using UnityEngine;

// ตัวอย่างสกิล SpinAttack
public class SpinAttackSkill : BaseSkill
{
    [SerializeField] private GameObject spinEffectPrefab;
    [SerializeField] private GameObject hitboxPrefab;
    private GameObject currentEffect;

    public override void OnEffectSpawn()
    {
        base.OnEffectSpawn();
        currentEffect = Instantiate(spinEffectPrefab, transform.position, transform.rotation);
        currentEffect.transform.parent = transform; // ให้ effect หมุนตามตัวละคร
    }

    public override void OnHitboxActivate()
    {
        base.OnHitboxActivate();
        var hitbox = Instantiate(hitboxPrefab, transform.position, transform.rotation);
        Destroy(hitbox, 0.3f);
    }

    public override void OnSkillEnd()
    {
        base.OnSkillEnd();
        if (currentEffect != null)
        {
            Destroy(currentEffect);
        }
    }
}
