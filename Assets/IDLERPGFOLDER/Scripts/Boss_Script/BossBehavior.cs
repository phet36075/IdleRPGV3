using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BossBehavior : MonoBehaviour
{
    [SerializeField] private List<BaseBossSkill> bossSkills = new List<BaseBossSkill>();
    
    [Header("References")]
    public EnemyData _BossData;
    public Animator animator;
    
    [Header("Combat Stats")]
    public float attackRange = 2f;
    public float chaseRange = 5f;
    public float attackCooldown = 1f;
    public float skillCooldown = 5f;
    
    [Header("Movement")]
    public float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private Transform player;
    private float lastSkillTime = 0;
    private float lastAttackTime;

    public bool IsUsingSkill;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        IsUsingSkill = false;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (IsUsingSkill) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleBossMovement(distanceToPlayer);
    }

    private void HandleBossMovement(float distanceToPlayer)
    {
        bool isInChaseRange = distanceToPlayer <= chaseRange;
        bool isInAttackRange = distanceToPlayer <= attackRange;
        
        animator.SetBool("IsWalking", isInChaseRange && !isInAttackRange);
        animator.SetBool("IsAttacking", isInAttackRange);

        if (!isInChaseRange) return;

        if (isInAttackRange)
        {
            AttackPlayer();
            RotateTowardsTarget();
            agent.SetDestination(transform.position);
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    public void animPunch()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            PlayerManager playerManager = player.GetComponent<PlayerManager>();
            playerManager.TakeDamage(_BossData.BaseAttack, _BossData.armorPenetration);
        }
    }

    private void AttackPlayer()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        if (Time.time - lastSkillTime >= skillCooldown)
        {
            Debug.Log("Random Skill BOss");
            UseRandomSkill();
        }
       // UseRandomSkill();
       /* if (Random.value < 0.01f && Time.time - lastSkillTime >= skillCooldown)
        {
            Debug.Log("Random Skill BOss");
           
        }*/
    }

    private void UseRandomSkill()
    {
        if (bossSkills.Count == 0) return;
        
        int randomIndex = Random.Range(0, bossSkills.Count);
        UseSkill(randomIndex);
        lastSkillTime = Time.time;
    }

    public void UseSkill(int index)
    {
        if (index >= 0 && index < bossSkills.Count && !IsUsingSkill)
        {
            IsUsingSkill = true;
            RotateTowardsTarget();
            if (agent.enabled)
            {
                agent.SetDestination(transform.position);
            }
           
            bossSkills[index].UseSkill();
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    
    public Transform GetPlayerTransform() => player;
}