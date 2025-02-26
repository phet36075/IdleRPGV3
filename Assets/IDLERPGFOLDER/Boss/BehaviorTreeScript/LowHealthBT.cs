using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class LowHealthBT : Conditional
{
    public float LowHealthFirstValue = 0.70f;
    public float LowHealthSecondValue = 0.35f;
    public EnemyHealth enemyHealth;
    protected UnityEngine.AI.NavMeshAgent navMeshAgent;
    //private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnAwake()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        if (ShouldSuccess())
        {
            navMeshAgent.ResetPath();
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private bool hasTriggered70 = false;
    private bool hasTriggered30 = false;
    public bool ShouldSuccess()
    {
        
        float currentHealth = enemyHealth.GetCurrentHealth();
        float maxHealth = enemyHealth.GetCurrentMaxHealth();
        if (currentHealth < maxHealth * LowHealthFirstValue  && !hasTriggered70)
        {
            hasTriggered70 = true; // ป้องกันการเรียกซ้ำ
            return true;
        }
        if (currentHealth < maxHealth *LowHealthSecondValue && !hasTriggered30)
        {
            hasTriggered30 = true; // ป้องกันการเรียกซ้ำ
            return true;
        }

        return false; // ไม่เข้าเงื่อนไข
    }
}
